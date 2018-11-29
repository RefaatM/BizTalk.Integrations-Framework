using System;
using System.ComponentModel;
using System.Drawing;
namespace GT.BizTalk.SSO.AdminMMC.UI.Wizard
{
    partial class AppWizardStepWelcome
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.picWizard = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picWizard)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(194, 29);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(288, 20);
            this.lblTitle.TabIndex = 14;
            this.lblTitle.Text = "Welcome to the Application Wizard";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(195, 76);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(366, 13);
            this.lblInfo.TabIndex = 15;
            this.lblInfo.Text = "This wizard will guide you through the creation of a configuration application.";
            // 
            // picWizard
            // 
            this.picWizard.Image = global::GT.BizTalk.SSO.AdminMMC.Properties.Resources.AppWizardLargeBitmap;
            this.picWizard.InitialImage = global::GT.BizTalk.SSO.AdminMMC.Properties.Resources.AppWizardLargeBitmap;
            this.picWizard.Location = new System.Drawing.Point(0, 0);
            this.picWizard.Name = "picWizard";
            this.picWizard.Size = new System.Drawing.Size(164, 367);
            this.picWizard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picWizard.TabIndex = 13;
            this.picWizard.TabStop = false;
            // 
            // AppWizardStepWelcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.picWizard);
            this.Name = "AppWizardStepWelcome";
            this.Size = new System.Drawing.Size(600, 370);
            ((System.ComponentModel.ISupportInitialize)(this.picWizard)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picWizard;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblInfo;
    }
}
