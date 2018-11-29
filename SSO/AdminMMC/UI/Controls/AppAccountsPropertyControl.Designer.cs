namespace GT.BizTalk.SSO.AdminMMC.UI.Controls
{
    partial class AppAccountsPropertyControl
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
            this.grpAppAdmins = new System.Windows.Forms.GroupBox();
            this.grpAppUsers = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // grpAppAdmins
            // 
            this.grpAppAdmins.Location = new System.Drawing.Point(19, 16);
            this.grpAppAdmins.Name = "grpAppAdmins";
            this.grpAppAdmins.Size = new System.Drawing.Size(531, 132);
            this.grpAppAdmins.TabIndex = 0;
            this.grpAppAdmins.TabStop = false;
            this.grpAppAdmins.Text = "Application &Administrators";
            // 
            // grpAppUsers
            // 
            this.grpAppUsers.Location = new System.Drawing.Point(19, 164);
            this.grpAppUsers.Name = "grpAppUsers";
            this.grpAppUsers.Size = new System.Drawing.Size(531, 132);
            this.grpAppUsers.TabIndex = 1;
            this.grpAppUsers.TabStop = false;
            this.grpAppUsers.Text = "Application &Users";
            // 
            // AppAccountsPropertyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpAppUsers);
            this.Controls.Add(this.grpAppAdmins);
            this.Name = "AppAccountsPropertyControl";
            this.Size = new System.Drawing.Size(570, 315);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpAppAdmins;
        private System.Windows.Forms.GroupBox grpAppUsers;
    }
}
