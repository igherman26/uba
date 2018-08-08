using Newtonsoft.Json;
using OS_module_cs.nvidia;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace UBA
{
    public class CounterData
    {
        public string CategoryName;
        public string CounterName;
        public string description;
        public int id;

        public CounterData(string CategoryName, string CounterName, string description, int id)
        {
            this.CategoryName = CategoryName;
            this.CounterName = CounterName;
            this.description = description;
            this.id = id;
        }
    }

    class PerformanceDataFetcher : DataFetcher
    {
        // default configuration file location
        public const string COUNTERS_FILE = "counters.json";
        public const int MAX_COUNTERS = 16;
        private static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        // the performance counter threads list
        List<Thread> pcThreads = new List<Thread>();

        // the constructor - used to init the shared events list
        public PerformanceDataFetcher(BlockingCollection<Event> events)
        {
            this.events = events;
        }

        // contains the used PerformanceCounters
        private Dictionary<string, List<PerformanceCounter>> performanceCounters = new Dictionary<string, List<PerformanceCounter>>();
        // contains the counters to be loaded
        private List<CounterData> counters = new List<CounterData>();
        // it's the shared queue where the generated events need to be added
        private BlockingCollection<Event> events = new BlockingCollection<Event>();

        // returns a list with the available categories
        public string[] GetAvailableCategories()
        {
            PerformanceCounterCategory[] categories = PerformanceCounterCategory.GetCategories();
            List<string> categoryNames = new List<string>();

            foreach (PerformanceCounterCategory c in categories)
                categoryNames.Add(c.CategoryName);

            return categoryNames.ToArray();
        }

        // returns a dictionary with info: name, type and help
        public Dictionary<string, string> GetCategoryInfo(string categoryName)
        {
            PerformanceCounterCategory[] categories = PerformanceCounterCategory.GetCategories();
            Dictionary<string, string> res = new Dictionary<string, string>();

            foreach (PerformanceCounterCategory c in categories)
                if (c.CategoryName == categoryName)
                {
                    res.Add("name", c.CategoryName);
                    res.Add("type", c.CategoryType.ToString());
                    res.Add("help", c.CategoryHelp);
                    break;
                }

            return res;
        }

        // returns an array of instances (if it is multi-instance)
        public string[] GetCategoryInstances(string categoryName)
        {
            PerformanceCounterCategory[] categories = PerformanceCounterCategory.GetCategories();

            foreach (PerformanceCounterCategory c in categories)
                if (c.CategoryName == categoryName)
                    return c.GetInstanceNames();

            return null;
        }

        // get a list of available counters for the category
        public string[] GetCategoryCounters(string categoryName)
        {
            PerformanceCounterCategory[] categories = PerformanceCounterCategory.GetCategories();
            PerformanceCounter[] counters;
            List<string> counterNames = new List<string>();

            foreach (PerformanceCounterCategory c in categories)
                if (c.CategoryName == categoryName)
                {
                    counters = c.GetCounters();
                    foreach (PerformanceCounter pc in counters)
                    {
                        counterNames.Add(pc.CategoryName);
                    }

                    return counterNames.ToArray();
                }

            return null;
        }

        // init the counters
        protected override void InitFetcher()
        {
            LoadCountersFromFile();

            foreach (CounterData c in counters)
                AddPerformanceCounter(c.CategoryName, c.CounterName);
        }

        // loads the counter list from the json file
        private bool LoadCountersFromFile(string pathToJson = COUNTERS_FILE)
        {
            try
            {
                using (StreamReader r = new StreamReader(pathToJson))
                {
                    string json = r.ReadToEnd();
                    counters = JsonConvert.DeserializeObject<List<CounterData>>(json);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // save the counter list in the json file
        public bool SaveCountersToFile(string pathToJson = COUNTERS_FILE)
        {
            try
            {
                File.WriteAllText(pathToJson, JsonConvert.SerializeObject(counters));
                return true;
            }
            catch
            {
                return false;
            }
        }

        // add a counter to the counter list
        public void AddCounter(CounterData counter)
        {
            if (counters.Count < MAX_COUNTERS)
                counters.Add(counter);
        }

        // remove a counter from the counter list
        public void RemoveCounter(CounterData counter)
        {
            counters.Remove(counter);
        }

        // add the counter to the active counters
        private bool AddPerformanceCounter(string categoryName, string counterName)
        {
            try
            {
                string[] instances = GetCategoryInstances(categoryName);
                List<PerformanceCounter> lpc = new List<PerformanceCounter>();

                if (instances.Any())
                {
                    foreach (string instance in instances)
                        lpc.Add(new PerformanceCounter(categoryName, counterName, instance));
                    performanceCounters.Add(categoryName + counterName, lpc);
                }
                else
                {
                    lpc.Add(new PerformanceCounter(categoryName, counterName));
                    performanceCounters.Add(categoryName + counterName, lpc);

                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // contains the initialization of the default counters
        public void DefaultInit()
        {
            //AddCounter(new CounterData("Process", "ID Process", "running processes", 50));
            AddCounter(new CounterData("Processor", "% Processor Time", "global CPU usage", 51));       
            AddCounter(new CounterData("Process", "% Processor Time", "local CPU usage", 52));          
            //AddCounter(new CounterData("Processor", "% Privileged Time", "global privileged CPU usage", 53));
            //AddCounter(new CounterData("Process", "% Privileged Time", "local privileged CPU usage", 54));                   
            //AddCounter(new CounterData("Memory", "Available MBytes"));
            AddCounter(new CounterData("Memory", "% Committed Bytes In Use", "global Memory usage %", 55));
            //AddCounter(new CounterData("Process", "Working Set - Private", "local Memory usage in bytes", 56));
            
            AddCounter(new CounterData("PhysicalDisk", "Disk Bytes/sec", "global Disk usage in B/sec", 57));
            //AddCounter(new CounterData("Process", "IO Data Bytes/sec", "local IO usage in B/sec", 58));
            
            AddCounter(new CounterData("Network Interface", "Bytes Sent/Sec", "global Network sent B/s", 60));
            AddCounter(new CounterData("Network Interface", "Bytes Received/Sec", "global Network received B/s", 61));
            
            //AddCounter(new CounterData("Process", "Elapsed Time", "process elapsed time", 62));

            SaveCountersToFile();
        }

        // starts a main thread which assigns PerformanceCounters to individual threads
        protected override void RunFetcher()
        {
            int counter = 0;

            // create and launch a thread for each counter
            foreach (KeyValuePair<string, List<PerformanceCounter>> entry in performanceCounters)
            { 
                pcThreads.Add(new Thread(() => FetchCounter(entry.Value)));
                pcThreads[counter++].Start();
            }

            // create launch a thread for the GPU fetcher
            pcThreads.Add(new Thread(() => FetchGPUCounter(null)));
            pcThreads[counter].Start();
        }

        // override the stopfetching
        public override bool StopFetching()
        {
            try
            {
                fetchingThread.Abort();
                foreach (Thread t in pcThreads)
                    t.Abort();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // fetches the GPU counter using the NVidia API wrapper
        // code from https://github.com/coraxx/CpuGpuGraph
        // originally inspired from https://eliang.blogspot.com/2011/05/getting-nvidia-gpu-usage-in-c.html
        void FetchGPUCounter(object o)
        {
            while(true)
            {
                try
                {
                    // send the GPU usage
                    DateTime time = DateTime.Now;
                    ParseData(new SEvent(59, time, "global GPU usage", "global", EventType.SINGLE, new string[] { "value" }, new string[] { NvGpuLoad.GetGpuLoad().ToString() }));
                    Thread.Sleep(samplingTime);
                }
                catch   
                {
                    // probably no NVidia GPU is present so close the thread
                    //TODO should be managed in a more elegant way
                    Thread.CurrentThread.Abort();
                }
            }
        }

        // fetches a counter - used as a callback
        // if the counter is a big multi-instance counter it doesn't sleep the sampleTime
        // because it take a lot of time and it won't be able to send data faster than once per second anyway
        void FetchCounter(object o)
        {
            List<PerformanceCounter> entry = (List<PerformanceCounter>)o;
            while (true)
            {
                DateTime time = DateTime.Now;

                PerformanceCounter pc0 = entry[0];
                // obtain event.id and event.Description
                var query = from CounterData cd in counters where cd.CategoryName == pc0.CategoryName && cd.CounterName == pc0.CounterName select cd;
                CounterData qcd = query.FirstOrDefault();
                int id = qcd.id;
                string description = qcd.description;

                if (entry.Count == 1)
                {
                    // single-instance
                    int val = (int)Math.Round(pc0.NextValue());
                    if (val < 0)
                        val = 0;

                    ParseData(new SEvent(id, time, description, "global", EventType.SINGLE, new string[] { "value" }, new string[] { val.ToString() }));
                    Thread.Sleep(samplingTime);
                }
                if (entry.Count > 1)
                {
                    // multi-instance
                    SEvent[] events = new SEvent[entry.Count];

                    int i = 0;

                    foreach (PerformanceCounter pc in entry)
                    {
                        string instance = pc.InstanceName;
                        int val;

                        try
                        {
                            val = (int)Math.Round(pc.NextValue());
                            if (val < 0)
                                val = 0;
                        }
                        catch
                        {
                            val = -1;
                        }
                        events[i++] = new SEvent(id, time, description, instance, EventType.SINGLE, new string[] { "value" }, new string[] { val.ToString() });
                    }

                    ParseData(new MEvent(id, time, description, EventType.MULTIPLE, events.Length, events));

                    // if it's a big multi-instance counter skip the sleeping part
                    if(entry.Count < 10)
                        Thread.Sleep(samplingTime);
                }
            }
        }

        // only adds the event to the event list
        protected override void ParseData(Event e)
        {
            events.TryAdd(e);
        }

        // static methods to be used directly by the UI
        public static int GetCPUValue()
        {
            return (int)Math.Ceiling(cpuCounter.NextValue());
        }
        public static int GetGPUValue()
        {
            try
            {
                return NvGpuLoad.GetGpuLoad();
            }
            catch
            {
                // probably no NVidia GPU is present so close the thread
                return -1;
            }
        }
    }
}