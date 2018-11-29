using System;
using System.Collections.Generic;

using Microsoft.ManagementConsole;

using GT.BizTalk.SSO.AdminMMC.Management;
using GT.BizTalk.SSO.AdminMMC.UI;

namespace GT.BizTalk.SSO.AdminMMC.UI.MMC
{
    public class AppRootScopeNode : SSOScopeNode
    {
        #region Constructor
        public AppRootScopeNode()
        {
            this.DisplayName = Properties.Resources.EnterpriseSSO;

            // create the list view for the node
            MmcListViewDescription listViewDescription = new MmcListViewDescription();
            listViewDescription.DisplayName = "Enterprise Single Sign-On Config Store";
            listViewDescription.ViewType = typeof(AppListView);
            listViewDescription.Options = MmcListViewOptions.ExcludeScopeNodes;

            // attach the view to the node
            this.ViewDescriptions.Add(listViewDescription);
            this.ViewDescriptions.DefaultIndex = 0;

            // attach to view events
            SSOListView.ListViewChanged += SSOListView_ListViewChanged;

            // enable verbs
            this.EnabledStandardVerbs |= StandardVerbs.Refresh;
        }
        #endregion

        #region ScopeNode Overrides
        protected override void OnExpand(AsyncStatus status)
        {
            this.OnRefresh(status);
            base.OnExpand(status);
        }

        protected override void OnRefresh(AsyncStatus status)
        {
            this.RefreshApps();
            base.OnRefresh(status);
        }
        #endregion

        #region Public Methods
        public void RefreshApps()
        {
            try
            {
                // load apps
                List<SSOAppInfo> applications = SSOManager.GetApplications();
                this.Children.Clear();
                foreach (SSOAppInfo appInfo in applications)
                {
                    AppScopeNode appScopeNode = new AppScopeNode(appInfo.Name);
                    this.Children.Add(appScopeNode);
                }
            }
            catch (Exception ex)
            {
                MsgBoxUtil.Show(this.SnapIn, ex);
            }
        }
        #endregion

        #region Event Handlers
        void SSOListView_ListViewChanged(object sender, EventArgs<string> e)
        {
            if (sender is AppListView && e.Value == this.DisplayName)
            {
                this.RefreshApps();
            }
        }
        #endregion
    }
}
