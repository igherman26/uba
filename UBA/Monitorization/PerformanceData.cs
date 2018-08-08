using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBA
{
    public class PerformanceData
    {
        // network usage
        public long[] globalNetworkSend;                             // 5 min bins
        public long[] globalNetworkReceive;                          // 5 min bins / process
        public Dictionary<string, long[]> processesNetworkSend;      // 5 min bins / process
        public Dictionary<string, long[]> processesNetworkReceive;   // 5 min bins / process
        // disk usage
        public long[] globalDiskUsage;                               // 5 min bins
        //public Dictionary<string, long[]> processesDiskRead;       // 5 min bins / process
        //public Dictionary<string, long[]> processesDiskWrite;      // 5 min bins / process
        // CPU usage
        public long[] globalCPUusage;                                // 0.5 min bins
        public long[] globalCPUusageValuesNo;                        // needed to compute the medium value in the 30 seconds bin
        public Dictionary<string, long[]> processesCPUusage;         // 0.5 min bins / process
        public Dictionary<string, long[]> processesCPUusageValuesNo; // needed to compute the medium value in the 30 seconds bin
        // GPU usage
        public long[] globalGPUusage;                                // 0.5 min bins
        public long[] globalGPUusageValuesNo;                        // needed to compute the medium value in the 30 seconds bin
        // Memory usage
        public long[] globalMemoryUsage;                             // 0.5 min bins
        public long[] globalMemoryUsageValuesNo;                     // needed to compute the medium value in the 30 seconds bin
        
        // packets
        public long[] packets_counter;                               // 48 x 0.5h bins

        public PerformanceData()
        {
            globalNetworkSend = new long[288];
            globalNetworkReceive = new long[288];
            processesNetworkSend = new Dictionary<string, long[]>();
            processesNetworkReceive = new Dictionary<string, long[]>();
            globalDiskUsage = new long[288];
            //processesDiskRead = new Dictionary<string, long[]>();
            //processesDiskWrite = new Dictionary<string, long[]>();
            globalCPUusage = new long[2880];
            globalCPUusageValuesNo = new long[2880];
            processesCPUusage = new Dictionary<string, long[]>();
            processesCPUusageValuesNo = new Dictionary<string, long[]>();
            globalGPUusage = new long[2880];
            globalGPUusageValuesNo = new long[2880];
            globalMemoryUsage = new long[2880];
            globalMemoryUsageValuesNo = new long[2880];
            packets_counter = new long[48];
        }
    }
}
