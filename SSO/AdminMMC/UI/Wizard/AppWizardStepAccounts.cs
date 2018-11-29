using System.Windows.Forms;

using GT.BizTalk.SSO.AdminMMC.Management;

namespace GT.BizTalk.SSO.AdminMMC.UI.Wizard
{
    public partial class AppWizardStepAccounts : AppWizardStep
    {
        #region Constructor
        public AppWizardStepAccounts()
        {
            InitializeComponent();

            this.accounts.DataChanged += accounts_DataChanged;
        }
        #endregion

        #region AppWizardStep Overrides
        public override bool IsValid()
        {
            return this.accounts.IsValid();
        }

        public override void Refresh(SSOAppInfo appInfo)
        {
            this.accounts.UserAccounts = appInfo.UserAccounts;
            this.accounts.AdminAccounts = appInfo.AdminAccounts;
            this.accounts.AllowLocalAccounts = appInfo.AllowLocalAccounts;
            this.accounts.UseSSOAffiliateAdmins = appInfo.UseSSOAffiliateAdmins;
        }

        public override void Update(SSOAppInfo appInfo)
        {
            appInfo.UserAccounts = this.accounts.UserAccounts;
            appInfo.AdminAccounts = this.accounts.AdminAccounts;
        }
        #endregion

        #region Event Handlers
        void accounts_DataChanged(object sender, System.EventArgs e)
        {
            // forward DataChanged notification to subscribers of this control
            this.OnDataChanged(e);
        }
        #endregion
    }
}
