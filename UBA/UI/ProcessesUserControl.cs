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
    public partial class ProcessesUserControl : UserControl
    {
        private Manager man;
        private PerformanceData currentPerformanceData;

        private long[] cpuUsage;
        private long[] cpuUsageValuesNo;
        private long[] cpuUsageBaseline;
        private long[] cpuUsageBaselineValuesNo;

        private long[] networkSend;
        private long[] networkSendBaseline;
        private long[] networkReceive;
        private long[] networkReceiveBaseline;

        private int baselineDatesCount;


        public ProcessesUserControl(Manager man)
        {
            InitializeComponent();

            this.man = man;
            this.currentPerformanceData = currentPerformanceData;
            this.baselineDatesCount = man.currentUser.baselineDates.Count;

            // add days in the days combo box
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

            dataComboBox.Enabled = false;
            processComboBox.Enabled = false;
        }

        private void SetCurrentDateData()
        {
            currentPerformanceData = man.performanceData;
        }
        private void SetAnotherDateData(int index)
        {
            string date = man.currentUser.availableDates[index];
            currentPerformanceData = man.GetPerformanceData(date);  
        }

        // actually is dataComboBox
        private void profileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            processComboBox.Enabled = false;

            // populate the other combobox
            switch(dataComboBox.SelectedIndex)
            {
                case (0):
                    {
                        List<String> processes = currentPerformanceData.processesCPUusage.Keys.ToList();
                        processes.Sort();
                        foreach (string s in processes)
                            processComboBox.Items.Add(s);
                        if (processComboBox.Items.Count == 0)
                        {
                            processComboBox.Text = "No processes available";
                            processComboBox.Enabled = false;
                        }
                        break;
                    }
                case (1):
                    {
                        List<String> processes = currentPerformanceData.processesNetworkSend.Keys.ToList();
                        processes.Sort();
                        foreach (string s in processes)
                            processComboBox.Items.Add(s);
                        if (processComboBox.Items.Count == 0)
                        {
                            processComboBox.Text = "No processes available";
                            processComboBox.Enabled = false;
                        }

                        break;
                    }
                case (2):
                    {
                        List<String> processes = currentPerformanceData.processesNetworkReceive.Keys.ToList();
                        processes.Sort();
                        foreach (string s in processes)
                            processComboBox.Items.Add(s);
                        if (processComboBox.Items.Count == 0)
                        {
                            processComboBox.Text = "No processes available";
                            processComboBox.Enabled = false;
                        }
                        break;
                    }
            }

            processComboBox.Enabled = true;
        }

        private void processComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // clear chart
            foreach (var series in chart.Series)
            {
                series.Points.Clear();
            }

            // add data to chart
            switch (dataComboBox.SelectedIndex)
            {
                case (0):
                    {
                        cpuUsage = currentPerformanceData.processesCPUusage[(string)processComboBox.SelectedItem];
                        cpuUsageValuesNo = currentPerformanceData.processesCPUusageValuesNo[(string)processComboBox.SelectedItem];

                        if (man.baselineData.processesCPUusage.ContainsKey((string)processComboBox.SelectedItem))
                        {
                            cpuUsageBaseline = man.baselineData.processesCPUusage[(string)processComboBox.SelectedItem];
                            cpuUsageBaselineValuesNo = man.baselineData.processesCPUusageValuesNo[(string)processComboBox.SelectedItem];
                        }
                        else
                        {
                            cpuUsageBaseline = null;
                            cpuUsageBaselineValuesNo = null;
                        }

                        for (int i = 0; i < 2880; i++)
                        {
                            if (cpuUsageValuesNo[i] != 0)
                                chart.Series["Usage"].Points.AddXY(i, cpuUsage[i] / cpuUsageValuesNo[i]);
                            else
                                chart.Series["Usage"].Points.AddXY(i, 0);

                            if(cpuUsageBaseline != null && cpuUsageBaselineValuesNo != null && cpuUsageBaselineValuesNo[i] != 0 && baselineDatesCount > 0)
                                chart.Series["Baseline"].Points.AddXY(i, cpuUsageBaseline[i] / cpuUsageBaselineValuesNo[i] / baselineDatesCount);
                            else
                                chart.Series["Baseline"].Points.AddXY(i, 0);
                        }

                        break;
                    }
                case (1):
                    {
                        networkSend = currentPerformanceData.processesNetworkSend[(string)processComboBox.SelectedItem];

                        if (man.baselineData.processesNetworkSend.ContainsKey((string)processComboBox.SelectedItem))
                            networkSendBaseline = man.baselineData.processesNetworkSend[(string)processComboBox.SelectedItem];
                        else
                            networkSendBaseline = null;

                        for (int i = 0; i < 288; i++)
                        {
                            chart.Series["Usage"].Points.AddXY(i, networkSend[i]);

                            if (networkSendBaseline != null && baselineDatesCount > 0)
                                chart.Series["Baseline"].Points.AddXY(i, networkSendBaseline[i] / baselineDatesCount);
                            else
                                chart.Series["Baseline"].Points.AddXY(i, 0);
                        }

                        break;
                    }
                case (2):
                    {
                        networkReceive = currentPerformanceData.processesNetworkReceive[(string)processComboBox.SelectedItem];

                        if (man.baselineData.processesNetworkReceive.ContainsKey((string)processComboBox.SelectedItem))
                            networkReceiveBaseline = man.baselineData.processesNetworkReceive[(string)processComboBox.SelectedItem];
                        else
                            networkReceiveBaseline = null;

                        for (int i = 0; i < 288; i++)
                        {
                            chart.Series["Usage"].Points.AddXY(i, networkReceive[i]);

                            if (networkReceiveBaseline != null && baselineDatesCount > 0)
                                chart.Series["Baseline"].Points.AddXY(i, networkReceiveBaseline[i]);
                            else
                                chart.Series["Baseline"].Points.AddXY(i, 0);
                        }

                        break;
                    }
            }
        }

        private void dayComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataComboBox.Enabled = false;

            SetAnotherDateData(dayComboBox.SelectedIndex);

            dataComboBox.Enabled = true;
        }

        private void dayComboBox_Click(object sender, EventArgs e)
        {
            dataComboBox.Enabled = false;
            dataComboBox.SelectedIndex = -1;
            processComboBox.Enabled = false;
            processComboBox.SelectedIndex = -1;
        }

        private void dayComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
