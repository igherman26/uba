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
    public partial class AlertsUserControl : UserControl
    {
        private Manager man;
        private List<Alert> alerts;
        public AlertsUserControl(Manager man)
        {
            InitializeComponent();

            this.man = man;

            UpdateData();
        }

        private void UpdateData()
        {
            alerts = man.alerts_list;
            alerts.Reverse();

            int counter = 0;
            foreach (Alert a in alerts)
            {
                alertsDataGrid.Rows.Add(a.type, a.timestamp, a.description);

                if (a.read)
                {
                    alertsDataGrid.Rows[counter].Cells[0].Style.BackColor = Color.Gray;
                    alertsDataGrid.Rows[counter].Cells[1].Style.BackColor = Color.Gray;
                    alertsDataGrid.Rows[counter].Cells[2].Style.BackColor = Color.Gray;

                    counter++;
                    continue;
                }
                switch (a.type)
                {
                    case ALERT_TYPE.LOW:
                        alertsDataGrid.Rows[counter].Cells[0].Style.BackColor = Color.Yellow;
                        break;
                    case ALERT_TYPE.MED:
                        alertsDataGrid.Rows[counter].Cells[0].Style.BackColor = Color.Orange;
                        break;
                    case ALERT_TYPE.HIGH:
                        alertsDataGrid.Rows[counter].Cells[0].Style.BackColor = Color.Red;
                        break;
                }
                man.alerts_list[counter].read = true;
                counter++;
            }
        }

        private void alertsDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Event ev = alerts[e.RowIndex].e;
            if(ev.et == EventType.SINGLE)
            {
                SEvent se = (SEvent)ev;
                MessageBox.Show(se.ToString());
            }
            else
            {
                MEvent me = (MEvent)ev;
                MessageBox.Show(me.ToString());
            }
        }
    }
}
