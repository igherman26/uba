using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace UBA
{
    public enum MonitorizationOptions {ALL, EVENTS_ONLY, PERFORMANCE_ONLY, NETWORK_ONLY, EVENTS_AND_PERFORMANCE, EVENTS_AND_NETWORK, PERFORMANCE_AND_NETWORK};

    public class Manager
    {
        private BlockingCollection<Event> events = new BlockingCollection<Event>();
        private BlockingCollection<Alert> alerts = new BlockingCollection<Alert>();
        public List<Alert> alerts_list = new List<Alert>();

        private BlockingCollection<Event> performanceDataQueue = new BlockingCollection<Event>();
        private BlockingCollection<Event> basicDataQueue = new BlockingCollection<Event>();
        private BlockingCollection<Event> directDataQueue = new BlockingCollection<Event>();


        private MonitorizationOptions opt;
        public int interfaceNo = 0;
        public bool defaultFetchersInit = true;

        public string username;
        public UserProfile currentUser;
        public BasicData basicData;
        public PerformanceData baselineData;
        public DateTime performanceDataDate;
        public PerformanceData performanceData;

        EventsDataFetcher edf;
        PerformanceDataFetcher pdf;
        NetworkDataFetcher ndf;

        Thread fetchingThread;
        Thread savingThread;
        Thread performanceDataThread;
        bool performanceDataThreadPaused = false;
        Thread basicDataThread;
        bool basicDataThreadPaused = false;
        Thread directDataThread;
        bool directDataThreadPaused = false;
        Thread alertGeneratingThread;
        bool alertGeneratingThreadPaused = false;

        bool pauseUpdatingThreads = false;

        public Manager(string username, MonitorizationOptions opt, int interfaceNo = 0, bool defaultFetchersInit = true)
        {
            this.opt = opt;
            this.interfaceNo = interfaceNo;
            this.defaultFetchersInit = defaultFetchersInit;

            this.username = username;
            currentUser = null;
            basicData = null;
            baselineData = null;
            performanceData = null;
            performanceDataDate = DateTime.Now;

            edf = null;
            pdf = null;
            ndf = null;

            performanceDataThread = null;
            basicDataThread = null;
            directDataThread = null;
            alertGeneratingThread = null;
        }

        public bool InitManager()
        {
            // load user; create if it doesn't exist
            currentUser = UserProfile.LoadProfile(username);
            if (currentUser == null && UserProfile.CreateNewProfile(username))
                currentUser = UserProfile.LoadProfile(username);
            if (currentUser == null)
                return false;
            // load baseline data; create if it doesn't exist
            if (!LoadBaselineData())
            {
                baselineData = new PerformanceData();

                // clear the dates so that the baseline will be complete
                currentUser.baselineDates.Clear();

                // 2 or more dates should be available for baseline generation
                if (currentUser.availableDates.Count > 1)
                    GenerateBaselineData();

                if (!SaveBaselineData())
                    return false;
            }
            // load basic data; create if it doesn't exist
            if (!LoadBasicData())
            {
                basicData = new BasicData();

                if (!SaveBasicData())
                    return false;
            }
            // load performance data; create if it doesn't exist
            if (!LoadPerformanceData(DateTime.Now))
            {
                performanceData = new PerformanceData();

                if (!SavePerformanceData(DateTime.Now))
                    return false;
            }
            // load alerts; create json file if it doesn't exist
            if (!LoadAlerts(DateTime.Now))
            {
                if (!SaveAlerts(DateTime.Now))
                    return false;
            }

            performanceDataDate = DateTime.Now;

            // start data fetcher and interpreters and the alert generator
            fetchingThread = new Thread(() => FetchingThreadMethod());
            fetchingThread.Start();
            savingThread = new Thread(() => SavingThreadMethod());
            savingThread.Start();
            performanceDataThread = new Thread(() => PerformanceDataThreadMethod());
            performanceDataThread.Start();
            basicDataThread = new Thread(() => BasicDataThreadMethod());
            basicDataThread.Start();
            directDataThread = new Thread(() => DirectDataThreadMethod());
            directDataThread.Start();
            alertGeneratingThread = new Thread(() => AlertGeneratingThreadMethod());
            alertGeneratingThread.Start();

            // start data fetchers
            switch (opt)
            {
                case MonitorizationOptions.ALL:
                    {
                        edf = new EventsDataFetcher(events);
                        pdf = new PerformanceDataFetcher(events);
                        ndf = new NetworkDataFetcher(events, interfaceNo);

                        if (defaultFetchersInit)
                        {
                            edf.DefaultInit();
                            pdf.DefaultInit();
                        }
                        edf.StartFetching();
                        pdf.StartFetching();
                        ndf.StartFetching();
                        break;
                    }
                case MonitorizationOptions.EVENTS_AND_NETWORK:
                    {
                        edf = new EventsDataFetcher(events);
                        ndf = new NetworkDataFetcher(events, interfaceNo);

                        if (defaultFetchersInit)
                            edf.DefaultInit();
                        edf.StartFetching();
                        ndf.StartFetching();
                        break;
                    }
                case MonitorizationOptions.EVENTS_AND_PERFORMANCE:
                    {
                        edf = new EventsDataFetcher(events);
                        pdf = new PerformanceDataFetcher(events);

                        if (defaultFetchersInit)
                        {
                            edf.DefaultInit();
                            pdf.DefaultInit();
                        }
                        edf.StartFetching();
                        pdf.StartFetching();
                        break;
                    }
                case MonitorizationOptions.PERFORMANCE_AND_NETWORK:
                    {
                        pdf = new PerformanceDataFetcher(events);
                        ndf = new NetworkDataFetcher(events, interfaceNo);

                        if (defaultFetchersInit)
                            pdf.DefaultInit();
                        pdf.StartFetching();
                        ndf.StartFetching();
                        break;
                    }
                case MonitorizationOptions.EVENTS_ONLY:
                    {
                        edf = new EventsDataFetcher(events);

                        if (defaultFetchersInit)
                            edf.DefaultInit();
                        edf.StartFetching();
                        break;
                    }
                case MonitorizationOptions.NETWORK_ONLY:
                    {
                        ndf = new NetworkDataFetcher(events, interfaceNo);

                        ndf.StartFetching();
                        break;
                    }
                case MonitorizationOptions.PERFORMANCE_ONLY:
                    {
                        pdf = new PerformanceDataFetcher(events);

                        if (defaultFetchersInit)
                            pdf.DefaultInit();
                        pdf.StartFetching();
                        break;
                    }
            }

            return true;
        }

        
        private void GenerateBaselineData()
        {
            MessageBox.Show("The baseline will be generated.\nPress OK to continue.");

            PerformanceData _baseline = new PerformanceData();

            // old baseline values count
            int oldBaselineCount = currentUser.baselineDates.Count;

            List<PerformanceData> _pds = new List<PerformanceData>();
            // find missing days
            foreach (string s in currentUser.availableDates)
            {
                // don't add the day of today in the baseline
                if (s == DateTime.Now.ToString("yyyyMMdd"))
                    continue;

                PerformanceData _pd;

                if(!currentUser.baselineDates.Contains(s))
                {
                    _pd = GetPerformanceData(s);
                    _pds.Add(_pd);
                    currentUser.baselineDates.Add(s);
                }
            }

            // calculate how much the new values will matter in the total baseline
            // new baseline values count
            int newBaselineCount = currentUser.baselineDates.Count;
            float new_percentage = newBaselineCount / ((float)(newBaselineCount + oldBaselineCount));

            // add values to temporary baseline
            foreach (PerformanceData pd in _pds)
            {
                // 5 min global bins
                for (int i = 0; i < 288; i++)
                {
                    _baseline.globalNetworkSend[i] += pd.globalNetworkSend[i];
                    _baseline.globalNetworkReceive[i] += pd.globalNetworkReceive[i];
                    _baseline.globalDiskUsage[i] += pd.globalDiskUsage[i];
                }

                // 5 min process bins
                // per process network send
                foreach (KeyValuePair<string, long[]> pair in pd.processesNetworkSend)
                {
                    // if the baseline doesn't contain the process data, add it
                    if (!_baseline.processesNetworkSend.ContainsKey(pair.Key))
                        _baseline.processesNetworkSend.Add(pair.Key, pair.Value);
                    else
                    {
                        for (int j = 0; j < pair.Value.Length; j++)
                        {
                            _baseline.processesNetworkSend[pair.Key][j] += pd.processesNetworkSend[pair.Key][j];
                        }
                    }
                }
                // per process network receive
                foreach (KeyValuePair<string, long[]> pair in pd.processesNetworkReceive)
                {
                    // if the baseline doesn't contain the process data, add it
                    if (!_baseline.processesNetworkReceive.ContainsKey(pair.Key))
                        _baseline.processesNetworkReceive.Add(pair.Key, pair.Value);
                    else
                    {
                        for (int j = 0; j < pair.Value.Length; j++)
                        {
                            _baseline.processesNetworkReceive[pair.Key][j] += pd.processesNetworkReceive[pair.Key][j];
                        }
                    }
                }

                // 0.5 min global bins
                for (int i = 0; i < 2880; i++)
                {
                    _baseline.globalCPUusage[i] += pd.globalCPUusage[i];
                    _baseline.globalCPUusageValuesNo[i] += pd.globalCPUusageValuesNo[i];
                    _baseline.globalGPUusage[i] += pd.globalGPUusage[i];
                    _baseline.globalGPUusageValuesNo[i] += pd.globalGPUusageValuesNo[i];
                    _baseline.globalMemoryUsage[i] += pd.globalMemoryUsage[i];
                    _baseline.globalMemoryUsageValuesNo[i] += pd.globalMemoryUsageValuesNo[i];
                }

                // 0.5 min process bins
                // per process cpu usage
                foreach (KeyValuePair<string, long[]> pair in pd.processesCPUusage)
                {
                    // if the baseline doesn't contain the process data, add it
                    if (!_baseline.processesCPUusage.ContainsKey(pair.Key))
                        _baseline.processesCPUusage.Add(pair.Key, pair.Value);
                    else
                    {
                        for (int j = 0; j < pair.Value.Length; j++)
                        {
                            _baseline.processesCPUusage[pair.Key][j] += pd.processesCPUusage[pair.Key][j];
                        }
                    }
                }

                // per process cpu usage
                foreach (KeyValuePair<string, long[]> pair in pd.processesCPUusageValuesNo)
                {
                    // if the baseline doesn't contain the process data, add it
                    if (!_baseline.processesCPUusageValuesNo.ContainsKey(pair.Key))
                        _baseline.processesCPUusageValuesNo.Add(pair.Key, pair.Value);
                    else
                    {
                        for (int j = 0; j < pair.Value.Length; j++)
                        {
                            _baseline.processesCPUusageValuesNo[pair.Key][j] += pd.processesCPUusageValuesNo[pair.Key][j];
                        }
                    }
                }

                // 0.5h bins
                for (int i = 0; i < 48; i++)
                {
                    _baseline.packets_counter[i] = pd.packets_counter[i];
                }
            }

            // add temporary values to the baseline file
            ////////////////////////////////////////////
            // 5 min bins
            for (int i = 0; i < 288; i++)
            {
                baselineData.globalNetworkSend[i] = Convert.ToInt64(Math.Round(baselineData.globalNetworkSend[i] * (1 - new_percentage) + _baseline.globalNetworkSend[i] * new_percentage));
                baselineData.globalNetworkReceive[i] = Convert.ToInt64(Math.Round(baselineData.globalNetworkReceive[i] * (1 - new_percentage) + _baseline.globalNetworkReceive[i] * new_percentage));
                baselineData.globalDiskUsage[i] = Convert.ToInt64(Math.Round(baselineData.globalDiskUsage[i] * (1 - new_percentage) + _baseline.globalDiskUsage[i] * new_percentage));
            }

            // per process network send
            foreach (KeyValuePair<string, long[]> pair in _baseline.processesNetworkSend)
            {
                // if the baseline doesn't contain the process data, add it
                if (!baselineData.processesNetworkSend.ContainsKey(pair.Key))
                    baselineData.processesNetworkSend.Add(pair.Key, pair.Value);
                else
                {
                    for (int j = 0; j < pair.Value.Length; j++)
                    {
                        baselineData.processesNetworkSend[pair.Key][j] = Convert.ToInt64(Math.Round(baselineData.processesNetworkSend[pair.Key][j] * (1 - new_percentage) + _baseline.processesNetworkSend[pair.Key][j] * new_percentage));
                    }
                }
            }
            // per process network receive
            foreach (KeyValuePair<string, long[]> pair in _baseline.processesNetworkReceive)
            {
                // if the baseline doesn't contain the process data, add it
                if (!baselineData.processesNetworkReceive.ContainsKey(pair.Key))
                    baselineData.processesNetworkReceive.Add(pair.Key, pair.Value);
                else
                {
                    for (int j = 0; j < pair.Value.Length; j++)
                    {
                        baselineData.processesNetworkReceive[pair.Key][j] = Convert.ToInt64(Math.Round(baselineData.processesNetworkReceive[pair.Key][j] * (1 - new_percentage) + _baseline.processesNetworkReceive[pair.Key][j] * new_percentage));
                    }
                }
            }

            // 0.5 min bins
            for (int i = 0; i < 2880; i++)
            {
                baselineData.globalCPUusage[i] = Convert.ToInt64(Math.Round(baselineData.globalCPUusage[i] * (1 - new_percentage) + _baseline.globalCPUusage[i] * new_percentage));
                baselineData.globalCPUusageValuesNo[i] = Convert.ToInt64(Math.Round(baselineData.globalCPUusageValuesNo[i] * (1 - new_percentage) + _baseline.globalCPUusageValuesNo[i] * new_percentage));
                baselineData.globalGPUusage[i] = Convert.ToInt64(Math.Round(baselineData.globalGPUusage[i] * (1 - new_percentage) + _baseline.globalGPUusage[i] * new_percentage));
                baselineData.globalGPUusageValuesNo[i] = Convert.ToInt64(Math.Round(baselineData.globalGPUusageValuesNo[i] * (1 - new_percentage) + _baseline.globalGPUusageValuesNo[i] * new_percentage));
                baselineData.globalMemoryUsage[i] = Convert.ToInt64(Math.Round(baselineData.globalMemoryUsage[i] * (1 - new_percentage) + _baseline.globalMemoryUsage[i] * new_percentage));
                baselineData.globalMemoryUsageValuesNo[i] = Convert.ToInt64(Math.Round(baselineData.globalMemoryUsageValuesNo[i] * (1 - new_percentage) + _baseline.globalMemoryUsageValuesNo[i] * new_percentage));
            }

            // per process cpu usage
            foreach (KeyValuePair<string, long[]> pair in _baseline.processesCPUusage)
            {
                // if the baseline doesn't contain the process data, add it
                if (!baselineData.processesCPUusage.ContainsKey(pair.Key))
                    baselineData.processesCPUusage.Add(pair.Key, pair.Value);
                else
                {
                    for (int j = 0; j < pair.Value.Length; j++)
                    {
                        baselineData.processesCPUusage[pair.Key][j] = Convert.ToInt64(Math.Round(baselineData.processesCPUusage[pair.Key][j] * (1 - new_percentage) + _baseline.processesCPUusage[pair.Key][j] * new_percentage));
                    }
                }
            }
            // per process cpu usage
            foreach (KeyValuePair<string, long[]> pair in _baseline.processesCPUusageValuesNo)
            {
                // if the baseline doesn't contain the process data, add it
                if (!baselineData.processesCPUusageValuesNo.ContainsKey(pair.Key))
                    baselineData.processesCPUusageValuesNo.Add(pair.Key, pair.Value);
                else
                {
                    for (int j = 0; j < pair.Value.Length; j++)
                    {
                        baselineData.processesCPUusageValuesNo[pair.Key][j] = Convert.ToInt64(Math.Round(baselineData.processesCPUusageValuesNo[pair.Key][j] * (1 - new_percentage) + _baseline.processesCPUusageValuesNo[pair.Key][j] * new_percentage));
                    }
                }
            }

            // 0.5h bins
            for (int i = 0; i < 48; i++)
            {
                baselineData.packets_counter[i] = Convert.ToInt64(Math.Round(baselineData.packets_counter[i] * (1 - new_percentage) + _baseline.packets_counter[i] * new_percentage));
            }
        }

        // fetches the events from the buffer and moves them further in the other buffers
        private void FetchingThreadMethod()
        {
            int[] directIDs = { 3, 4, 5, 7, 30, 31, 32, 33, 34, 35 };
            int[] basicIDs = { 1, 2, 6, 8, 9, 10, 11, 20, 21, 22, 24, 41, 42 };
            int[] performanceIDs = { 40, 51, 52, 53, 55, 56, 57, 59, 60, 61 };

            while (true)
            {
                // if empty, wait
                if (events.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                // take event
                Event e = events.Take();

                // parse event
                // send to direct data queue
                if (directIDs.Contains(e.id))
                    directDataQueue.TryAdd(e);
                // send to basic data queue
                if (basicIDs.Contains(e.id))
                    basicDataQueue.TryAdd(e);
                // send to performance data queue
                if (performanceIDs.Contains(e.id))
                    performanceDataQueue.TryAdd(e);
            }
        }

        // saves data once per minute and also checks for day change
        private void SavingThreadMethod()
        {
            int counter = 0;
            while (true)
            {
                // check if the day changed and swap the performance data object
                if (performanceDataDate.Date != DateTime.Now.Date)
                {
                    SavePerformanceData(DateTime.Now);
                    performanceDataDate = DateTime.Now;
                }

                // check if a new baseline needs to be added
                if (currentUser.availableDates.Count - 1 > currentUser.baselineDates.Count)
                    GenerateBaselineData();

                // move alerts from blocking queue to non-blocking list
                // this thread being the only writer
                // if empty, wait
                int alerts_count = alerts.Count;
                if (alerts_count != 0)
                {
                    // take alerts
                    for (int i = 0; i < alerts_count; i++)
                    {
                        Alert a = alerts.Take();
                        if (a != null)
                            alerts_list.Add(a);
                    }
                }

                counter++;

                // atleast 10 seconds have passed
                // pause threads and save progress
                if (counter == 10)
                {
                    pauseUpdatingThreads = true;
                    while (!performanceDataThreadPaused || !basicDataThreadPaused
                        || !directDataThreadPaused || !alertGeneratingThreadPaused)
                        Thread.Sleep(100);

                    // now that the threads are paused save the data
                    currentUser.SaveProfile();
                    SaveAlerts(DateTime.Now);
                    SaveBasicData();
                    SaveBaselineData();
                    SavePerformanceData(DateTime.Now);

                    // let the threads start again
                    pauseUpdatingThreads = false;

                    // reset counter
                    counter = 0;
                }

                // wait 1 second
                Thread.Sleep(1000);
            }
        }

        // updates the performance data information based on received events
        private void PerformanceDataThreadMethod()
        {
            while (true)
            {
                // don't update if the day is not properly set
                while (performanceDataDate.Date != DateTime.Now.Date)
                    Thread.Sleep(5000);

                // check if the thread needs to be paused
                while (pauseUpdatingThreads)
                {
                    // set paused
                    if (!performanceDataThreadPaused)
                        performanceDataThreadPaused = true;

                    Thread.Sleep(1000);
                }

                // reset paused
                if (performanceDataThreadPaused)
                    performanceDataThreadPaused = false;


                // if empty, wait
                if (performanceDataQueue.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                // take event
                Event e = performanceDataQueue.Take();


                switch (e.id)
                {
                    // local Network usage
                    // will update protocols, packet_counter, sent and received bytes
                    case 40:
                        {
                            SEvent se = (SEvent)e;

                            string pname = se.values[0];
                            string protocol = se.values[6];
                            long sent = Int64.Parse(se.values[7]);
                            long received = Int64.Parse(se.values[8]);
                            long packet_counter = Int64.Parse(se.values[9]);

                            // update protocols
                            if (protocol != "")
                            {
                                if (!basicData.usedProtocols.ContainsKey(protocol))
                                    basicData.usedProtocols.Add(protocol, packet_counter);
                                else
                                    basicData.usedProtocols[protocol] += packet_counter;
                            }

                            // update packet_counter
                            if (e.timestamp.Minute >= 30)
                                performanceData.packets_counter[2 * e.timestamp.Hour + 1] += packet_counter;
                            else
                                performanceData.packets_counter[2 * e.timestamp.Hour] += packet_counter;

                            // update sent and received bytes
                            int position = Convert.ToInt32(Math.Ceiling((double)(e.timestamp.Hour * 60 + e.timestamp.Minute) / 5));
                            if (!performanceData.processesNetworkReceive.ContainsKey(pname))
                            {
                                long[] values = new long[288];
                                values[position] += received;
                                performanceData.processesNetworkReceive.Add(pname, values);
                            }
                            else
                            {
                                long[] values = performanceData.processesNetworkReceive[pname];
                                values[position] += received;
                            }

                            if (!performanceData.processesNetworkSend.ContainsKey(pname))
                            {
                                long[] values = new long[288];
                                values[position] += sent;
                                performanceData.processesNetworkSend.Add(pname, values);
                            }
                            else
                            {
                                long[] values = performanceData.processesNetworkSend[pname];
                                values[position] += sent;
                            }

                            break;
                        }
                    // global CPU usage
                    case 51:
                        {
                            MEvent me = (MEvent)e;
                            foreach (SEvent se in me.events)
                            {
                                if (se.instance == "_Total")
                                {
                                    int position = Convert.ToInt32(Math.Ceiling((double)((e.timestamp.Hour * 60 + e.timestamp.Minute)*2)));
                                    if (e.timestamp.Second > 30)
                                        position += 1;

                                    performanceData.globalCPUusage[position] += Int64.Parse(se.values[0]);
                                    performanceData.globalCPUusageValuesNo[position] += 1;
                                    break;
                                }
                            }
                            break;
                        }
                    // local CPU usage
                    case 52:
                        {
                            MEvent me = (MEvent)e;
                            foreach (SEvent se in me.events)
                            {
                                string _pname = se.instance;
                                // treat multi processes (with #s)
                                string pname;
                                if (_pname.Contains('#'))
                                    pname = _pname.Split('#')[0];
                                else
                                    pname = _pname;

                                long value = Int64.Parse(se.values[0]);

                                int position = Convert.ToInt32(Math.Ceiling((double)((e.timestamp.Hour * 60 + e.timestamp.Minute)*2)));
                                if (e.timestamp.Second > 30)
                                    position += 1;


                                if (!performanceData.processesCPUusage.ContainsKey(pname))
                                {
                                    long[] values = new long[2880];
                                    long[] counts = new long[2880];
                                    values[position] = value;
                                    counts[position] = 1;
                                    performanceData.processesCPUusage.Add(pname, values);
                                    performanceData.processesCPUusageValuesNo.Add(pname, counts);
                                }
                                else
                                {
                                    long[] values = performanceData.processesCPUusage[pname];
                                    long[] counts = performanceData.processesCPUusageValuesNo[pname];
                                    values[position] += value;
                                    counts[position]++;
                                }
                            }
                            break;
                        }
                    // global priviledged CPU usage
                    // won't be implemented in the first release
                    case 53:
                        break;
                    // global memory usage
                    case 55:
                        {
                            SEvent se = (SEvent)e;

                            int position = Convert.ToInt32(Math.Ceiling((double)((e.timestamp.Hour * 60 + e.timestamp.Minute)*2)));
                            if (e.timestamp.Second > 30)
                                position += 1;

                            performanceData.globalMemoryUsage[position] += Int64.Parse(se.values[0]);
                            performanceData.globalMemoryUsageValuesNo[position] += 1;
                            break;
                        }
                    // local memory usage
                    // won't be implemented in the first release
                    case 56:
                        break;
                    // global disk usage
                    case 57:
                        {
                            MEvent me = (MEvent)e;
                            foreach (SEvent se in me.events)
                            {
                                if (se.instance == "_Total")
                                {
                                    int position = Convert.ToInt32(Math.Ceiling((double)(e.timestamp.Hour * 60 + e.timestamp.Minute) / 5));

                                    performanceData.globalDiskUsage[position] += Int64.Parse(se.values[0]);
                                    break;
                                }
                            }
                            break;
                        }
                    // global gpu usage
                    case 59:
                        {
                            SEvent se = (SEvent)e;

                            int position = Convert.ToInt32(Math.Ceiling((double)((e.timestamp.Hour * 60 + e.timestamp.Minute)*2)));
                            if (e.timestamp.Second > 30)
                                position += 1;

                            performanceData.globalGPUusage[position] += Int64.Parse(se.values[0]);
                            performanceData.globalGPUusageValuesNo[position] += 1;
                            break;
                        }
                    // global network sent
                    case 60:
                        {
                            MEvent me = (MEvent)e;
                            int position = Convert.ToInt32(Math.Ceiling((double)(e.timestamp.Hour * 60 + e.timestamp.Minute) / 5));
                            foreach (SEvent se in me.events)
                                performanceData.globalNetworkSend[position] += Int64.Parse(se.values[0]);
                            break;
                        }
                    // global newtork received
                    case 61:
                        {
                            MEvent me = (MEvent)e;
                            int position = Convert.ToInt32(Math.Ceiling((double)(e.timestamp.Hour * 60 + e.timestamp.Minute) / 5));
                            foreach (SEvent se in me.events)
                                performanceData.globalNetworkReceive[position] += Int64.Parse(se.values[0]);
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        // updates the basic data information based on received events
        private void BasicDataThreadMethod()
        {
            while (true)
            {
                // check if the thread needs to be paused
                while (pauseUpdatingThreads)
                {
                    // set paused
                    if (!basicDataThreadPaused)
                        basicDataThreadPaused = true;

                    Thread.Sleep(1000);
                }

                // reset paused
                if (basicDataThreadPaused)
                    basicDataThreadPaused = false;

                // if empty, wait
                if (basicDataQueue.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                // take event
                Event e = basicDataQueue.Take();

                switch (e.id)
                {
                    // system startup
                    case 1:
                        {
                            // increment the value at the hour
                            basicData.startups[e.timestamp.Hour]++;
                            break;
                        }
                    // system shutdown - windows doesn't actually use this one...
                    case 2:
                        break;
                    // account logon
                    // won't be implemented in the first release
                    case 6:
                        break;
                    // account logout
                    // won't be implemented in the first release
                    case 8:
                        break;
                    // special privileges assigned to logon
                    // won't be implemented in the first release
                    case 9:
                        break;
                    // workstation was locked
                    // won't be implemented in the first release
                    case 10:
                        break;
                    // workstation was unlocked
                    // won't be implemented in the first release
                    case 11:
                        break;
                    // process creation with full elevation
                    case 20:
                        {
                            // check if the process is part of the elevated processes
                            // if not, add it and generate an alert (MED if the processes is known, HIGH otherwise)
                            SEvent se = (SEvent)e;

                            if (!basicData.knownElevatedProcessesList.Contains(se.values[1]))
                            {
                                basicData.knownElevatedProcessesList.Add(se.values[1]);

                                Alert a;
                                if (basicData.knownProcessesList.Contains(se.values[1]))
                                    a = new Alert(ALERT_TYPE.MED, String.Format("new elevated process: {0}", se.values[1]), e.timestamp, e);
                                else
                                {
                                    basicData.knownProcessesList.Add(se.values[1]);
                                    a = new Alert(ALERT_TYPE.HIGH, String.Format("new elevated process: {0}", se.values[1]), e.timestamp, e);
                                }

                                alerts.TryAdd(a);
                            }

                            break;
                        }
                    // process creation with default elevation
                    case 21:
                        {
                            // check if the process is part of the known processes
                            // if not, add it and generate an alert (LOW)
                            SEvent se = (SEvent)e;

                            if (!basicData.knownProcessesList.Contains(se.values[1]))
                            {
                                basicData.knownProcessesList.Add(se.values[1]);

                                Alert a = new Alert(ALERT_TYPE.LOW, String.Format("new process: {0}", se.values[1]), e.timestamp, e);

                                alerts.TryAdd(a);
                            }

                            break;
                        }
                    // process creation with limited elevation
                    case 22:
                        {
                            // check if the process is part of the known processes
                            // if not, add it and generate an alert (LOW)
                            SEvent se = (SEvent)e;

                            if (!basicData.knownProcessesList.Contains(se.values[1]))
                            {
                                basicData.knownProcessesList.Add(se.values[1]);

                                Alert a = new Alert(ALERT_TYPE.LOW, String.Format("new process: {0}", se.values[1]), e.timestamp, e);

                                alerts.TryAdd(a);
                            }

                            break;
                        }
                    // DNS query
                    case 41:
                        {
                            // check if the domain is part of the known domains and increment its values if it exists
                            // if not, add it and generate an alert (LOW)
                            SEvent se = (SEvent)e;

                            if (se.values[0] == null)
                                break;

                            string domain = se.values[0].Remove(se.values[0].Length - 1);

                            if (!basicData.domainConnections.ContainsKey(domain))
                            {
                                basicData.domainConnections.Add(domain, 1);

                                Alert a = new Alert(ALERT_TYPE.LOW, String.Format("new domain: {0}", domain), e.timestamp, e);

                                alerts.TryAdd(a);
                            }
                            else
                                basicData.domainConnections[domain]++;

                            break;
                        }
                    // communication with country X
                    case 42:
                        {
                            // check if the domain is part of the known domains and increment its values if it exists
                            // if not, add it and generate an alert (LOW)
                            SEvent se = (SEvent)e;

                            if (se.values[1] == null)
                                break;

                            if (!basicData.countryConnections.ContainsKey(se.values[1]))
                            {
                                basicData.countryConnections.Add(se.values[1], 1);

                                Alert a = new Alert(ALERT_TYPE.MED, String.Format("new country communication: {0}", se.values[1]), e.timestamp, e);

                                alerts.TryAdd(a);
                            }
                            else
                                basicData.countryConnections[se.values[1]]++;

                            break;
                        }
                    default:
                        break;
                }
            }
        }

        // wraps events into Alert objects and adds them to alerts
        private void DirectDataThreadMethod()
        {
            // alerts levels - could be dynamically loaded..
            Dictionary<int, ALERT_TYPE> alert_types = new Dictionary<int, ALERT_TYPE>
            {
                { 3 , ALERT_TYPE.HIGH },
                { 4 , ALERT_TYPE.HIGH },
                { 5 , ALERT_TYPE.HIGH },
                { 7 , ALERT_TYPE.HIGH },
                {30 , ALERT_TYPE.HIGH },
                {31 , ALERT_TYPE.HIGH },
                {32 , ALERT_TYPE.HIGH },
                {33 , ALERT_TYPE.MED  },
                {34 , ALERT_TYPE.MED  },
                {35 , ALERT_TYPE.LOW  }
            };

            while (true)
            {
                // check if the thread needs to be paused
                while (pauseUpdatingThreads)
                {
                    // set paused
                    if (!directDataThreadPaused)
                        directDataThreadPaused = true;

                    Thread.Sleep(1000);
                }

                // reset paused
                if (directDataThreadPaused)
                    directDataThreadPaused = false;

                // if empty, wait
                if (directDataQueue.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }

                // take event
                Event e = directDataQueue.Take();

                // wrap event and add it to alerts
                Alert a = new Alert(alert_types[e.id], e.description, e.timestamp, e);
                alerts.TryAdd(a);
            }
        }

        //TODO
        public void AlertGeneratingThreadMethod()
        {
            while (true)
            {
                // check if the thread needs to be paused
                while (pauseUpdatingThreads)
                {
                    // set paused
                    if (!alertGeneratingThreadPaused)
                        alertGeneratingThreadPaused = true;

                    Thread.Sleep(1000);
                }
                // reset paused
                if (alertGeneratingThreadPaused)
                    alertGeneratingThreadPaused = false;

                //TODO
                Thread.Sleep(5000);
            }
        }

        // aborts all working threads
        public bool StopThreads()
        {
            try
            {
                fetchingThread.Abort();
                performanceDataThread.Abort();
                basicDataThread.Abort();
                directDataThread.Abort();
                alertGeneratingThread.Abort();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // create/update user's basic data
        public bool SaveBasicData()
        {
            try
            {
                File.WriteAllText(currentUser.basicDataLocation, JsonConvert.SerializeObject(basicData));
                return true;
            }
            catch
            {
                return false;
            }
        }
        // load current user's basic data
        public bool LoadBasicData()
        {
            try
            {
                using (StreamReader r = new StreamReader(currentUser.basicDataLocation))
                {
                    string json = r.ReadToEnd();
                    BasicData bd = JsonConvert.DeserializeObject<BasicData>(json);
                    basicData = bd;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // save/update user's baseline data
        private bool SaveBaselineData()
        {
            try
            {
                File.WriteAllText(currentUser.baselineDataLocation, JsonConvert.SerializeObject(this.baselineData));
                return true;
            }
            catch
            {
                return false;
            }
        }
        // load current user's baseline data
        private bool LoadBaselineData()
        {
            try
            {
                using (StreamReader r = new StreamReader(currentUser.baselineDataLocation))
                {
                    string json = r.ReadToEnd();
                    PerformanceData pd = JsonConvert.DeserializeObject<PerformanceData>(json);
                    baselineData = pd;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // save/update user's performance data
        private bool SavePerformanceData(DateTime date)
        {
            if (date == null)
                date = DateTime.Now;

            string fileName = date.ToString("yyyyMMdd") + ".json";
            try
            {
                File.WriteAllText(Path.Combine(currentUser.profileLocation, fileName), JsonConvert.SerializeObject(this.performanceData));
                if (!currentUser.availableDates.Contains(date.ToString("yyyyMMdd")))
                    currentUser.availableDates.Add(date.ToString("yyyyMMdd"));
                return true;
            }
            catch
            {
                return false;
            }
        }
        // load user's performance data
        private bool LoadPerformanceData(DateTime date)
        {

            if (date == null)
                date = DateTime.Now;
            string fileName = date.ToString("yyyyMMdd") + ".json";

            if (!currentUser.availableDates.Contains(date.ToString("yyyyMMdd")))
                return false;

            try
            {
                using (StreamReader r = new StreamReader(Path.Combine(currentUser.profileLocation, fileName)))
                {
                    string json = r.ReadToEnd();
                    PerformanceData pd = JsonConvert.DeserializeObject<PerformanceData>(json);
                    performanceData = pd;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // used to get performance data from other dates
        public PerformanceData GetPerformanceData(string date)
        {
            string fileName = date + ".json";

            if (!currentUser.availableDates.Contains(date))
                return null;

            try
            {
                PerformanceData pd;
                using (StreamReader r = new StreamReader(Path.Combine(currentUser.profileLocation, fileName)))
                {
                    string json = r.ReadToEnd();
                    pd = JsonConvert.DeserializeObject<PerformanceData>(json);
                }
                return pd;
            }
            catch
            {
                return null;
            }
        }


        // save alerts
        private bool SaveAlerts(DateTime date)
        {
            if (date == null)
                date = DateTime.Now;

            string fileName = date.ToString("yyyyMMdd") + "_alerts.json";
            try
            {
                File.WriteAllText(Path.Combine(currentUser.profileLocation, fileName), JsonConvert.SerializeObject(this.alerts_list));
                return true;
            }
            catch
            {
                return false;
            }
        }
        // returns the alerts list from the date
        private bool LoadAlerts(DateTime date)
        {

            if (date == null)
                date = DateTime.Now;
            string fileName = date.ToString("yyyyMMdd") + "_alerts.json";

            if (!currentUser.availableDates.Contains(date.ToString("yyyyMMdd_alerts")))
                return false;

            try
            {
                using (StreamReader r = new StreamReader(Path.Combine(currentUser.profileLocation, fileName)))
                {
                    string json = r.ReadToEnd();
                    alerts_list = JsonConvert.DeserializeObject<List<Alert>>(json);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        // save events
        private bool SaveEvents(DateTime date)
        {
            if (date == null)
                date = DateTime.Now;

            string fileName = date.ToString("yyyyMMdd_events") + ".json";
            try
            {
                File.WriteAllText(Path.Combine(currentUser.profileLocation, fileName), JsonConvert.SerializeObject(events.ToList()));
                return true;
            }
            catch
            {
                return false;
            }
        }
        // returns the events list from the date
        private List<Event> LoadEvents(DateTime date)
        {

            if (date == null)
                date = DateTime.Now;
            string fileName = date.ToString("yyyyMMdd_events") + ".json";

            if (!currentUser.availableDates.Contains(date.ToString("yyyyMMdd_events")))
                return null;

            try
            {
                using (StreamReader r = new StreamReader(Path.Combine(currentUser.profileLocation, fileName)))
                {
                    string json = r.ReadToEnd();
                    List<Event> le = JsonConvert.DeserializeObject<List<Event>>(json);
                    return le;
                }
            }
            catch
            {
                return null;
            }
        }

        // return a list with all events
        private List<Event> GetAllEvents()
        {
            return events.ToList();
        }

        public int GetEventsSamplingTime()
        {
            if (edf != null)
                return edf.samplingTime;
            else
                return -1;
        }

        public void SetEventsSamplingTime(int samplingTime)
        {
            edf.samplingTime = samplingTime;
        }

        public int GetPerformanceSamplingTime()
        {
            if (pdf != null)
                return pdf.samplingTime;
            else
                return -1;
        }

        public void SetPerformanceSamplingTime(int samplingTime)
        {
            pdf.samplingTime = samplingTime;
        }

        public bool KillNetstat()
        {
            if (ndf != null)
            {
                try
                {
                    ndf.netstat.Kill();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't kill netstat.\n" + ex.Message);
                    return false;
                }
            }
            else
                return true;
        }

        public bool NetstatHasExited()
        {
            if (ndf != null)
            {
                ndf.netstat.Refresh();
                return ndf.netstat.HasExited;
            }
            else
                return true;
        }
    }
}
