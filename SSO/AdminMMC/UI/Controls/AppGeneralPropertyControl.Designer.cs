namespace GT.BizTalk.SSO.AdminMMC.UI.Controls
{
    partial class AppGeneralPropertyControl
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
            this.components = new System.ComponentModel.Container();
            this.lblAppName = new System.Windows.Forms.Label();
            this.lblAppDescription = new System.Windows.Forms.Label();
            this.txtAppName = new System.Windows.Forms.TextBox();
            this.txtAppDescription = new System.Windows.Forms.TextBox();
            this.chkAllowLocalAccounts = new System.Windows.Forms.CheckBox();
            this.chkUseSSOAffiliateAdmins = new System.Windows.Forms.CheckBox();
            this.lblAppNameRequired = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblAppName
            // 
            this.lblAppName.AutoSize = true;
            this.lblAppName.Location = new System.Drawing.Point(29, 31);
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Size = new System.Drawing.Size(91, 13);
            this.lblAppName.TabIndex = 0;
            this.lblAppName.Text = "Application &name:";
            // 
            // lblAppDescription
            // 
            this.lblAppDescription.AutoSize = true;
            this.lblAppDescription.Location = new System.Drawing.Point(29, 63);
            this.lblAppDescription.Name = "lblAppDescription";
            this.lblAppDescription.Size = new System.Drawing.Size(63, 13);
            this.lblAppDescription.TabIndex = 2;
            this.lblAppDescription.Text = "&Description:";
            // 
            // txtAppName
            // 
            this.txtAppName.Location = new System.Drawing.Point(146, 28);
            this.txtAppName.Name = "txtAppName";
            this.txtAppName.Size = new System.Drawing.Size(314, 20);
            this.txtAppName.TabIndex = 1;
            this.txtAppName.TextChanged += new System.EventHandler(this.txtAppName_TextChanged);
            // 
            // txtAppDescription
            // 
            this.txtAppDescription.Location = new System.Drawing.Point(146, 60);
            this.txtAppDescription.Name = "txtAppDescription";
            this.txtAppDescription.Size = new System.Drawing.Size(314, 20);
            this.txtAppDescription.TabIndex = 3;
            this.txtAppDescription.TextChanged += new System.EventHandler(this.txtAppDescription_TextChanged);
            // 
            // chkAllowLocalAccounts
            // 
            this.chkAllowLocalAccounts.AutoSize = true;
            this.chkAllowLocalAccounts.Location = new System.Drawing.Point(29, 106);
            this.chkAllowLocalAccounts.Name = "chkAllowLocalAccounts";
            this.chkAllowLocalAccounts.Size = new System.Drawing.Size(227, 17);
            this.chkAllowLocalAccounts.TabIndex = 4;
            this.chkAllowLocalAccounts.Text = "Allow &Local accounts for Access accounts";
            this.chkAllowLocalAccounts.UseVisualStyleBackColor = true;
            this.chkAllowLocalAccounts.CheckedChanged += new System.EventHandler(this.chkAllowLocalAccounts_CheckedChanged);
            // 
            // chkUseSSOAffiliateAdmins
            // 
            this.chkUseSSOAffiliateAdmins.AutoSize = true;
            this.chkUseSSOAffiliateAdmins.Location = new System.Drawing.Point(29, 130);
            this.chkUseSSOAffiliateAdmins.Name = "chkUseSSOAffiliateAdmins";
            this.chkUseSSOAffiliateAdmins.Size = new System.Drawing.Size(335, 17);
            this.chkUseSSOAffiliateAdmins.TabIndex = 5;
            this.chkUseSSOAffiliateAdmins.Text = "Use &SSO Affiliate Admin accounts for Application Admin accounts";
            this.chkUseSSOAffiliateAdmins.UseVisualStyleBackColor = true;
            this.chkUseSSOAffiliateAdmins.CheckedChanged += new System.EventHandler(this.chkUseSSOAffiliateAdmins_CheckedChanged);
            // 
            // lblAppNameRequired
            // 
            this.lblAppNameRequired.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAppNameRequired.ForeColor = System.Drawing.Color.Red;
            this.lblAppNameRequired.Location = new System.Drawing.Point(122, 27);
            this.lblAppNameRequired.Name = "lblAppNameRequired";
            this.lblAppNameRequired.Size = new System.Drawing.Size(21, 18);
            this.lblAppNameRequired.TabIndex = 6;
            this.lblAppNameRequired.Text = "*";
            this.toolTip.SetToolTip(this.lblAppNameRequired, "The application name is required.");
            // 
            // AppGeneralPropertyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblAppNameRequired);
            this.Controls.Add(this.chkUseSSOAffiliateAdmins);
            this.Controls.Add(this.chkAllowLocalAccounts);
            this.Controls.Add(this.txtAppDescription);
            this.Controls.Add(this.txtAppName);
            this.Controls.Add(this.lblAppDescription);
            this.Controls.Add(this.lblAppName);
            this.Name = "AppGeneralPropertyControl";
            this.Size = new System.Drawing.Size(493, 182);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAppName;
        private System.Windows.Forms.Label lblAppDescription;
        private System.Windows.Forms.Label lblAppNameRequired;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TextBox txtAppName;
        private System.Windows.Forms.TextBox txtAppDescription;
        private System.Windows.Forms.CheckBox chkAllowLocalAccounts;
        private System.Windows.Forms.CheckBox chkUseSSOAffiliateAdmins;
    }
}
