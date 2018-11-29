using System;

namespace GT.BizTalk.SSO.AdminMMC.UI.Controls
{
    public partial class AppGeneralPropertyControl : SSOPropertyControl
    {
        #region Fields
        private string appName;
        private string appDescription;
        private bool allowLocalAccounts;
        private bool useSSOAffiliateAdmins;
        #endregion

        #region Constructor
        public AppGeneralPropertyControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        public string AppName
        {
            get
            {
                return this.txtAppName.Text.Trim();
            }
            set
            {
                if (this.txtAppName.Text.Trim() != value)
                {
                    this.txtAppName.Text = value;
                    this.appName = value;
                    this.SetRequiredIndicators();
                }
            }
        }

        public string AppDescription
        {
            get
            {
                return this.txtAppDescription.Text.Trim();
            }
            set
            {
                if (this.txtAppDescription.Text.Trim() != value)
                {
                    this.txtAppDescription.Text = value;
                    this.appDescription = value;
                }
            }
        }

        public bool AllowLocalAccounts
        {
            get
            {
                return this.chkAllowLocalAccounts.Checked;
            }
            set
            {
                if (this.chkAllowLocalAccounts.Checked != value)
                {
                    this.chkAllowLocalAccounts.Checked = value;
                    this.allowLocalAccounts = value;
                }
            }
        }

        public bool UseSSOAffiliateAdmins
        {
            get
            {
                return this.chkUseSSOAffiliateAdmins.Checked;
            }
            set
            {
                if (this.chkUseSSOAffiliateAdmins.Checked != value)
                {
                    this.chkUseSSOAffiliateAdmins.Checked = value;
                    this.useSSOAffiliateAdmins = value;
                }
            }
        }
        #endregion

        #region SSOPropertyControl Implementation
        public override bool IsDirty()
        {
            return
                (this.appName != this.txtAppName.Text.Trim())
                || (this.appDescription != this.txtAppDescription.Text.Trim())
                || (this.allowLocalAccounts != this.chkAllowLocalAccounts.Checked)
                || (this.useSSOAffiliateAdmins != this.chkUseSSOAffiliateAdmins.Checked);
        }

        public override bool IsValid()
        {
            return (this.txtAppName.Text.Trim().Length > 0);
        }
        #endregion

        #region Methods
        public void SetNewMode(bool newApp)
        {
            this.lblAppName.Enabled = newApp;
            this.txtAppName.Enabled = newApp;
            this.chkAllowLocalAccounts.Enabled = newApp;
            this.chkUseSSOAffiliateAdmins.Enabled = newApp;
        }
        #endregion

        #region Event Handlers
        private void txtAppName_TextChanged(object sender, EventArgs e)
        {
            this.SetRequiredIndicators();
            this.OnDataChanged(e);
        }

        private void txtAppDescription_TextChanged(object sender, EventArgs e)
        {
            this.OnDataChanged(e);
        }

        private void chkAllowLocalAccounts_CheckedChanged(object sender, EventArgs e)
        {
            this.OnDataChanged(e);
        }

        private void chkUseSSOAffiliateAdmins_CheckedChanged(object sender, EventArgs e)
        {
            this.OnDataChanged(e);
        }
        #endregion

        #region Helpers
        private void SetRequiredIndicators()
        {
            this.lblAppNameRequired.Visible = !this.IsValid();
        }
        #endregion
    }
}
