using GT.BizTalk.SSO.AdminMMC.Management;
using GT.BizTalk.SSO.AdminMMC.UI.Controls;

namespace GT.BizTalk.SSO.AdminMMC.UI.PropertyPages
{
    public class AppGeneralPropertyPage : SSOPropertyPage<SSOAppInfo>
    {
        #region Fields
        private AppGeneralPropertyControl propertyControl;
        #endregion

        #region Constructor
        public AppGeneralPropertyPage()
        {
            this.Title = "General";
            this.propertyControl = new AppGeneralPropertyControl();
            this.propertyControl.DataChanged += PropertyControl_DataChanged;
            this.PropertyControl = this.propertyControl;
        }
        #endregion

        #region SSOPropertyPage Overrides
        public override void Refresh(SSOAppInfo appInfo)
        {
            base.Refresh(appInfo);

            // refresh UI
            this.propertyControl.SetNewMode(false);
            this.propertyControl.AppName = appInfo.Name;
            this.propertyControl.AppDescription = appInfo.Description;
            this.propertyControl.AllowLocalAccounts = appInfo.AllowLocalAccounts;
            this.propertyControl.UseSSOAffiliateAdmins = appInfo.UseSSOAffiliateAdmins;
        }

        public override void Update(SSOAppInfo appInfo)
        {
            base.Update(appInfo);

            // update appInfo with new values from the UI
            appInfo.Description = this.propertyControl.AppDescription;
        }
        #endregion

        #region Event Handlers
        void PropertyControl_DataChanged(object sender, System.EventArgs e)
        {
            this.Dirty = this.propertyControl.IsDirty();
        }
        #endregion
    }
}
