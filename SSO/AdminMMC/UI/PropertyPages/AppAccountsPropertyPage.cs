using GT.BizTalk.SSO.AdminMMC.Management;
using GT.BizTalk.SSO.AdminMMC.UI.Controls;

namespace GT.BizTalk.SSO.AdminMMC.UI.PropertyPages
{
    public class AppAccountsPropertyPage : SSOPropertyPage<SSOAppInfo>
    {
        #region Fields
        private AppAccountsPropertyControl propertyControl;
        #endregion

        #region Constructor
        public AppAccountsPropertyPage()
        {
            this.Title = "Accounts";
            this.propertyControl = new AppAccountsPropertyControl();
            this.propertyControl.DataChanged += propertyControl_DataChanged;
            this.PropertyControl = this.propertyControl;
        }
        #endregion

        #region SSOPropertyPage Overrides
        public override void Refresh(SSOAppInfo appInfo)
        {
            base.Refresh(appInfo);

            // refresh UI
            this.propertyControl.UserAccounts = appInfo.UserAccounts;
            this.propertyControl.AdminAccounts = appInfo.AdminAccounts;
            this.propertyControl.AllowLocalAccounts = appInfo.AllowLocalAccounts;
            this.propertyControl.UseSSOAffiliateAdmins = appInfo.UseSSOAffiliateAdmins;
        }

        public override void Update(SSOAppInfo appInfo)
        {
            base.Update(appInfo);

            // update appInfo with new values from the UI
            appInfo.UserAccounts = this.propertyControl.UserAccounts;
            appInfo.AdminAccounts = this.propertyControl.AdminAccounts;
        }
        #endregion

        #region Event Handlers
        void propertyControl_DataChanged(object sender, System.EventArgs e)
        {
            this.Dirty = this.propertyControl.IsDirty();
        }
        #endregion
    }
}
