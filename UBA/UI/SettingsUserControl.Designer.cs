namespace UBA
{
    partial class SettingsUserControl
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
            this.button1 = new System.Windows.Forms.Button();
            this.performanceLabel = new System.Windows.Forms.Label();
            this.eventsLabel = new System.Windows.Forms.Label();
            this.eventsComboBox = new System.Windows.Forms.ComboBox();
            this.pcComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(106)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(226)))), ((int)(((byte)(231)))));
            this.button1.Location = new System.Drawing.Point(1069, 795);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(969, 123);
            this.button1.TabIndex = 24;
            this.button1.Text = "Set values";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // performanceLabel
            // 
            this.performanceLabel.AutoSize = true;
            this.performanceLabel.Font = new System.Drawing.Font("Century Gothic", 14.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.performanceLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(172)))), ((int)(((byte)(146)))));
            this.performanceLabel.Location = new System.Drawing.Point(66, 183);
            this.performanceLabel.Name = "performanceLabel";
            this.performanceLabel.Size = new System.Drawing.Size(835, 56);
            this.performanceLabel.TabIndex = 21;
            this.performanceLabel.Text = "Performance Counters sample rate";
            // 
            // eventsLabel
            // 
            this.eventsLabel.AutoSize = true;
            this.eventsLabel.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.eventsLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(172)))), ((int)(((byte)(146)))));
            this.eventsLabel.Location = new System.Drawing.Point(63, 53);
            this.eventsLabel.Name = "eventsLabel";
            this.eventsLabel.Size = new System.Drawing.Size(656, 81);
            this.eventsLabel.TabIndex = 20;
            this.eventsLabel.Text = "Events sample rate";
            // 
            // eventsComboBox
            // 
            this.eventsComboBox.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.eventsComboBox.FormattingEnabled = true;
            this.eventsComboBox.Items.AddRange(new object[] {
            "1000",
            "2000",
            "3000",
            "4000",
            "5000",
            "10000"});
            this.eventsComboBox.Location = new System.Drawing.Point(1069, 53);
            this.eventsComboBox.Name = "eventsComboBox";
            this.eventsComboBox.Size = new System.Drawing.Size(969, 89);
            this.eventsComboBox.TabIndex = 25;
            this.eventsComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.eventsComboBox_KeyPress);
            // 
            // pcComboBox
            // 
            this.pcComboBox.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pcComboBox.FormattingEnabled = true;
            this.pcComboBox.Items.AddRange(new object[] {
            "500",
            "1000",
            "2000",
            "3000",
            "4000",
            "5000",
            "10000"});
            this.pcComboBox.Location = new System.Drawing.Point(1069, 160);
            this.pcComboBox.Name = "pcComboBox";
            this.pcComboBox.Size = new System.Drawing.Size(969, 89);
            this.pcComboBox.TabIndex = 26;
            this.pcComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.pcComboBox_KeyPress);
            // 
            // SettingsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(255)))), ((int)(((byte)(252)))));
            this.Controls.Add(this.pcComboBox);
            this.Controls.Add(this.eventsComboBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.performanceLabel);
            this.Controls.Add(this.eventsLabel);
            this.Name = "SettingsUserControl";
            this.Size = new System.Drawing.Size(2100, 970);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label performanceLabel;
        private System.Windows.Forms.Label eventsLabel;
        private System.Windows.Forms.ComboBox eventsComboBox;
        private System.Windows.Forms.ComboBox pcComboBox;
    }
}
