using System;

using Microsoft.ManagementConsole;

using GT.BizTalk.SSO.AdminMMC.Management;

namespace GT.BizTalk.SSO.AdminMMC.UI.PropertyPages
{
    public class AppPropertyPageManager
    {
        #region Fields
        private SSOAppInfo appInfo;
        private AppGeneralPropertyPage generalPropPage;
        private AppAccountsPropertyPage accountsPropPage;
        #endregion

        #region Constructor
        public AppPropertyPageManager(string appName)
        {
            // load app info from SSO
            this.appInfo = SSOManager.GetApplicationInfo(appName);

            // initialize property pages
            // general
            this.generalPropPage = new AppGeneralPropertyPage();
            this.generalPropPage.Load += PropertyPage_Load;
            this.generalPropPage.Save += PropertyPage_Save;
            // accounts
            this.accountsPropPage = new AppAccountsPropertyPage();
        }
        #endregion

        #region Methods
        public void AddPropertyPages(PropertyPageCollection propPageCollection)
        {
            propPageCollection.Add(this.generalPropPage);
            propPageCollection.Add(this.accountsPropPage);
        }
        #endregion

        #region Event Handlers
        private void PropertyPage_Load(object sender, EventArgs e)
        {
            // refresh both pages
            this.generalPropPage.Refresh(this.appInfo);
            this.accountsPropPage.Refresh(this.appInfo);
        }

        private void PropertyPage_Save(object sender, ResultEventArgs<bool> e)
        {
            if (this.generalPropPage.IsValid() == false || this.accountsPropPage.IsValid() == false)
            {
                MsgBoxUtil.Show(this.generalPropPage.ParentSheet, "Some information is missing or incorrect. Please review and correct the information entered on each page.");
                e.Result = false;
            }
            else
            {
                if (this.generalPropPage.IsDirty() == true || this.accountsPropPage.IsDirty() == true)
                {
                    // load app info from SSO
                    this.appInfo = SSOManager.GetApplicationInfo(this.appInfo.Name);
                    // update it with new information from property pages
                    this.generalPropPage.Update(this.appInfo);
                    this.accountsPropPage.Update(this.appInfo);
                    // save changes into SSO
                    SSOManager.UpdateApplicationInfo(this.appInfo);

                    // notify subscribers the application has been saved
                    this.OnSaved(EventArgs.Empty);
                }
                e.Result = true;
            }
        }
        #endregion

        #region Events
        public event EventHandler Saved;

        protected void OnSaved(EventArgs e)
        {
            if (this.Saved != null)
                this.Saved(this, e);
        }
        #endregion
    }
}
