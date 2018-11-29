namespace GT.BizTalk.SSO.AdminMMC.UI.Wizard
{
    partial class AppWizardStepAccounts
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.picHeader = new System.Windows.Forms.PictureBox();
            this.lblStepInfo = new System.Windows.Forms.Label();
            this.lblStepTitle = new System.Windows.Forms.Label();
            this.accounts = new GT.BizTalk.SSO.AdminMMC.UI.Controls.AppAccountsPropertyControl();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHeader)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pnlHeader.Controls.Add(this.picHeader);
            this.pnlHeader.Controls.Add(this.lblStepInfo);
            this.pnlHeader.Controls.Add(this.lblStepTitle);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(600, 57);
            this.pnlHeader.TabIndex = 1;
            // 
            // picHeader
            // 
            this.picHeader.Image = global::GT.BizTalk.SSO.AdminMMC.Properties.Resources.AppWizardSmallBitmap;
            this.picHeader.InitialImage = global::GT.BizTalk.SSO.AdminMMC.Properties.Resources.AppWizardSmallBitmap;
            this.picHeader.Location = new System.Drawing.Point(549, 3);
            this.picHeader.Name = "picHeader";
            this.picHeader.Size = new System.Drawing.Size(48, 48);
            this.picHeader.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picHeader.TabIndex = 4;
            this.picHeader.TabStop = false;
            // 
            // lblStepInfo
            // 
            this.lblStepInfo.AutoSize = true;
            this.lblStepInfo.Location = new System.Drawing.Point(29, 30);
            this.lblStepInfo.Name = "lblStepInfo";
            this.lblStepInfo.Size = new System.Drawing.Size(298, 13);
            this.lblStepInfo.TabIndex = 3;
            this.lblStepInfo.Text = "Specify the access accounts for the configuration application.";
            // 
            // lblStepTitle
            // 
            this.lblStepTitle.AutoSize = true;
            this.lblStepTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStepTitle.Location = new System.Drawing.Point(14, 13);
            this.lblStepTitle.Name = "lblStepTitle";
            this.lblStepTitle.Size = new System.Drawing.Size(60, 13);
            this.lblStepTitle.TabIndex = 2;
            this.lblStepTitle.Text = "Accounts";
            // 
            // accounts
            // 
            this.accounts.AdminAccounts = "";
            this.accounts.AllowLocalAccounts = true;
            this.accounts.Location = new System.Drawing.Point(2, 59);
            this.accounts.MinAccounts = 1;
            this.accounts.Name = "accounts";
            this.accounts.Size = new System.Drawing.Size(570, 315);
            this.accounts.TabIndex = 2;
            this.accounts.UserAccounts = "";
            this.accounts.UseSSOAffiliateAdmins = false;
            // 
            // AppWizardStepAccounts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.accounts);
            this.Controls.Add(this.pnlHeader);
            this.Name = "AppWizardStepAccounts";
            this.Size = new System.Drawing.Size(600, 370);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picHeader)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.PictureBox picHeader;
        private System.Windows.Forms.Label lblStepInfo;
        private System.Windows.Forms.Label lblStepTitle;
        private GT.BizTalk.SSO.AdminMMC.UI.Controls.AppAccountsPropertyControl accounts;
    }
}
