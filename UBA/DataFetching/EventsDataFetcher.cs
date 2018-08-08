using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace UBA
{
    class EventsDataFetcher : DataFetcher
    {
        // default configuration files locations
        public const string POLICIES_FILE = "policies.csv";
        public const string PARSERS_FILE = "parsers.json";

        // current loaded event parsers
        public Dictionary<int, EventParser> parsers = new Dictionary<int, EventParser>();
        // it's the shared queue where the generated events need to be added
        private BlockingCollection<Event> events = new BlockingCollection<Event>();

        // internal event parser class with a default parsing method and some static parsing methods
        internal class EventParser
        {
            // data for basic cases
            public int id;
            public string message;
            public string[] valueNames;
            public int[] valuePositions;

            public Event ParseEventLog(EventLogEntry ele)
            {
                // treating particular cases first
                switch (ele.EventID)
                {
                    case 4663:
                        return Treat4663(ele);
                    case 4688:
                        return Treat4688(ele);
                    default:
                        string[] values = new string[valueNames.Length];

                        for (int i = 0; i < values.Length; i++)
                            values[i] = ele.ReplacementStrings[valuePositions[i]];

                        return new SEvent(id, ele.TimeGenerated, message, "", EventType.SINGLE, valueNames, values);
                }
            }

            public static Event Treat4663(EventLogEntry ele)
            {
                string access = ele.ReplacementStrings[9];

                switch (ele.ReplacementStrings[5])
                {
                    case "File":
                        if (access == "0x1")
                            return new SEvent(30, ele.TimeGenerated, "sensitive file/folder opened", "", EventType.SINGLE, new string[] { "pid", "process", "file" },
                                new string[] { ele.ReplacementStrings[10], ele.ReplacementStrings[11], ele.ReplacementStrings[6] });

                        if (access == "0x2" || access == "0x4" || access == "0x6")
                            return new SEvent(31, ele.TimeGenerated, "sensitive file/folder modified", "", EventType.SINGLE, new string[] { "pid", "process", "file" },
                                new string[] { ele.ReplacementStrings[10], ele.ReplacementStrings[11], ele.ReplacementStrings[6] });
                        if (access == "0x10000")
                            return new SEvent(32, ele.TimeGenerated, "sensitive file/folder deleted", "", EventType.SINGLE, new string[] { "pid", "process", "file" },
                                new string[] { ele.ReplacementStrings[10], ele.ReplacementStrings[11], ele.ReplacementStrings[6] });
                        break;
                    case "Key":
                        if (access == "0x1")
                            return new SEvent(33, ele.TimeGenerated, "registry key query", "", EventType.SINGLE, new string[] { "pid", "process", "file" },
                                new string[] { ele.ReplacementStrings[10], ele.ReplacementStrings[11], ele.ReplacementStrings[6] });
                        break;
                    default:
                        break;
                }

                return null;
            }

            public static Event Treat4688(EventLogEntry ele)
            {
                switch (ele.ReplacementStrings[6])
                {
                    case "%%1936":
                        return new SEvent(20, ele.TimeGenerated, "process creation with full elevation", "", EventType.SINGLE, new string[] { "pidNew", "processNew", "cmdLine", "processC", "pidC" },
                                new string[] { ele.ReplacementStrings[4], ele.ReplacementStrings[5], ele.ReplacementStrings[8], ele.ReplacementStrings[13], ele.ReplacementStrings[7] });
                    case "%%1937":
                        return new SEvent(21, ele.TimeGenerated, "process creation with default elevation", "", EventType.SINGLE, new string[] { "pidNew", "processNew", "cmdLine", "processC", "pidC" },
                                new string[] { ele.ReplacementStrings[4], ele.ReplacementStrings[5], ele.ReplacementStrings[8], ele.ReplacementStrings[13], ele.ReplacementStrings[7] });
                    case "%%1938":
                        return new SEvent(22, ele.TimeGenerated, "process creation with limited elevation", "", EventType.SINGLE, new string[] { "pidNew", "processNew", "cmdLine", "processC", "pidC" },
                                new string[] { ele.ReplacementStrings[4], ele.ReplacementStrings[5], ele.ReplacementStrings[8], ele.ReplacementStrings[13], ele.ReplacementStrings[7] });
                    default:
                        break;
                }

                return null;
            }

        }

        // auditing types
        public enum AuditSetting
        {
            NoAuditing, Success, Failure, SuccessAndFailure
        }

        // the constructor - used to init the shared events list
        public EventsDataFetcher(BlockingCollection<Event> events)
        {
            this.events = events;
        }

        // set an audit policy
        // 0 - No Auditing ; 1 - Success ; 2 - Failure ; 3 - Success and Failure
        public bool SetPolicy(string subcategory, AuditSetting setting)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Normal,  // = Hidden
                FileName = "cmd.exe"
            };

            switch (setting)
            {
                case AuditSetting.NoAuditing:
                    startInfo.Arguments = String.Format("/get /subcategory:\"{0}\" /failure:disable /success:disable", subcategory);
                    break;
                case AuditSetting.Success:
                    startInfo.Arguments = String.Format("/get /subcategory:\"{0}\" /failure:disable /success:enable", subcategory);
                    break;
                case AuditSetting.Failure:
                    startInfo.Arguments = String.Format("/get /subcategory:\"{0}\" /failure:enable /success:disable", subcategory);
                    break;
                case AuditSetting.SuccessAndFailure:
                    startInfo.Arguments = String.Format("/get /subcategory:\"{0}\" /failure:enable /success:enable", subcategory);
                    break;
                default:
                    break;
            }

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
                return false;
            else
                return true;
        }

        // returns all current policies
        public Dictionary<String, AuditSetting> GetAllPolicies()
        {
            Dictionary<String, AuditSetting> res = new Dictionary<string, AuditSetting>();

            string path = Path.GetRandomFileName();
            if (!this.SavePolicies(path))
                return null;

            var lines = File.ReadLines(path);
            foreach (var line in lines)
            {
                string[] splittedLine = line.Split(',');
                if (splittedLine[1] != "" || splittedLine[1] != "Policy Target")
                {
                    switch (splittedLine[4])
                    {
                        case "No Auditing":
                            res.Add(splittedLine[2], AuditSetting.NoAuditing);
                            break;
                        case "Success":
                            res.Add(splittedLine[2], AuditSetting.Success);
                            break;
                        case "Failure":
                            res.Add(splittedLine[2], AuditSetting.Failure);
                            break;
                        case "Success and Failure":
                            res.Add(splittedLine[2], AuditSetting.SuccessAndFailure);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (File.Exists(path))
                File.Delete(path);

            return res;
        }

        // saves the current policies in a csv file
        public bool SavePolicies(string fileName = POLICIES_FILE)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Normal,  // = Hidden
                FileName = "auditpol",
                Arguments = String.Format("/backup /file:{0}", fileName)
            };

            process.StartInfo = startInfo;
            process.Start();

            process.WaitForExit();

            if (process.ExitCode != 0)
                return false;
            else
                return true;
        }

        // restores/loads policies
        public bool RestorePolicies(string fileName = POLICIES_FILE)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Normal,  // = Hidden
                FileName = "auditpol",
                Arguments = String.Format("/restore /file:{0}", fileName)
            };

            process.StartInfo = startInfo;
            process.Start();

            process.WaitForExit();

            if (process.ExitCode != 0)
                return false;
            else
                return true;
        }

        // loads the parsers from the json file
        private bool LoadParsers(string fileName = PARSERS_FILE)
        {
            try
            {
                using (StreamReader r = new StreamReader(fileName))
                {
                    string json = r.ReadToEnd();
                    parsers = JsonConvert.DeserializeObject<Dictionary<int, EventParser>>(json);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // saves the parsers in the json file
        public bool SaveParsers(string fileName = PARSERS_FILE)
        {
            try
            {
                File.WriteAllText(fileName, JsonConvert.SerializeObject(parsers));
                return true;
            }
            catch
            {
                return false;
            }
        }

        // contains the initialization of the default event parsers
        public void DefaultInit()
        {
            parsers.Add(4608, new EventParser()
            {
                id = 1,
                message = "system startup",
                valueNames = new string[] { },
                valuePositions = new int[] { }
            });

            parsers.Add(4609, new EventParser()
            {
                id = 2,
                message = "system shutdown",
                valueNames = new string[] { },
                valuePositions = new int[] { }
            });

            parsers.Add(4616, new EventParser()
            {
                id = 3,
                message = "system time change",
                valueNames = new string[] { "user", "pid", "process" },
                valuePositions = new int[] { 1, 6, 7 }
            });

            parsers.Add(4907, new EventParser()
            {
                id = 4,
                message = "auditing settings on object change",
                valueNames = new string[] { "user", "pid", "process", "object_type", "object_name" },
                valuePositions = new int[] { 1, 10, 11, 5, 6 }
            });

            parsers.Add(4912, new EventParser()
            {
                id = 5,
                message = "audit policy change",
                valueNames = new string[] { "user", "category_id", "subcategory_id" },
                valuePositions = new int[] { 1, 5, 6 }
            });

            parsers.Add(4719, new EventParser()
            {
                id = 5,
                message = "audit policy change",
                valueNames = new string[] { "user", "category_id", "subcategory_id" },
                valuePositions = new int[] { 1, 4, 5 }
            });

            parsers.Add(4624, new EventParser()
            {
                id = 6,
                message = "account logon",
                valueNames = new string[] { "user" },
                valuePositions = new int[] { 1 }
            });

            parsers.Add(4625, new EventParser()
            {
                id = 7,
                message = "account failed to log on",
                valueNames = new string[] { },
                valuePositions = new int[] { }
            });

            parsers.Add(4634, new EventParser()
            {
                id = 8,
                message = "account logout",
                valueNames = new string[] { "user" },
                valuePositions = new int[] { 1 }
            });

            parsers.Add(4672, new EventParser()
            {
                id = 9,
                message = "special privileges assigned to logon",
                valueNames = new string[] { "user" },
                valuePositions = new int[] { 1 }
            });

            parsers.Add(4800, new EventParser()
            {
                id = 10,
                message = "workstation was locked",
                valueNames = new string[] { "user" },
                valuePositions = new int[] { 1 }
            });

            parsers.Add(4801, new EventParser()
            {
                id = 11,
                message = "workstation was unlocked",
                valueNames = new string[] { "user" },
                valuePositions = new int[] { 1 }
            });

            // --------------------------------------------------------------------

            parsers.Add(4689, new EventParser()
            {
                id = 23,
                message = "process termination",
                valueNames = new string[] { "pid", "process" },
                valuePositions = new int[] { 5, 6 }
            });

            // --------------------------------------------------------------------

            parsers.Add(4657, new EventParser()
            {
                id = 34,
                message = "registry key set",
                valueNames = new string[] { "pid", "process", "reg_key", "from_value", "to_value" },
                valuePositions = new int[] { 12, 13, 4, 9, 11 }
            });

            parsers.Add(4946, new EventParser()
            {
                id = 35,
                message = "MS Firewall rules modifications",
                valueNames = new string[] { "rule_id", "rule_name" },
                valuePositions = new int[] { 1, 2 }
            });

            parsers.Add(4947, new EventParser()
            {
                id = 35,
                message = "MS Firewall rules modifications",
                valueNames = new string[] { "rule_id", "rule_name" },
                valuePositions = new int[] { 1, 2 }
            });

            parsers.Add(4948, new EventParser()
            {
                id = 35,
                message = "MS Firewall rules modifications",
                valueNames = new string[] { "rule_id", "rule_name" },
                valuePositions = new int[] { 1, 2 }
            });

            // --------------------------------------------------------------------


            //TODO - add these events
            /*
            parsers.Add(, new EventParser()
            {
                id = 12,
                message = "event log service was started",
                valueNames = new string[] { },
                valuePositions = new int[] { }
            });

            parsers.Add(, new EventParser()
            {
                id = 13,
                message = "event log service was stopped",
                valueNames = new string[] { },
                valuePositions = new int[] { }
            });
            */
            // --------------------------------------------------------------------

            SaveParsers();
        }

        // loads the event parsers
        protected override void InitFetcher()
        {
            LoadParsers();
        }

        // starts the fetching thread
        protected override void RunFetcher()
        {
            EventLog[] eventLogs = EventLog.GetEventLogs();
            EventLog securityLog = eventLogs.FirstOrDefault(x => x.Log == "Security");

            EventLogEntryCollection securityEntries = securityLog.Entries;

            int lastCount = -1;
            int lastIndex = -1;
            try
            {
                lastIndex = securityEntries[securityEntries.Count - 1].Index;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(-1);
            }

            while (true)
            {
                if (securityEntries.Count != lastCount)
                {
                    lastCount = securityEntries.Count;

                    var query = from EventLogEntry e in securityEntries where e.Index > lastIndex select e;

                    foreach (EventLogEntry ele in query)
                    {
                        Event e = null;
                        if (parsers.ContainsKey(ele.EventID))
                            e = parsers[ele.EventID].ParseEventLog(ele);


                        if (ele.EventID == 4663)
                            e = EventParser.Treat4663(ele);


                        if (ele.EventID == 4688)
                            e = EventParser.Treat4688(ele);

                        if (e != null)
                            ParseData(e);
                    }

                    if (query.Any())
                        lastIndex = query.Last().Index;
                }

                Thread.Sleep(samplingTime);
            }
        }

        // adds the event to the shared queue
        protected override void ParseData(Event e)
        {
            events.TryAdd(e);
        }
    }
}
