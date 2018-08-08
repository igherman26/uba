using PcapDotNet.Core;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UBA
{
    public partial class ManagerSetup : UserControl
    {
        private Manager man;

        public ManagerSetup()
        {
            InitializeComponent();

            // populate combobox
            foreach (string s in UserProfile.ExistingProfiles())
                profileComboBox.Items.Add(s);
            if (profileComboBox.Items.Count == 0)
            {
                profileComboBox.Text = "No profiles available";
                profileComboBox.Enabled = false;
            }

            // populate interfaces combobox
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;
            if (allDevices.Count == 0)
            {
                interfaceComboBox.Text = "No interfaces found! Make sure WinPcap is installed.";
                interfaceComboBox.Enabled = false;
            }
            for (int i = 0; i != allDevices.Count; ++i)
            {
                string interfaceText = "";
                LivePacketDevice device = allDevices[i];
                //interfaceText += device.Name;
                //interfaceText += "---";

                if (device.Description != null)
                    interfaceText += device.Description;
                else
                    interfaceText += "(No description available)";

                interfaceComboBox.Items.Add(interfaceText);
            }
        }

        private void profileComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void interfaceComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void monOptionComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (interfaceComboBox.SelectedIndex < 0 || monOptionComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Make your selections first.");
                return;
            }

            if (!(newProfileTextBox.Text.Length > 0) && profileComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Insert profile name.");
                return;
            }

            // create new profile if needed
            if (newProfileTextBox.Text.Length > 0)
            {
                try
                {
                    man = new Manager(newProfileTextBox.Text, (MonitorizationOptions)monOptionComboBox.SelectedIndex, interfaceComboBox.SelectedIndex);
                    man.InitManager();
                    Dashboard.SetManager(man);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                try
                {
                    man = new Manager((string)profileComboBox.SelectedItem, (MonitorizationOptions)monOptionComboBox.SelectedIndex, interfaceComboBox.SelectedIndex);
                    man.InitManager();
                    Dashboard.SetManager(man);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            this.Visible = false;
        }
    }
}
