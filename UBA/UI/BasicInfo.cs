using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UBA
{
    public partial class BasicInfo : UserControl
    {
        private Manager man;
        private List<long> cpuUsage;
        private List<long> gpuUsage;
        private Timer updateUsageTimer;

        private List<string> knownProcesses = new List<string>();
        private List<string> knownElevatedProcesses = new List<string>();

        private Dictionary<string, long> countries = new Dictionary<string, long>();
        private Dictionary<string, long> domains = new Dictionary<string, long>();
        private Dictionary<string, long> protocols = new Dictionary<string, long>();


        public BasicInfo(Manager man)
        {
            InitializeComponent();

            this.man = man;

            // init lists and timer
            cpuUsage = new List<long>(new long[100]);
            gpuUsage = new List<long>(new long[100]);
            updateUsageTimer = new Timer
            {
                Interval = 500
            };
            updateUsageTimer.Tick += new EventHandler(UpdateUsageTimer_Tick);
            updateUsageTimer.Enabled = true;

            // set limits on usage charts
            cpuUsageChart.ChartAreas[0].AxisX.Maximum = 100;
            cpuUsageChart.ChartAreas[0].AxisX.Minimum = 0;
            //cpuUsageChart.ChartAreas[0].AxisY.Maximum = 100;
            //cpuUsageChart.ChartAreas[0].AxisY.Minimum = 0;
            gpuUsageChart.ChartAreas[0].AxisX.Maximum = 100;
            gpuUsageChart.ChartAreas[0].AxisX.Minimum = 0;
            //gpuUsageChart.ChartAreas[0].AxisY.Maximum = 100;
            //gpuUsageChart.ChartAreas[0].AxisY.Minimum = 0

            UpdateData();
        }

        private void UpdateData()
        {
            knownProcesses = man.basicData.knownProcessesList;
            knownElevatedProcesses = man.basicData.knownElevatedProcessesList;

            countries = man.basicData.countryConnections;
            domains = man.basicData.domainConnections;
            protocols = man.basicData.usedProtocols;

            var topCountries = from entry in countries orderby entry.Value descending select entry.Key;
            var topDomains = from entry in domains orderby entry.Value descending select entry.Key;
            var topProtocols = from entry in protocols orderby entry.Value descending select entry.Key;

            countriesChart.Series["Countries"].Points.Clear();
            int counter = 0;
            foreach(var s in topCountries)
            {
                if(counter++ < 10)
                    countriesChart.Series["Countries"].Points.AddXY(s, countries[s]);
                countriesDataGrid.Rows.Add(s, countries[s]);
            }

            domainsChart.Series["Domains"].Points.Clear();
            counter = 0;
            foreach (var s in topDomains)
            {
                if(counter++ < 10)
                    domainsChart.Series["Domains"].Points.AddXY(s, domains[s]);
                domainsDataGrid.Rows.Add(s, domains[s]);
            }

            protocolsChart.Series["Protocols"].Points.Clear();
            counter = 0;
            foreach (var s in topProtocols)
            {
                if(counter++ < 5)
                    protocolsChart.Series["Protocols"].Points.AddXY(s, protocols[s]);
                protocolsDataGrid.Rows.Add(s, protocols[s]);
            }

            knownProcesses.Sort();
            knownElevatedProcesses.Sort();
            foreach (string s in knownProcesses)
                processesDataGrid1.Rows.Add(s);
            foreach (string s in knownElevatedProcesses)
                processesDataGrid2.Rows.Add(s);
        }

        private void UpdateUsageTimer_Tick(object Sender, EventArgs e)
        {
            int cpu_val = PerformanceDataFetcher.GetCPUValue();
            int gpu_val = PerformanceDataFetcher.GetGPUValue();

            // if the pc doesn't have the NVidia driver 
            if (gpu_val == -1)
                gpu_val = 0;

            cpuUsage.RemoveAt(0);
            cpuUsage.Add((cpu_val+cpuUsage[98])/2); // 1 entry was removed above so there are 99 in total
            gpuUsage.RemoveAt(0);
            gpuUsage.Add((gpu_val+gpuUsage[98])/2);

            cpuUsageChart.Series["Usage"].Points.Clear();
            gpuUsageChart.Series["Usage"].Points.Clear();

            for (int i = 0; i < 100; i++)
            {
                cpuUsageChart.Series["Usage"].Points.AddXY(i, cpuUsage[i]);
                gpuUsageChart.Series["Usage"].Points.AddXY(i, gpuUsage[i]);
            }
        }

        private void cpuUsageChart_Click(object sender, EventArgs e)
        {

        }
    }
}
