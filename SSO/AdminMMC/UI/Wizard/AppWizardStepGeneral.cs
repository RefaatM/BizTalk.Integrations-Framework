using System.Windows.Forms;

using GT.BizTalk.SSO.AdminMMC.Management;

namespace GT.BizTalk.SSO.AdminMMC.UI.Wizard
{
    public partial class AppWizardStepGeneral : AppWizardStep
    {
        #region Contructor
        public AppWizardStepGeneral()
        {
            InitializeComponent();

            this.general.DataChanged += general_DataChanged;
        }
        #endregion

        #region AppWizardStep Overrides
        public override bool IsValid()
        {
            return this.general.IsValid();
        }

        public override void Refresh(SSOAppInfo appInfo)
        {
            this.general.AppName = appInfo.Name;
            this.general.AppDescription = appInfo.Description;
            this.general.AllowLocalAccounts = appInfo.AllowLocalAccounts;
            this.general.UseSSOAffiliateAdmins = appInfo.UseSSOAffiliateAdmins;
        }

        public override void Update(SSOAppInfo appInfo)
        {
            appInfo.Name = this.general.AppName;
            appInfo.Description = this.general.AppDescription;
            appInfo.AllowLocalAccounts = this.general.AllowLocalAccounts;
            appInfo.UseSSOAffiliateAdmins = this.general.UseSSOAffiliateAdmins;
        }
        #endregion

        #region Event Handlers
        void general_DataChanged(object sender, System.EventArgs e)
        {
            // forward DataChanged notification to subscribers of this control
            this.OnDataChanged(e);
        }
        #endregion
    }
}
