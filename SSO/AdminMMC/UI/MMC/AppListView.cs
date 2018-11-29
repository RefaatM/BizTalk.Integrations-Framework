using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

using Microsoft.ManagementConsole;
using Microsoft.ManagementConsole.Advanced;
using MMCAction = Microsoft.ManagementConsole.Action;

using GT.BizTalk.SSO.AdminMMC.Management;
using GT.BizTalk.SSO.AdminMMC.UI;
using GT.BizTalk.SSO.AdminMMC.UI.PropertyPages;
using GT.BizTalk.SSO.AdminMMC.UI.Wizard;

namespace GT.BizTalk.SSO.AdminMMC.UI.MMC
{
    public class AppListView : SSOListView
    {
        #region Fields
        private static bool cancelProcessMultipleApps;
        private string currentPropertyPageTitle;
        #endregion

        #region MmcListView Overrides
        protected override void OnInitialize(AsyncStatus status)
        {
            // add columns
            this.Columns[0].Title = "Name";
            this.Columns[0].SetWidth(100);
            this.Columns.Add(new MmcListViewColumn("Status", 100));
            this.Columns.Add(new MmcListViewColumn("Description", 200));
            this.Columns.Add(new MmcListViewColumn("Admin Accounts", 100));
            this.Columns.Add(new MmcListViewColumn("User Accounts", 100));

            // set up view and actions
            this.Mode = MmcListViewMode.Report;
            this.SnapIn.SmallImages.Add(Properties.Resources.add_scope);
            this.ActionsPaneItems.Add(new MMCAction("Create Application...", "Create a configuration store application.", -1, "Create"));
            this.ActionsPaneItems.Add(new MMCAction("Import Application", "Import a configuration store application.", -1, "Import"));
            base.OnInitialize(status);

            // attach to scope node events
            this.ScopeNode.Refresh += ScopeNode_Refresh;
            this.ScopeNode.Action += ScopeNode_Action;
            this.ScopeNode.AddPropertyPages += ScopeNode_AddPropertyPages;
        }

        protected override void OnShow()
        {
            this.RefreshApps();
        }

        protected override void OnRefresh(AsyncStatus status)
        {
            this.RefreshApps();
            base.OnRefresh(status);
        }

        protected override void OnDelete(SyncStatus status)
        {
            try
            {
                if (MsgBoxUtil.Confirm(this.SnapIn, Properties.Resources.DeleteApplicationMessge) == true)
                {
                    this.ProcessMultipleApps("Delete");
                    this.OnListViewChanged();
                }
            }
            catch (Exception ex)
            {
                MsgBoxUtil.Show(this.SnapIn, ex);
            }
        }

        protected override void OnAction(MMCAction action, AsyncStatus status)
        {
            try
            {
                switch ((string)action.Tag)
                {
                    case "Create":
                        AppWizardForm appWizForm = new AppWizardForm();
                        appWizForm.TopMost = true;
                        DialogResult ret = appWizForm.ShowDialog();
                        if (ret == DialogResult.OK)
                        {
                            this.RefreshApps();
                            this.OnListViewChanged();
                        }
                        break;

                    case "Import":
                        this.Import();
                        this.RefreshApps();
                        this.OnListViewChanged();
                        break;
                }
            }
            catch (Exception ex)
            {
                MsgBoxUtil.Show(this.SnapIn, ex);
            }
        }

        protected override void OnSelectionChanged(SyncStatus status)
        {
            this.SelectionData.ActionsPaneItems.Clear();
            int count = this.SelectedNodes.Count;
            if (count == 0)
            {
                this.SelectionData.Clear();
            }
            else
            {
                this.SelectionData.EnabledStandardVerbs = StandardVerbs.Delete;
                if (count == 1)
                {
                    this.SelectionData.EnabledStandardVerbs |= StandardVerbs.Properties;
                    this.SelectionData.ActionsPaneItems.Add(new MMCAction("Add Field", "Add a new field to this application.", -1, "AddField"));
                    this.SelectionData.ActionsPaneItems.Add(new MMCAction("Export Application", "Export the selected application configuration.", -1, "Export"));
                    this.SelectionData.ActionsPaneItems.Add(new ActionSeparator());
                    this.SelectionData.Update(this.SelectedNodes[0], false, null, null);
                }
                else
                {
                    this.SelectionData.Update(this.SelectedNodes, true, null, null);
                }

                this.SelectionData.ActionsPaneItems.Add(new MMCAction("Enable", "Enable the selected applications", -1, "Enable"));
                this.SelectionData.ActionsPaneItems.Add(new MMCAction("Disable", "Disable the selected applications", -1, "Disable"));
                this.SelectionData.ActionsPaneItems.Add(new ActionSeparator());
                this.SelectionData.ActionsPaneItems.Add(new MMCAction("Clear Cache", "Clear the credential cache for the selected applications", -1, "PurgeCache"));
            }
        }

        protected override void OnSelectionAction(MMCAction action, AsyncStatus status)
        {
            try
            {
                switch ((string)action.Tag)
                {
                    case "AddField":
                        this.currentPropertyPageTitle = "Add Field";
                        this.SelectionData.ShowPropertySheet("Add Field");
                        break;

                    case "Export":
                        string appName = this.SelectedNodes[0].DisplayName;
                        this.Export(appName);
                        break;

                    default:
                        this.ProcessMultipleApps((string)action.Tag);
                        base.OnSelectionAction(action, status);
                        this.OnListViewChanged();
                        break;
                }
            }
            catch (Exception ex)
            {
                MsgBoxUtil.Show(this.SnapIn, ex);
            }
        }

