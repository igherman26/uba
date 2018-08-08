using System;
using System.Windows.Forms;

namespace UBA
{
    public partial class Dashboard : Form
    {
        private static Manager man;
        private static ManagerSetup ms;
        private static BasicInfo bi;
        private static AlertsUserControl auc;
        private static PerformanceUserControl puc;
        private static ProcessesUserControl pruc;
        private static SettingsUserControl suc;

        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            ms = new ManagerSetup();

            try
            {
                containerPanel.Controls.Clear();
                containerPanel.Controls.Add(ms);
                ms.Show();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                Close();
            }
        }

        private void Dashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            // kill netstat if it was spawned
            while(! man.NetstatHasExited())
                man.KillNetstat();

            Environment.Exit(0);
        }

        //BasicInfoButton
        private void Button_Click(object sender, EventArgs e)
        {
            if(man == null)
            {
                MessageBox.Show("Start the monitorization first.");
                return;
            }

            bi = new BasicInfo(man);

            try
            {
                containerPanel.Controls.Clear();
                containerPanel.Controls.Add(bi);
                ms.Show();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                Close();
            }
        }

        public static Manager GetManager()
        {
            return man;
        }
        public static void SetManager(Manager m)
        {
            man = m;
        }

        private void alertsButton_Click(object sender, EventArgs e)
        {
            if (man == null)
            {
                MessageBox.Show("Start the monitorization first.");
                return;
            }

            auc = new AlertsUserControl(man);

            try
            {
                containerPanel.Controls.Clear();
                containerPanel.Controls.Add(auc);
                ms.Show();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                Close();
            }
        }

        private void performanceButton_Click(object sender, EventArgs e)
        {
            if (man == null)
            {
                MessageBox.Show("Start the monitorization first.");
                return;
            }

            puc = new PerformanceUserControl(man);

            try
            {
                containerPanel.Controls.Clear();
                containerPanel.Controls.Add(puc);
                ms.Show();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                Close();
            }
        }

        private void processesButton_Click(object sender, EventArgs e)
        {
            if (man == null)
            {
                MessageBox.Show("Start the monitorization first.");
                return;
            }

            pruc = new ProcessesUserControl(man);

            try
            {
                containerPanel.Controls.Clear();
                containerPanel.Controls.Add(pruc);
                ms.Show();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                Close();
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            if (man == null)
            {
                MessageBox.Show("Start the monitorization first.");
                return;
            }

            suc = new SettingsUserControl(man);

            try
            {
                containerPanel.Controls.Clear();
                containerPanel.Controls.Add(suc);
                ms.Show();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                Close();
            }
        }
    }
}
