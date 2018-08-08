using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBA
{
    public enum ALERT_TYPE {LOW, MED, HIGH};

    public class Alert
    {
        public ALERT_TYPE type;
        public string description;
        public DateTime timestamp;
        public bool read;

        public Event e;

        public Alert(ALERT_TYPE type, string description, DateTime timestamp, Event e = null)
        {
            this.type = type;
            this.description = description;
            this.timestamp = timestamp;
            this.e = e;
            this.read = false;
        }

        public string ToString()
        {
            string t = "";
            switch(type)
            {
                case ALERT_TYPE.LOW:
                    t += "LOW";
                    break;
                case ALERT_TYPE.MED:
                    t += "MED";
                    break;
                case ALERT_TYPE.HIGH:
                    t += "HIGH";
                    break;
            }

            return String.Format("TYPE:{0}\tDescription:{1, -25}\tTimestamp:{2}", t, description, timestamp);
        }
    }
}
