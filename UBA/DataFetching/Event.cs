using System;

namespace UBA
{
    public enum EventType {SINGLE, MULTIPLE};

    public class Event
    {
        public int id;
        public DateTime timestamp;
        public string description;
        public string instance;
        public EventType et;

        public Event(int id, DateTime timestamp, string description, string instance, EventType et)
        {
            this.id = id;
            this.timestamp = timestamp;
            this.description = description;
            this.instance = instance;
            this.et = et;
        }
    }

    class SEvent : Event
    {
        public string[] valueNames;
        public string[] values;

        public SEvent(int id, DateTime timestamp, string description, string instance, EventType et, string[] valueNames, string[] values) : base(id, timestamp, description, instance, et)
        {
            this.id = id;
            this.timestamp = timestamp;
            this.description = description;
            this.instance = instance;
            this.et = et;
            this.valueNames = valueNames;
            this.values = values;
        }

        public string GetContent()
        {
            string content = "";
            for (int i = 0; i < valueNames.Length; i++)
                content += "" + valueNames[i] + ":" + values[i] + "\n";

            return content;
        }

        public string ToString()
        {
            return String.Format("ID:{0}\nDescription:{1, -25}\nTimestamp:{2}\n{3}", id, description, timestamp, GetContent());
        }

        public void PrintEvent()
        {
            Console.WriteLine(String.Format("ID:{0}\nDescription:{1, -25}\nTimestamp:{2}\n{3}",id, description, timestamp, GetContent()));
        }
    }

    class MEvent : Event
    {
        public int eventsNo;
        public SEvent[] events;

        public MEvent(int id, DateTime timestamp, string description, EventType et, int eventsNo, SEvent[] events) : base(id, timestamp, description, "multiple", et)
        {
            this.id = id;
            this.timestamp = timestamp;
            this.description = description;
            this.et = et;
            this.eventsNo = eventsNo;
            this.events = events;
        }

        public void PrintEvent()
        {
            Console.WriteLine(String.Format("ID:{0}\tDescription:{1, -25}\tTimestamp:{2}\tNo. of events:{3}", id, description, timestamp, eventsNo));

            int min = (eventsNo < 5) ? eventsNo : 5;

            for (int i = 0; i < min; i++)
                Console.WriteLine(String.Format("\t\tInstance:{0,-25}\t{1}", events[i].instance, events[i].GetContent()));
        }

        public string ToString()
        {
            return String.Format("ID:{0}\nDescription:{1, -25}\nTimestamp:{2}\nNo. of events:{3}", id, description, timestamp, eventsNo);
        }
    }
}