        protected override void OnAddPropertyPages(PropertyPageCollection propertyPageCollection)
        {
            // get application name from selected node
            string appName = this.SelectedNodes[0].DisplayName;

            switch (this.currentPropertyPageTitle)
            {
                case "Add Field":
                    // create property page and attach event handlers
                    AppFieldPropertyPage propPage = new AppFieldPropertyPage(appName);
                    propPage.Saved += PropPage_Saved;
                    // add the page to the collection
                    propertyPageCollection.Add(new AppFieldPropertyPage(appName));
                    break;

                default:
                    // create instance of the AppPropertyPageManager
                    AppPropertyPageManager propPageManager = new AppPropertyPageManager(appName);
                    propPageManager.Saved += PropPageManager_Saved;
                    // add property pages
                    propPageManager.AddPropertyPages(propertyPageCollection);
                    break;
            }
        }
        #endregion

        #region Public Methods
        public void RefreshApps()
        {
            try
            {
                // load apps
                List<SSOAppInfo> applications = SSOManager.GetApplications();
                
                this.ResultNodes.Clear(); 
                foreach (SSOAppInfo app in applications)
                {
                    ResultNode resultNode = new ResultNode();
                    resultNode.DisplayName = app.Name;
                    resultNode.SubItemDisplayNames.AddRange(new string[5]
                    {
                        app.Status,
                        app.Description,
                        app.AdminAccounts,
                        app.UserAccounts,
                        app.Contact
                    });
                    this.ResultNodes.Add(resultNode);
                }
            }
            catch (Exception ex)
            {
                MsgBoxUtil.Show(this.SnapIn, ex);
            }
        }
        #endregion

        #region Event Handlers
        void ScopeNode_Refresh(object sender, EventArgs e)
        {
            this.RefreshApps();
        }

        void ScopeNode_Action(object sender, EventArgs<string> e)
        {
            switch (e.Value)
            {
                case "AddField":
                    this.ScopeNode.ShowPropertySheet("Add Field");
                    break;
            }
        }

        void ScopeNode_AddPropertyPages(object sender, EventArgs<PropertyPageCollection> e)
        {
            switch (this.ScopeNode.CurrentPropertySheetTitle)
            {
                case "Add Field":
                    // create property page and attach event handlers
                    string appName = this.ScopeNode.DisplayName;
                    AppFieldPropertyPage propPage = new AppFieldPropertyPage(appName);
                    propPage.Saved += PropPage_Saved;
                    // add the page to the collection
                    e.Value.Add(propPage);
                    break;
            }
        }

        void PropPageManager_Saved(object sender, EventArgs e)
        {
            this.RefreshApps();
            this.OnListViewChanged();
        }

        void PropPage_Saved(object sender, EventArgs<SSOAppField> e)
        {
        }
        #endregion

        #region Multi-selection operations
        private class ProcessAppsData
        {
            public string Action;
            public Node[] SelectedNodes;
            public WaitDialog WaitDialog;
            public NamespaceSnapInBase SnapIn;
        }

        private void ProcessMultipleApps(string action)
        {
            AppListView.cancelProcessMultipleApps = false;
            
            // copy selection to an array
            Node[] array = this.SelectedNodes.ToArray();

            WaitDialog waitDialog = new WaitDialog();
            waitDialog.Title = "Processing multiple applications";
            waitDialog.StatusText = "Please wait while your request is performed.";
            waitDialog.Name = waitDialog.Title;
            waitDialog.CanCancel = true;
            waitDialog.TotalWork = this.SelectedNodes.Count;
            waitDialog.Cancel += new EventHandler(this.WaitCancelHandler);
            ThreadPool.QueueUserWorkItem(new WaitCallback(AppListView.WaitCallbackProcessApps), new ProcessAppsData()
            {
                Action = action,
                SelectedNodes = array,
                WaitDialog = waitDialog,
                SnapIn = this.SnapIn
            });
            
            waitDialog.ShowDialog();
            if (action == "PurgeCache")
                return;

            this.RefreshApps();
        }

        private static void WaitCallbackProcessApps(object objData)
        {
            ProcessAppsData processAppsData = (ProcessAppsData)objData;
            try
            {
                foreach (Node node in processAppsData.SelectedNodes)
                {
                    if (AppListView.cancelProcessMultipleApps)
                        break;

                    string displayName = node.DisplayName;
                    switch (processAppsData.Action)
                    {
                        case "Enable":
                            SSOManager.EnableApplication(displayName, true);
                            break;
                        case "Disable":
                            SSOManager.EnableApplication(displayName, false);
                            break;
                        case "Delete":
                            SSOManager.DeleteApplication(displayName);
                            break;
                        case "PurgeCache":
                            SSOManager.PurgeApplicationCache(displayName);
                            break;
                    }
                    ++processAppsData.WaitDialog.WorkProcessed;
                }
            }
            catch (Exception ex)
            {
                MsgBoxUtil.Show(processAppsData.SnapIn, ex);
            }
            finally
            {
                processAppsData.WaitDialog.CompleteDialog();
            }
        }

        private void WaitCancelHandler(object sender, EventArgs e)
        {
            AppListView.cancelProcessMultipleApps = true;
        }
        #endregion
    }
}
