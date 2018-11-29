using System;
using System.ComponentModel;
using System.Windows.Forms;

using Microsoft.EnterpriseSingleSignOn;

using GT.BizTalk.SSO.AdminMMC.Management;

namespace GT.BizTalk.SSO.AdminMMC.UI.Controls
{
    public partial class AppAccountsPropertyControl : SSOPropertyControl
    {
        #region Fields
        private AccountControl accountControlAppAdmins;
        private AccountControl accountControlAppUsers;
        private bool useSSOAffiliateAdmins;
        private string appAdminAccounts;
        private string appUserAccounts;
        #endregion

        #region Constructor
        public AppAccountsPropertyControl()
        {
            InitializeComponent();
            InitializeSSOComponents();
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public string AdminAccounts
        {
            get
            {
                return this.accountControlAppAdmins.GetAccounts();
            }
            set
            {
                this.appAdminAccounts = value;
                this.SetAccounts(this.accountControlAppAdmins, value);
            }
        }

        [Browsable(false)]
        public string UserAccounts
        {
            get
            {
                return this.accountControlAppUsers.GetAccounts();
            }
            set
            {
                this.appUserAccounts = value;
                this.SetAccounts(this.accountControlAppUsers, value);
            }
        }

        public bool AllowLocalAccounts
        {
            get
            {
                return this.accountControlAppAdmins.allowLocalAccounts;
            }
            set
            {
                this.accountControlAppAdmins.allowLocalAccounts = value;
                this.accountControlAppUsers.allowLocalAccounts = value;
            }
        }

        public bool UseSSOAffiliateAdmins
        {
            get
            {
                return this.useSSOAffiliateAdmins;
            }
            set
            {
                this.useSSOAffiliateAdmins = value;
                this.accountControlAppAdmins.Enabled = !this.useSSOAffiliateAdmins;
                if (this.useSSOAffiliateAdmins)
                {
                    this.accountControlAppAdmins.SetAccounts(SSOManager.SSOAffiliateAdminAccounts);
                }
            }
        }
        public int MinAccounts
        {
            get
            {
                return this.accountControlAppAdmins.minAccounts;
            }
            set
            {
                this.accountControlAppAdmins.minAccounts = value;
                this.accountControlAppUsers.minAccounts = value;
            }
        }
        #endregion

        #region SSOPropertyControl Implementation
        public override bool IsDirty()
        {
            return
                (this.appAdminAccounts != this.accountControlAppAdmins.GetAccounts())
                || (this.appUserAccounts != this.accountControlAppUsers.GetAccounts());
        }

        public override bool IsValid()
        {
            return
                (this.accountControlAppAdmins.listBoxAccounts.Items.Count >= this.MinAccounts)
                && (this.accountControlAppUsers.listBoxAccounts.Items.Count >= this.MinAccounts);
        }
        #endregion

        #region Event Handlers
        private void AccountControl_ChangedAccounts(int count)
        {
            this.OnDataChanged(EventArgs.Empty);
        }
        #endregion

        #region Helpers
        private void InitializeSSOComponents()
        {
            this.accountControlAppAdmins = new AccountControl();
            this.accountControlAppAdmins.Dock = DockStyle.Fill;
            this.accountControlAppAdmins.minAccounts = 1;
            this.accountControlAppAdmins.allowGroupAccounts = true;
            this.accountControlAppAdmins.allowLocalAccounts = true;
            this.accountControlAppAdmins.changedAccounts += new AccountControl.ChangedAccounts(this.AccountControl_ChangedAccounts);
            this.grpAppAdmins.Controls.Add(this.accountControlAppAdmins);

            this.accountControlAppUsers = new AccountControl();
            this.accountControlAppUsers.secondControl = true;
            this.accountControlAppUsers.Dock = DockStyle.Fill;
            this.accountControlAppUsers.minAccounts = 1;
            this.accountControlAppUsers.allowGroupAccounts = true;
            this.accountControlAppUsers.allowLocalAccounts = true;
            this.accountControlAppUsers.changedAccounts += new AccountControl.ChangedAccounts(this.AccountControl_ChangedAccounts);
            this.grpAppUsers.Controls.Add(this.accountControlAppUsers);
        }

        private void SetAccounts(AccountControl accountControl, string accounts)
        {
            if (string.IsNullOrEmpty(accounts) == false)
                accountControl.SetAccounts(accounts);
            else
                accountControl.listBoxAccounts.Items.Clear();
            accountControl.labelRequired.Visible = (accountControl.listBoxAccounts.Items.Count == 0);
        }
        #endregion
    }
}
