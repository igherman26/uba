namespace UBA
{
    partial class ProcessesUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.dataComboBox = new System.Windows.Forms.ComboBox();
            this.processComboBox = new System.Windows.Forms.ComboBox();
            this.processLabel = new System.Windows.Forms.Label();
            this.dataLabel = new System.Windows.Forms.Label();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dayComboBox = new System.Windows.Forms.ComboBox();
            this.dayLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.SuspendLayout();
            // 
            // dataComboBox
            // 
            this.dataComboBox.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataComboBox.FormattingEnabled = true;
            this.dataComboBox.Items.AddRange(new object[] {
            "CPU Usage",
            "Network Send",
            "Network Receive"});
            this.dataComboBox.Location = new System.Drawing.Point(1066, 149);
            this.dataComboBox.Name = "dataComboBox";
            this.dataComboBox.Size = new System.Drawing.Size(969, 89);
            this.dataComboBox.TabIndex = 23;
            this.dataComboBox.SelectedIndexChanged += new System.EventHandler(this.profileComboBox_SelectedIndexChanged);
            // 
            // processComboBox
            // 
            this.processComboBox.Font = new System.Drawing.Font("Century Gothic", 9.900001F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processComboBox.FormattingEnabled = true;
            this.processComboBox.Location = new System.Drawing.Point(1066, 294);
            this.processComboBox.Name = "processComboBox";
            this.processComboBox.Size = new System.Drawing.Size(969, 48);
            this.processComboBox.TabIndex = 22;
            this.processComboBox.SelectedIndexChanged += new System.EventHandler(this.processComboBox_SelectedIndexChanged);
            // 
            // processLabel
            // 
            this.processLabel.AutoSize = true;
            this.processLabel.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(172)))), ((int)(((byte)(146)))));
            this.processLabel.Location = new System.Drawing.Point(70, 265);
            this.processLabel.Name = "processLabel";
            this.processLabel.Size = new System.Drawing.Size(535, 81);
            this.processLabel.TabIndex = 21;
            this.processLabel.Text = "Select process:";
            // 
            // dataLabel
            // 
            this.dataLabel.AutoSize = true;
            this.dataLabel.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(172)))), ((int)(((byte)(146)))));
            this.dataLabel.Location = new System.Drawing.Point(70, 149);
            this.dataLabel.Name = "dataLabel";
            this.dataLabel.Size = new System.Drawing.Size(431, 81);
            this.dataLabel.TabIndex = 20;
            this.dataLabel.Text = "Select data:";
            // 
            // chart
            // 
            this.chart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(255)))), ((int)(((byte)(252)))));
            chartArea1.AxisX.LabelStyle.ForeColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.MajorTickMark.Enabled = false;
            chartArea1.AxisX.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.IsMarginVisible = false;
            chartArea1.AxisY.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorTickMark.Enabled = false;
            chartArea1.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea1);
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chart.Legends.Add(legend1);
            this.chart.Location = new System.Drawing.Point(84, 411);
            this.chart.Name = "chart";
            this.chart.PaletteCustomColors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(59)))), ((int)(((byte)(71)))))};
            series1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(59)))), ((int)(((byte)(71)))));
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Area;
            series1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(216)))), ((int)(((byte)(222)))));
            series1.IsVisibleInLegend = false;
            series1.IsXValueIndexed = true;
            series1.Legend = "Legend1";
            series1.Name = "Usage";
            series2.BorderColor = System.Drawing.Color.Red;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            series2.Legend = "Legend1";
            series2.Name = "Baseline";
            this.chart.Series.Add(series1);
            this.chart.Series.Add(series2);
            this.chart.Size = new System.Drawing.Size(1948, 696);
            this.chart.TabIndex = 24;
            this.chart.Text = "Usages";
            // 
            // dayComboBox
            // 
            this.dayComboBox.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dayComboBox.FormattingEnabled = true;
            this.dayComboBox.Location = new System.Drawing.Point(1066, 21);
            this.dayComboBox.Name = "dayComboBox";
            this.dayComboBox.Size = new System.Drawing.Size(969, 89);
            this.dayComboBox.TabIndex = 36;
            this.dayComboBox.SelectedIndexChanged += new System.EventHandler(this.dayComboBox_SelectedIndexChanged);
            this.dayComboBox.Click += new System.EventHandler(this.dayComboBox_Click);
            this.dayComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dayComboBox_KeyPress);
            // 
            // dayLabel
            // 
            this.dayLabel.AutoSize = true;
            this.dayLabel.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dayLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(172)))), ((int)(((byte)(146)))));
            this.dayLabel.Location = new System.Drawing.Point(73, 25);
            this.dayLabel.Name = "dayLabel";
            this.dayLabel.Size = new System.Drawing.Size(405, 81);
            this.dayLabel.TabIndex = 35;
            this.dayLabel.Text = "Select day:";
            // 
            // ProcessesUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(255)))), ((int)(((byte)(252)))));
            this.Controls.Add(this.dayComboBox);
            this.Controls.Add(this.dayLabel);
            this.Controls.Add(this.chart);
            this.Controls.Add(this.dataComboBox);
            this.Controls.Add(this.processComboBox);
            this.Controls.Add(this.processLabel);
            this.Controls.Add(this.dataLabel);
            this.Name = "ProcessesUserControl";
            this.Size = new System.Drawing.Size(2100, 1150);
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox dataComboBox;
        private System.Windows.Forms.ComboBox processComboBox;
        private System.Windows.Forms.Label processLabel;
        private System.Windows.Forms.Label dataLabel;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.ComboBox dayComboBox;
        private System.Windows.Forms.Label dayLabel;
    }
}
