using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBA
{
    public class BasicData
    {
        // logins
        public long[] startups;                                  // 24 x 1h bins
        // processes
        public List<string> knownProcessesList;
        public List<string> knownElevatedProcessesList;
        // DNS domains
        public Dictionary<string, long> topLevelDomainsConnections;
        public Dictionary<string, long> domainConnections;
        // countries
        public Dictionary<string, long> countryConnections;
        // network
        public Dictionary<string, long> usedProtocols;

        public BasicData()
        {
            startups = new long[24];
            knownProcessesList = new List<string>();
            knownElevatedProcessesList = new List<string>();
            topLevelDomainsConnections = new Dictionary<string, long>();
            domainConnections = new Dictionary<string, long>();
            countryConnections = new Dictionary<string, long>();
            usedProtocols = new Dictionary<string, long>();
        }
    }
}
