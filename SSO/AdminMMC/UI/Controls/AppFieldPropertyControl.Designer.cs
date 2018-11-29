namespace GT.BizTalk.SSO.AdminMMC.UI.Controls
{
    partial class AppFieldPropertyControl
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
            this.configSettingInfo = new System.Windows.Forms.GroupBox();
            this.txtIdentifier = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.lblKey = new System.Windows.Forms.Label();
            this.configSettingInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // configSettingInfo
            // 
            this.configSettingInfo.Controls.Add(this.txtIdentifier);
            this.configSettingInfo.Controls.Add(this.label2);
            this.configSettingInfo.Controls.Add(this.txtValue);
            this.configSettingInfo.Controls.Add(this.label1);
            this.configSettingInfo.Controls.Add(this.txtKey);
            this.configSettingInfo.Controls.Add(this.lblKey);
            this.configSettingInfo.Location = new System.Drawing.Point(21, 26);
            this.configSettingInfo.Name = "configSettingInfo";
            this.configSettingInfo.Size = new System.Drawing.Size(546, 145);
            this.configSettingInfo.TabIndex = 0;
            this.configSettingInfo.TabStop = false;
            this.configSettingInfo.Text = "&Config Setting";
            // 
            // txtIdentifier
            // 
            this.txtIdentifier.Enabled = false;
            this.txtIdentifier.Location = new System.Drawing.Point(91, 103);
            this.txtIdentifier.Name = "txtIdentifier";
            this.txtIdentifier.Size = new System.Drawing.Size(425, 20);
            this.txtIdentifier.TabIndex = 6;
            this.txtIdentifier.TextChanged += new System.EventHandler(this.txtIdentifier_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "&Identifier:";
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(91, 66);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(425, 20);
            this.txtValue.TabIndex = 4;
            this.txtValue.TextChanged += new System.EventHandler(this.txtValue_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "&Value";
            // 
            // txtKey
            // 
            this.txtKey.Location = new System.Drawing.Point(91, 30);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(425, 20);
            this.txtKey.TabIndex = 2;
            this.txtKey.TextChanged += new System.EventHandler(this.txtKey_TextChanged);
            // 
            // lblKey
            // 
            this.lblKey.AutoSize = true;
            this.lblKey.Location = new System.Drawing.Point(26, 33);
            this.lblKey.Name = "lblKey";
            this.lblKey.Size = new System.Drawing.Size(28, 13);
            this.lblKey.TabIndex = 1;
            this.lblKey.Text = "&Key:";
            // 
            // AppFieldPropertyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.configSettingInfo);
            this.Name = "AppFieldPropertyControl";
            this.Size = new System.Drawing.Size(597, 200);
            this.configSettingInfo.ResumeLayout(false);
            this.configSettingInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox configSettingInfo;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.TextBox txtIdentifier;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label label1;
    }
}
