using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Dns;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace UBA
{
    class NetworkDataFetcher : DataFetcher
    {
        // it's the shared queue where the generated events need to be added
        private BlockingCollection<Event> events;

        // an intermediary blocking queue of the intercepted packets
        // the sniffing threads only adds the packets in this queue and they will be processed later
        private BlockingCollection<Packet> packets = new BlockingCollection<Packet>();
        // a blocking queue of ips which need to be checked (country of origin)
        private BlockingCollection<string> ips_to_be_checked = new BlockingCollection<string>();
        // already verified IPs
        private Dictionary<string, string> checked_ips = new Dictionary<string, string>();
        // a blocking dictionary which contains "links" between opened ports and processes(pid + process name)
        private ConcurrentDictionary<int, ProcessData> used_ports = new ConcurrentDictionary<int, ProcessData>();

        private string api_url = "http://ip-api.com/line";  // the URL of the API which gives the country origin of the IPs

        // the IPs of the sniffed network interface
        public string deviceIPv4;
        public string deviceIPv6;

        private int interface_no;           // the index of the sniffed network interface

        // the threads which will do the job
        private Thread sniffingThread;
        private Thread countriesThread;
        private Thread fsdManagerThread;
        private Thread netstatThread;

        public Process netstat;

        // hard coded dict of known ports - dynamically loading of these ports SHOULD be added at a later time
        private Dictionary<int, string> wellKnownPorts = new Dictionary<int, string>
        {
            { 7, "Echo" },
            { 18, "Message Send Protocol (MSP)" },
            { 20, "FTP" },
            { 21, "FTP" },
            { 22, "SSH" },
            { 23, "Telnet" },
            { 25, "SMTP" },
            { 37, "Time" },
            { 53, "DNS"},
            { 69, "TFTP" },
            { 80, "HTTP" },
            {109, "POP2" },
            {110, "POP3" },
            {115, "SFTP" },
            {194, "IRC" },
            {443, "HTTPS"}
        };

        // class used as a struct of a process name and its pid
        internal class ProcessData
        {
            public int pid;
            public string name;
        }

        // a blocking queue of "full socket" information which is read by a thread and updated by another
        private BlockingCollection<FullSocketData> fsdQueue = new BlockingCollection<FullSocketData>();

        // abstraction of (sockets + processes + protocols + data traffic)
        internal class FullSocketData
        {
            public int pid;
            public string pname;
            public int localPort;
            public string server;
            public int serverPort;
            public string tprotocol;
            public string protocol;
            public int sent;
            public int received;
            public int packetCount;

            public FullSocketData(int pid, string pname, int localPort, string server, int serverPort, string tprotocol, string protocol, int sent, int received, int packetCount)
            {
                this.pid = pid;
                this.pname = pname;
                this.localPort = localPort;
                this.server = server;
                this.serverPort = serverPort;
                this.tprotocol = tprotocol;
                this.protocol = protocol;
                this.sent = sent;
                this.received = received;
                this.packetCount = packetCount;
            }

            // if they are the same socket it will add the sent and received values
            // it will also return true
            public bool SameSocketMerge(FullSocketData fsd)
            {
                if (localPort == fsd.localPort && server == fsd.server && serverPort == fsd.serverPort
                    && tprotocol == fsd.tprotocol && protocol == fsd.protocol)
                {
                    if (pid == -1)
                        pid = fsd.pid;
                    if (pname == "")
                        pname = fsd.pname;

                    sent += fsd.sent;
                    received += fsd.received;
                    packetCount += fsd.packetCount;

                    return true;
                }
                return false;
            }
        }

        // used to convert a FSD object to a Event object
        private SEvent FSDToEvent(FullSocketData fsd)
        {
            SEvent se = new SEvent(40, DateTime.Now, "local Network usage", fsd.pname, EventType.SINGLE,
                    new string[] { "pname", "pid", "local_port", "server", "server_port", "transport_protocol", "protocol", "sent", "received", "packets_counter" },
                    new string[] { fsd.pname, fsd.pid.ToString(), fsd.localPort.ToString(), fsd.server, fsd.serverPort.ToString(), fsd.tprotocol, fsd.protocol.ToString(), fsd.sent.ToString(), fsd.received.ToString(), fsd.packetCount.ToString() });
            return se;
        }

        // the constructor - used to init the shared events list and set the sniffed interface
        public NetworkDataFetcher(BlockingCollection<Event> events, int interface_no)
        {
            this.events = events;
            this.interface_no = interface_no;
        }

        // modified code from: https://gist.github.com/cheynewallace/5971686
        // calls netstat once per 2 seconds to find what process uses which open port
        public void GetNetStatPorts()
        {
            try
            {
                netstat = new Process();
                ProcessStartInfo ps = new ProcessStartInfo();
                ps.Arguments = "-a -n -o 2";
                ps.FileName = "netstat.exe";
                ps.UseShellExecute = false;
                ps.WindowStyle = ProcessWindowStyle.Hidden;
                ps.CreateNoWindow = true;
                ps.RedirectStandardOutput = true;

                netstat.StartInfo = ps;
                netstat.Start();

                StreamReader stdOutput = netstat.StandardOutput;

                while(true)
                {
                    string[] tokens = Regex.Split(stdOutput.ReadLine(), "\\s+");

                    if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                    {
                        string localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                        int port_number = Int32.Parse(localAddress.Split(':')[1]);
                        int ppid = tokens[1] == "UDP" ? Convert.ToInt16(tokens[4]) : Convert.ToInt16(tokens[5]);
                        string process_name = LookupProcess(ppid);
                        used_ports.TryAdd(port_number, new ProcessData()
                        {
                            name = process_name,
                            pid = ppid
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
        // code related to the method above
        public string LookupProcess(int pid)
        {
            string procName;
            try
            {
                Process p = Process.GetProcessById(pid);
                procName = p.ProcessName;
            }
            catch (Exception)
            {
                procName = "-";
            }
            return procName;
        }

        // calls the API to get the country origin of the given IP
        public string GetCountry(string ip)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("{0}/{1}", api_url, ip));
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                string resp = reader.ReadToEnd();

                if (resp.StartsWith("success"))
                    return resp.Split('\n')[1];
                else
                    return null;
            }
        }

        // check the origin countries of all IPs and creates events
        // ran on a separate thread
        private void CheckCountries()
        {
            int api_count = 0;

            while (true)
            {
                // if empty, wait
                if (ips_to_be_checked.Count == 0)
                {
                    Thread.Sleep(5000);
                    continue;
                }

                string ip = ips_to_be_checked.Take();

                if (!checked_ips.ContainsKey(ip))
                {
                    string country = GetCountry(ip);
                    api_count++;

                    checked_ips.Add(ip, country);

                    // create event
                    Event e = new SEvent(42, DateTime.Now, "communication with country X", "network", EventType.SINGLE, new string[] { "IP", "Country" }, new string[] { ip, country });
                    ParseData(e);

                    // timeout to not get banned
                    if (api_count == 100)
                    {
                        Thread.Sleep(60000);
                        api_count = 0;
                    }
                }

            }
        }

        // manages the "full sockets"
        // periodically creates events for each FSD object
        private void FSDManager()
        {
            List<FullSocketData> fsdList = new List<FullSocketData>();

            while(true)
            {
                // if empty, wait
                if (fsdQueue.Count == 0)
                {
                    Thread.Sleep(3000);
                    continue;
                }

                // move all FSDs from the concurrent queue to the local list
                int count = fsdQueue.Count;
                int counter = 0;
                foreach(FullSocketData fsd in fsdQueue.GetConsumingEnumerable())
                {
                    fsdList.Add(fsd);
                    if (++counter == count)
                        break;
                }

                // process FSDs
                while (fsdList.Count > 0)
                {
                    FullSocketData fsd = fsdList[0];
                    fsdList.Remove(fsd);


                    for (int i = fsdList.Count - 1; i >= 0; i--)
                    {
                        FullSocketData _fsd = fsdList[i];
                        if (fsd.SameSocketMerge(_fsd))
                            fsdList.Remove(_fsd);
                    }

                    ParseData(FSDToEvent(fsd));
                }

                Thread.Sleep(5000);
            }
        }
        
        // modified code from: https://github.com/PcapDotNet/Pcap.Net/wiki/Pcap.Net-Tutorial-Interpreting-the-packets
        // used in the first part of packet sniffing
        public void SniffPackets(PacketDevice selectedDevice)
        {
            // Open the device
            using (PacketCommunicator communicator =
                selectedDevice.Open(65536,                                  // portion of the packet to capture
                                                                            // 65536 guarantees that the whole packet will be captured on all the link layers
                                    PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                    1000))                                  // read timeout
            {

                // Compile the filter
                using (BerkeleyPacketFilter filter = communicator.CreateFilter(""))
                    communicator.SetFilter(filter);

                // start the capture
                communicator.ReceivePackets(0, PacketHandler);
            }
        }

        // Callback function invoked by libpcap for every incoming packet
        // only adds the packets in the collection (they will be processed when possible)
        private void PacketHandler(Packet packet)
        {
            packets.Add(packet);
        }

        // starts the sniffing
        // and launches the 4 threads: SniffPackets, CheckCountries, GetNetStatPorts and FSDManager
        protected override void InitFetcher()
        {
            // Retrieve the device list from the local machine
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;

            if (allDevices.Count == 0)
                return;
            
            // network interface
            PacketDevice selectedDevice = allDevices[interface_no];

            foreach (DeviceAddress a in selectedDevice.Addresses)
            {
                string[] spl = a.ToString().Split(' ');
                if (spl[1] == "Internet6")
                    deviceIPv6 = spl[2];
                if (spl[1] == "Internet")
                    deviceIPv4 = spl[2];
            }

            // sniffing thread
            sniffingThread = new Thread(() => SniffPackets(selectedDevice));
            sniffingThread.Start();

            // country checking thread
            countriesThread = new Thread(() => CheckCountries());
            countriesThread.Start();
            // process checking thread
            netstatThread = new Thread(() => GetNetStatPorts());
            netstatThread.Start();
            // fsd manager thread
            fsdManagerThread = new Thread(() => FSDManager());
            fsdManagerThread.Start();
        }

        // parses the packets from the temporary queue and assigns them to relevant queues
        // giving the working threads work to do
        // occasionally it also creates events      eg. DNS queries
        protected override void RunFetcher()
        {
            while (true)
            {
                // if empty, wait
                if (packets.Count == 0)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                // take packet
                Packet packet = packets.Take();
                IpV4Datagram ip = packet.Ethernet.IpV4;

                // skip if not tcp or udp
                if (ip.Protocol != IpV4Protocol.Udp && ip.Protocol != IpV4Protocol.Tcp)
                    continue;

                // ports
                int serverPort = -1;
                int localPort = -1;
                bool sending = false;

                string server = "";
                string transport_protocol = "";
                string protocol = "";

                // treat UDP
                if (ip.Protocol == IpV4Protocol.Udp)
                {
                    UdpDatagram udp = ip.Udp;

                    // set transport_protocol
                    transport_protocol += "UDP";

                    // check if sending or receiving and set ports
                    if (ip.Source.ToString() == deviceIPv4 || ip.Source.ToString() == deviceIPv6)
                    {
                        ips_to_be_checked.Add(ip.Destination.ToString());
                        serverPort = udp.DestinationPort;
                        localPort = udp.SourcePort;
                        sending = true;
                    }
                    if (ip.Destination.ToString() == deviceIPv4 || ip.Destination.ToString() == deviceIPv6)
                    {
                        ips_to_be_checked.Add(ip.Source.ToString());
                        serverPort = udp.SourcePort;
                        localPort = udp.DestinationPort;
                    }

                    // treat well known port
                    if (serverPort != -1 && wellKnownPorts.ContainsKey(serverPort))
                    {
                        // check if DNS
                        if (udp.DestinationPort == 53 && udp.Dns.IsQuery && udp.Dns.QueryCount == 1)
                        {
                            DnsDatagram dns = ip.Udp.Dns;
                            ParseData(new SEvent(41, DateTime.Now, "DNS query", "network", EventType.SINGLE, new string[] { "domain" }, new string[] { dns.Queries[0].DomainName.ToString() }));
                        }

                        // set well known protocol
                        protocol += wellKnownPorts[serverPort];
                    }
                }

                // treat TCP
                if (ip.Protocol == IpV4Protocol.Tcp)
                {
                    TcpDatagram tcp = ip.Tcp;

                    // set transport_protocol
                    transport_protocol += "TCP";

                    if (ip.Source.ToString() == deviceIPv4 || ip.Source.ToString() == deviceIPv6)
                    {
                        ips_to_be_checked.Add(ip.Destination.ToString());
                        serverPort = tcp.DestinationPort;
                        localPort = tcp.SourcePort;
                        sending = true;
                    }
                    if (ip.Destination.ToString() == deviceIPv4 || ip.Destination.ToString() == deviceIPv6)
                    {
                        ips_to_be_checked.Add(ip.Source.ToString());
                        serverPort = tcp.SourcePort;
                        localPort = tcp.DestinationPort;
                    }

                    // treat well known port
                    if (serverPort != -1 && wellKnownPorts.ContainsKey(serverPort))
                        protocol += wellKnownPorts[serverPort];
                }

                // process info
                ProcessData pd;
                if (used_ports.TryGetValue(localPort, out ProcessData _pd))
                    pd = new ProcessData()
                    {
                        name = _pd.name,
                        pid = _pd.pid
                    };
                else
                    pd = new ProcessData()
                    {
                        name = "null",
                        pid = -1
                    };
                
                // set server
                if (sending)
                    server += ip.Destination;
                else
                    server += ip.Source;

                FullSocketData fsd;
                if (pd.pid != -1)
                {
                    if(sending)
                        fsd = new FullSocketData(pd.pid, pd.name, localPort, server, serverPort, transport_protocol, protocol, packet.Length, 0, 1);
                    else
                        fsd = new FullSocketData(pd.pid, pd.name, localPort, server, serverPort, transport_protocol, protocol, 0, packet.Length, 1);
                }
                else
                {
                    if(sending)
                        fsd = new FullSocketData(-1, "", localPort, server, serverPort, transport_protocol, protocol, packet.Length, 0, 1);
                    else
                        fsd = new FullSocketData(-1, "", localPort, server, serverPort, transport_protocol, protocol, 0, packet.Length, 1);
                }

                fsdQueue.TryAdd(fsd);
            }
        }

        // adds the event to the shared queue
        protected override void ParseData(Event e)
        {
            events.TryAdd(e);
        }

        // stopfetching override which also stops the other working threads
        public override bool StopFetching()
        {
            try
            {
                fetchingThread.Abort();
                sniffingThread.Abort();
                countriesThread.Abort();
                fsdManagerThread.Abort();
                netstatThread.Abort();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
