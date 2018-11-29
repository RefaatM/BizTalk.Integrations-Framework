using System;

using Microsoft.ManagementConsole;
using MMCAction = Microsoft.ManagementConsole.Action;

using GT.BizTalk.SSO.AdminMMC.Management;
using GT.BizTalk.SSO.AdminMMC.UI;
using GT.BizTalk.SSO.AdminMMC.UI.PropertyPages;

namespace GT.BizTalk.SSO.AdminMMC.UI.MMC
{
    public class AppScopeNode : SSOScopeNode
    {
        #region Constructors
        public AppScopeNode(string applicationName)
        {
            if (SSOAppSnapIn.HasSecurityRights)
            {
                this.DisplayName = applicationName;

                // create list view
                MmcListViewDescription listViewDescription = new MmcListViewDescription();
                listViewDescription.DisplayName = applicationName;
                listViewDescription.ViewType = typeof(AppFieldListView);
                listViewDescription.Options = MmcListViewOptions.SingleSelect;

                // attach it to the node
                this.ViewDescriptions.Add(listViewDescription);
                this.ViewDescriptions.DefaultIndex = 0;

                // enable verbs
                this.EnabledStandardVerbs |= StandardVerbs.Delete;
                this.EnabledStandardVerbs |= StandardVerbs.Properties;
                this.EnabledStandardVerbs |= StandardVerbs.Refresh;
            }
            else
            {
                this.DisplayName = applicationName + " - No Access Rights!";
                MmcListViewDescription listViewDescription = new MmcListViewDescription();
                listViewDescription.DisplayName = applicationName + " - No Access Rights!";
                listViewDescription.Options = MmcListViewOptions.HideSelection;
                this.ViewDescriptions.Add(listViewDescription);
            }
        }
        #endregion

        #region Implementation
        protected override void OnDelete(SyncStatus status)
        {
            GT.BizTalk.SSO.AdminMMC.UI.MsgBoxUtil.Show("Notification from AppScopeNode.OnDelete");
            try
            {
                if (MsgBoxUtil.Confirm(this.SnapIn, Properties.Resources.DeleteApplicationMessge) == true)
                {
                    SSOManager.DeleteApplication(this.DisplayName);
                    ((AppRootScopeNode)this.Parent).RefreshApps();
                }
                base.OnDelete(status);
            }
            catch (Exception ex)
            {
                MsgBoxUtil.Show(this.SnapIn, ex);
            }
        }

        protected override void OnAddPropertyPages(PropertyPageCollection propertyPageCollection)
        {
            if (string.IsNullOrEmpty(this.CurrentPropertySheetTitle) == true)
            {
                AppPropertyPageManager propPageManager = new AppPropertyPageManager(this.DisplayName);
                propPageManager.AddPropertyPages(propertyPageCollection);
            }
            base.OnAddPropertyPages(propertyPageCollection);
        }
        #endregion
    }
}
