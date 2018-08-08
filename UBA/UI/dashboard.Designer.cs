namespace UBA
{
    partial class Dashboard
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dashboard));
            this.sidepanel = new System.Windows.Forms.Panel();
            this.processesButton = new System.Windows.Forms.Button();
            this.SettingsButton = new System.Windows.Forms.Button();
            this.alertsButton = new System.Windows.Forms.Button();
            this.performanceButton = new System.Windows.Forms.Button();
            this.basicInfoButton = new System.Windows.Forms.Button();
            this.ipanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.headerpanel = new System.Windows.Forms.Panel();
            this.topLabel = new System.Windows.Forms.Label();
            this.containerPanel = new System.Windows.Forms.Panel();
            this.sidepanel.SuspendLayout();
            this.ipanel.SuspendLayout();
            this.headerpanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // sidepanel
            // 
            this.sidepanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(59)))), ((int)(((byte)(71)))));
            this.sidepanel.Controls.Add(this.processesButton);
            this.sidepanel.Controls.Add(this.SettingsButton);
            this.sidepanel.Controls.Add(this.alertsButton);
            this.sidepanel.Controls.Add(this.performanceButton);
            this.sidepanel.Controls.Add(this.basicInfoButton);
            this.sidepanel.Controls.Add(this.ipanel);
            this.sidepanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidepanel.Location = new System.Drawing.Point(0, 0);
            this.sidepanel.Name = "sidepanel";
            this.sidepanel.Size = new System.Drawing.Size(510, 1604);
            this.sidepanel.TabIndex = 0;
            // 
            // processesButton
            // 
            this.processesButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.processesButton.FlatAppearance.BorderSize = 0;
            this.processesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.processesButton.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processesButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(216)))), ((int)(((byte)(222)))));
            this.processesButton.Location = new System.Drawing.Point(0, 687);
            this.processesButton.Name = "processesButton";
            this.processesButton.Size = new System.Drawing.Size(510, 143);
            this.processesButton.TabIndex = 6;
            this.processesButton.Text = "Processes";
            this.processesButton.UseVisualStyleBackColor = true;
            this.processesButton.Click += new System.EventHandler(this.processesButton_Click);
            // 
            // SettingsButton
            // 
            this.SettingsButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.SettingsButton.FlatAppearance.BorderSize = 0;
            this.SettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsButton.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(216)))), ((int)(((byte)(222)))));
            this.SettingsButton.Location = new System.Drawing.Point(0, 1461);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(510, 143);
            this.SettingsButton.TabIndex = 5;
            this.SettingsButton.Text = "Settings";
            this.SettingsButton.UseVisualStyleBackColor = true;
            this.SettingsButton.Click += new System.EventHandler(this.SettingsButton_Click);
            // 
            // alertsButton
            // 
            this.alertsButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.alertsButton.FlatAppearance.BorderSize = 0;
            this.alertsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.alertsButton.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alertsButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(216)))), ((int)(((byte)(222)))));
            this.alertsButton.Location = new System.Drawing.Point(0, 544);
            this.alertsButton.Name = "alertsButton";
            this.alertsButton.Size = new System.Drawing.Size(510, 143);
            this.alertsButton.TabIndex = 4;
            this.alertsButton.Text = "Alerts";
            this.alertsButton.UseVisualStyleBackColor = true;
            this.alertsButton.Click += new System.EventHandler(this.alertsButton_Click);
            // 
            // performanceButton
            // 
            this.performanceButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.performanceButton.FlatAppearance.BorderSize = 0;
            this.performanceButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.performanceButton.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.performanceButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(216)))), ((int)(((byte)(222)))));
            this.performanceButton.Location = new System.Drawing.Point(0, 401);
            this.performanceButton.Name = "performanceButton";
            this.performanceButton.Size = new System.Drawing.Size(510, 143);
            this.performanceButton.TabIndex = 3;
            this.performanceButton.Text = "Performance";
            this.performanceButton.UseVisualStyleBackColor = true;
            this.performanceButton.Click += new System.EventHandler(this.performanceButton_Click);
            // 
            // basicInfoButton
            // 
            this.basicInfoButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.basicInfoButton.FlatAppearance.BorderSize = 0;
            this.basicInfoButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.basicInfoButton.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.basicInfoButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(216)))), ((int)(((byte)(222)))));
            this.basicInfoButton.Location = new System.Drawing.Point(0, 258);
            this.basicInfoButton.Name = "basicInfoButton";
            this.basicInfoButton.Size = new System.Drawing.Size(510, 143);
            this.basicInfoButton.TabIndex = 2;
            this.basicInfoButton.Text = "Basic info";
            this.basicInfoButton.UseVisualStyleBackColor = true;
            this.basicInfoButton.Click += new System.EventHandler(this.Button_Click);
            // 
            // ipanel
            // 
            this.ipanel.Controls.Add(this.label1);
            this.ipanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ipanel.Location = new System.Drawing.Point(0, 0);
            this.ipanel.Name = "ipanel";
            this.ipanel.Size = new System.Drawing.Size(510, 258);
            this.ipanel.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(216)))), ((int)(((byte)(222)))));
            this.label1.Location = new System.Drawing.Point(64, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(395, 193);
            this.label1.TabIndex = 0;
            this.label1.Text = "UBA";
            // 
            // headerpanel
            // 
            this.headerpanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(106)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.headerpanel.Controls.Add(this.topLabel);
            this.headerpanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerpanel.Location = new System.Drawing.Point(510, 0);
            this.headerpanel.Name = "headerpanel";
            this.headerpanel.Size = new System.Drawing.Size(2230, 258);
            this.headerpanel.TabIndex = 1;
            // 
            // topLabel
            // 
            this.topLabel.AutoSize = true;
            this.topLabel.Font = new System.Drawing.Font("Century Gothic", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.topLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(226)))), ((int)(((byte)(231)))));
            this.topLabel.Location = new System.Drawing.Point(70, 72);
            this.topLabel.Name = "topLabel";
            this.topLabel.Size = new System.Drawing.Size(700, 141);
            this.topLabel.TabIndex = 1;
            this.topLabel.Text = "Dashboard";
            // 
            // containerPanel
            // 
            this.containerPanel.AutoScroll = true;
            this.containerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containerPanel.Location = new System.Drawing.Point(510, 258);
            this.containerPanel.Name = "containerPanel";
            this.containerPanel.Size = new System.Drawing.Size(2230, 1346);
            this.containerPanel.TabIndex = 2;
            // 
            // Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(255)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(2740, 1604);
            this.Controls.Add(this.containerPanel);
            this.Controls.Add(this.headerpanel);
            this.Controls.Add(this.sidepanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Dashboard";
            this.Text = "UBA";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Dashboard_FormClosed);
            this.Load += new System.EventHandler(this.Dashboard_Load);
            this.sidepanel.ResumeLayout(false);
            this.ipanel.ResumeLayout(false);
            this.ipanel.PerformLayout();
            this.headerpanel.ResumeLayout(false);
            this.headerpanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel sidepanel;
        private System.Windows.Forms.Panel headerpanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button basicInfoButton;
        private System.Windows.Forms.Panel ipanel;
        private System.Windows.Forms.Label topLabel;
        private System.Windows.Forms.Button SettingsButton;
        private System.Windows.Forms.Button alertsButton;
        private System.Windows.Forms.Button performanceButton;
        private System.Windows.Forms.Panel containerPanel;
        private System.Windows.Forms.Button processesButton;
    }
}

