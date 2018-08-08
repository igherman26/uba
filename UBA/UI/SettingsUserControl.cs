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
    public partial class SettingsUserControl : UserControl
    {
        private Manager man;

        public SettingsUserControl(Manager man)
        {
            InitializeComponent();

            this.man = man;

            int esr = man.GetEventsSamplingTime();
            int pcsr = man.GetPerformanceSamplingTime();

            if (esr != -1)
                eventsComboBox.Text = esr.ToString();
            else
                eventsComboBox.Enabled = false;
            if (pcsr != -1)
                pcComboBox.Text = pcsr.ToString();
            else
                pcComboBox.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (eventsComboBox.Enabled)
            {
                int esr = Int32.Parse((string)eventsComboBox.SelectedItem);
                man.SetEventsSamplingTime(esr);
            }
            if (pcComboBox.Enabled)
            {
                int pcsr = Int32.Parse((string)pcComboBox.SelectedItem);
                man.SetPerformanceSamplingTime(pcsr);
            }
        }

        private void eventsComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void pcComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
