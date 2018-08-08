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
    public partial class PerformanceUserControl : UserControl
    {
        private Manager man;
        private PerformanceData otherDateData;
        private long[] cpuUsage;
        private long[] cpuUsageValuesNo;
        private long[] cpuUsageBaseline;
        private long[] cpuUsageBaselineValuesNo;
        private long[] gpuUsage;
        private long[] gpuUsageValuesNo;
        private long[] gpuUsageBaseline;
        private long[] gpuUsageBaselineValuesNo;
        private long[] memoryUsage;
        private long[] memoryUsageValuesNo;
        private long[] memoryUsageBaseline;
        private long[] memoryUsageBaselineValuesNo;

        private long[] networkSend;
        private long[] networkSendBaseline;
        private long[] networkReceive;
        private long[] networkReceiveBaseline;
        private long[] diskUsage;
        private long[] diskUsageBaseline;

        private long[] packets;
        private long[] packetsBaseline;

        private int baselineDatesCount;

        public PerformanceUserControl(Manager man)
        {
            InitializeComponent();

            this.man = man;

            // add days in the combo box
            List<String> days = man.currentUser.availableDates;
            foreach (string s in days)
                dayComboBox.Items.Add(String.Format("{0}/{1}/{2}", s.Substring(6, 2), s.Substring(4, 2), s.Substring(0, 4)));
            if (dayComboBox.Items.Count == 0)
            {
                dayComboBox.Text = "No days available.";
                dayComboBox.Enabled = false;
            }
            else
                dayComboBox.SelectedIndex = dayComboBox.Items.Count - 1;

            // set current day
            SetCurrentDateData();

            // set baselineDatesCount
            baselineDatesCount = man.currentUser.baselineDates.Count;

            UpdateData();
        }

        private void SetCurrentDateData()
        {
            cpuUsage = man.performanceData.globalCPUusage;
            cpuUsageValuesNo = man.performanceData.globalCPUusageValuesNo;
            cpuUsageBaseline = man.baselineData.globalCPUusage;
            cpuUsageBaselineValuesNo = man.baselineData.globalCPUusageValuesNo;
            gpuUsage = man.performanceData.globalGPUusage;
            gpuUsageValuesNo = man.performanceData.globalGPUusageValuesNo;
            gpuUsageBaseline = man.baselineData.globalGPUusage;
            gpuUsageBaselineValuesNo = man.baselineData.globalGPUusageValuesNo;
            memoryUsage = man.performanceData.globalMemoryUsage;
            memoryUsageValuesNo = man.performanceData.globalMemoryUsageValuesNo;
            memoryUsageBaseline = man.baselineData.globalMemoryUsage;
            memoryUsageBaselineValuesNo = man.baselineData.globalMemoryUsageValuesNo;

            networkSend = man.performanceData.globalNetworkSend;
            networkSendBaseline = man.baselineData.globalNetworkSend;
            networkReceive = man.performanceData.globalNetworkReceive;
            networkReceiveBaseline = man.baselineData.globalNetworkReceive;
            diskUsage = man.performanceData.globalDiskUsage;
            diskUsageBaseline = man.baselineData.globalDiskUsage;

            packets = man.performanceData.packets_counter;
            packetsBaseline = man.baselineData.packets_counter;
        }

        private void SetAnotherDateData(int index)
        {
            string date = man.currentUser.availableDates[index];
            otherDateData = man.GetPerformanceData(date);

            cpuUsage = otherDateData.globalCPUusage;
            cpuUsageValuesNo = otherDateData.globalCPUusageValuesNo;
            cpuUsageBaseline = man.baselineData.globalCPUusage;
            cpuUsageBaselineValuesNo = man.baselineData.globalCPUusageValuesNo;
            gpuUsage = otherDateData.globalGPUusage;
            gpuUsageValuesNo = otherDateData.globalGPUusageValuesNo;
            gpuUsageBaseline = man.baselineData.globalGPUusage;
            gpuUsageBaselineValuesNo = man.baselineData.globalGPUusageValuesNo;
            memoryUsage = otherDateData.globalMemoryUsage;
            memoryUsageValuesNo = otherDateData.globalMemoryUsageValuesNo;
            memoryUsageBaseline = man.baselineData.globalMemoryUsage;
            memoryUsageBaselineValuesNo = man.baselineData.globalMemoryUsageValuesNo;

            networkSend = otherDateData.globalNetworkSend;
            networkSendBaseline = man.baselineData.globalNetworkSend;
            networkReceive = otherDateData.globalNetworkReceive;
            networkReceiveBaseline = man.baselineData.globalNetworkReceive;
            diskUsage = otherDateData.globalDiskUsage;
            diskUsageBaseline = man.baselineData.globalDiskUsage;

            packets = otherDateData.packets_counter;
            packetsBaseline = man.baselineData.packets_counter;
        }

        private void ClearCharts()
        {
            foreach (var series in cpuUsageChart.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in gpuUsageChart.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in memoryUsageChart.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in diskUsageChart.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in nsendUsageChart.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in nrecUsageChart.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in packetsChart.Series)
            {
                series.Points.Clear();
            }
        }

        private void UpdateData()
        {
            for (int i = 0; i < 2880; i ++)
            {
                if(cpuUsageValuesNo[i] != 0)
                    cpuUsageChart.Series["Usage"].Points.AddXY(i, cpuUsage[i] / cpuUsageValuesNo[i]);
                else
                    cpuUsageChart.Series["Usage"].Points.AddXY(i, 0);

                if (cpuUsageBaselineValuesNo[i] != 0 && baselineDatesCount > 0)
                    cpuUsageChart.Series["Baseline"].Points.AddXY(i, cpuUsageBaseline[i] / cpuUsageBaselineValuesNo[i] / baselineDatesCount);
                else
                    cpuUsageChart.Series["Baseline"].Points.AddXY(i, 0);
            }

            for (int i = 0; i < 2880; i++)
            {
                if (gpuUsageValuesNo[i] != 0)
                    gpuUsageChart.Series["Usage"].Points.AddXY(i, gpuUsage[i] / gpuUsageValuesNo[i]);
                else
                    gpuUsageChart.Series["Usage"].Points.AddXY(i, 0);

                if (gpuUsageBaselineValuesNo[i] != 0 && baselineDatesCount > 0)
                    gpuUsageChart.Series["Baseline"].Points.AddXY(i, gpuUsageBaseline[i] / gpuUsageBaselineValuesNo[i] / baselineDatesCount);
                else
                    gpuUsageChart.Series["Baseline"].Points.AddXY(i, 0);
            }

            for (int i = 0; i < 2880; i++)
            {
                if (memoryUsageValuesNo[i] != 0)
                    memoryUsageChart.Series["Usage"].Points.AddXY(i, memoryUsage[i] / memoryUsageValuesNo[i]);
                else
                    memoryUsageChart.Series["Usage"].Points.AddXY(i, 0);

                if (memoryUsageBaselineValuesNo[i] != 0 && baselineDatesCount > 0)
                    memoryUsageChart.Series["Baseline"].Points.AddXY(i, memoryUsageBaseline[i] / memoryUsageBaselineValuesNo[i] / baselineDatesCount);
                else
                    memoryUsageChart.Series["Baseline"].Points.AddXY(i, 0);
            }

            for (int i = 0; i < 288; i++)
            {
                diskUsageChart.Series["Usage"].Points.AddXY(i, diskUsage[i]);
                if (baselineDatesCount > 0)
                    diskUsageChart.Series["Baseline"].Points.AddXY(i, diskUsageBaseline[i] / baselineDatesCount);
                else
                    diskUsageChart.Series["Baseline"].Points.AddXY(i, 0);
            }

            for (int i = 0; i < 288; i++)
            {
                nsendUsageChart.Series["Usage"].Points.AddXY(i, networkSend[i]);
                if(baselineDatesCount > 0)
                    nsendUsageChart.Series["Baseline"].Points.AddXY(i, networkSendBaseline[i] / baselineDatesCount);
                else
                    nsendUsageChart.Series["Baseline"].Points.AddXY(i, 0);
            }

            for (int i = 0; i < 288; i++)
            {
                nrecUsageChart.Series["Usage"].Points.AddXY(i, networkReceive[i]);
                if(baselineDatesCount > 0)
                    nrecUsageChart.Series["Baseline"].Points.AddXY(i, networkReceiveBaseline[i] / baselineDatesCount);
                else
                    nrecUsageChart.Series["Baseline"].Points.AddXY(i, 0);
            }

            for (int i = 0; i < 48; i++)
            {
                packetsChart.Series["Packets"].Points.AddXY(i, packets[i]);
                if(baselineDatesCount > 0)
                    packetsChart.Series["Baseline"].Points.AddXY(i, packetsBaseline[i] / baselineDatesCount);
                else
                    packetsChart.Series["Baseline"].Points.AddXY(i, 0);
            }
        }

        private void dayComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetAnotherDateData(dayComboBox.SelectedIndex);
            ClearCharts();
            UpdateData();
        }

        private void dayComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
