namespace UBA
{
    partial class ManagerSetup
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
            this.profileComboBox = new System.Windows.Forms.ComboBox();
            this.interfaceComboBox = new System.Windows.Forms.ComboBox();
            this.monOptionComboBox = new System.Windows.Forms.ComboBox();
            this.newProfileTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.newProfileLabel = new System.Windows.Forms.Label();
            this.optionLabel = new System.Windows.Forms.Label();
            this.interfaceLabel = new System.Windows.Forms.Label();
            this.profileLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // profileComboBox
            // 
            this.profileComboBox.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.profileComboBox.FormattingEnabled = true;
            this.profileComboBox.Location = new System.Drawing.Point(1092, 65);
            this.profileComboBox.Name = "profileComboBox";
            this.profileComboBox.Size = new System.Drawing.Size(969, 89);
            this.profileComboBox.TabIndex = 19;
            this.profileComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.profileComboBox_KeyPress);
            // 
            // interfaceComboBox
            // 
            this.interfaceComboBox.Font = new System.Drawing.Font("Century Gothic", 9.900001F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.interfaceComboBox.FormattingEnabled = true;
            this.interfaceComboBox.Location = new System.Drawing.Point(1092, 210);
            this.interfaceComboBox.Name = "interfaceComboBox";
            this.interfaceComboBox.Size = new System.Drawing.Size(969, 48);
            this.interfaceComboBox.TabIndex = 18;
            this.interfaceComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.interfaceComboBox_KeyPress);
            // 
            // monOptionComboBox
            // 
            this.monOptionComboBox.Font = new System.Drawing.Font("Century Gothic", 9.900001F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monOptionComboBox.FormattingEnabled = true;
            this.monOptionComboBox.Items.AddRange(new object[] {
            "All",
            "Events only",
            "Performance only",
            "Network only",
            "Events and performance",
            "Events and network",
            "Performance and network"});
            this.monOptionComboBox.Location = new System.Drawing.Point(1092, 322);
            this.monOptionComboBox.Name = "monOptionComboBox";
            this.monOptionComboBox.Size = new System.Drawing.Size(969, 48);
            this.monOptionComboBox.TabIndex = 17;
            this.monOptionComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.monOptionComboBox_KeyPress);
            // 
            // newProfileTextBox
            // 
            this.newProfileTextBox.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newProfileTextBox.Location = new System.Drawing.Point(1092, 589);
            this.newProfileTextBox.Name = "newProfileTextBox";
            this.newProfileTextBox.Size = new System.Drawing.Size(969, 90);
            this.newProfileTextBox.TabIndex = 16;
            this.newProfileTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(106)))), ((int)(((byte)(98)))), ((int)(((byte)(98)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(226)))), ((int)(((byte)(231)))));
            this.button1.Location = new System.Drawing.Point(1102, 807);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(969, 123);
            this.button1.TabIndex = 15;
            this.button1.Text = "Start monitorization";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // newProfileLabel
            // 
            this.newProfileLabel.AutoSize = true;
            this.newProfileLabel.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newProfileLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(172)))), ((int)(((byte)(146)))));
            this.newProfileLabel.Location = new System.Drawing.Point(96, 589);
            this.newProfileLabel.Name = "newProfileLabel";
            this.newProfileLabel.Size = new System.Drawing.Size(408, 81);
            this.newProfileLabel.TabIndex = 14;
            this.newProfileLabel.Text = "New profile";
            // 
            // optionLabel
            // 
            this.optionLabel.AutoSize = true;
            this.optionLabel.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(172)))), ((int)(((byte)(146)))));
            this.optionLabel.Location = new System.Drawing.Point(96, 303);
            this.optionLabel.Name = "optionLabel";
            this.optionLabel.Size = new System.Drawing.Size(950, 81);
            this.optionLabel.TabIndex = 13;
            this.optionLabel.Text = "Select monitorization option";
            // 
            // interfaceLabel
            // 
            this.interfaceLabel.AutoSize = true;
            this.interfaceLabel.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.interfaceLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(172)))), ((int)(((byte)(146)))));
            this.interfaceLabel.Location = new System.Drawing.Point(96, 181);
            this.interfaceLabel.Name = "interfaceLabel";
            this.interfaceLabel.Size = new System.Drawing.Size(552, 81);
            this.interfaceLabel.TabIndex = 12;
            this.interfaceLabel.Text = "Select interface";
            // 
            // profileLabel
            // 
            this.profileLabel.AutoSize = true;
            this.profileLabel.Font = new System.Drawing.Font("Century Gothic", 20.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.profileLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(172)))), ((int)(((byte)(146)))));
            this.profileLabel.Location = new System.Drawing.Point(96, 65);
            this.profileLabel.Name = "profileLabel";
            this.profileLabel.Size = new System.Drawing.Size(463, 81);
            this.profileLabel.TabIndex = 11;
            this.profileLabel.Text = "Select profile";
            // 
            // ManagerSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(255)))), ((int)(((byte)(252)))));
            this.Controls.Add(this.profileComboBox);
            this.Controls.Add(this.interfaceComboBox);
            this.Controls.Add(this.monOptionComboBox);
            this.Controls.Add(this.newProfileTextBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.newProfileLabel);
            this.Controls.Add(this.optionLabel);
            this.Controls.Add(this.interfaceLabel);
            this.Controls.Add(this.profileLabel);
            this.Name = "ManagerSetup";
            this.Size = new System.Drawing.Size(2100, 970);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox profileComboBox;
        private System.Windows.Forms.ComboBox interfaceComboBox;
        private System.Windows.Forms.ComboBox monOptionComboBox;
        private System.Windows.Forms.TextBox newProfileTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label newProfileLabel;
        private System.Windows.Forms.Label optionLabel;
        private System.Windows.Forms.Label interfaceLabel;
        private System.Windows.Forms.Label profileLabel;
    }
}
