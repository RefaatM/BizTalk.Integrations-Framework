using System;

using Microsoft.ManagementConsole;
using MMCAction = Microsoft.ManagementConsole.Action;

using GT.BizTalk.SSO.AdminMMC.Management;
using GT.BizTalk.SSO.AdminMMC.UI;
using GT.BizTalk.SSO.AdminMMC.UI.PropertyPages;

namespace GT.BizTalk.SSO.AdminMMC.UI.MMC
{
    public class AppFieldListView : SSOListView
    {
        #region MmcListView Overrides
        protected override void OnInitialize(AsyncStatus status)
        {
            // add columns
            this.Columns[0].Title = "Field Name";
            this.Columns[0].SetWidth(200);
            this.Columns.Add(new MmcListViewColumn("Field Value", 350));
            this.Columns.Add(new MmcListViewColumn("Identifier", 100));

            this.Mode = MmcListViewMode.Report;
            this.SnapIn.SmallImages.Add(Properties.Resources.add_scope);
            this.ActionsPaneItems.Add(new MMCAction("Add Field", "Add a new field to this application.", -1, "AddField"));
            this.ActionsPaneItems.Add(new MMCAction("Export Application", "Export the selected application configuration.", -1, "Export"));
            this.ActionsPaneItems.Add(new ActionSeparator());
            this.ActionsPaneItems.Add(new MMCAction("Enable", "Enable the selected applications", -1, "Enable"));
            this.ActionsPaneItems.Add(new MMCAction("Disable", "Disable the selected applications", -1, "Disable"));
            this.ActionsPaneItems.Add(new ActionSeparator());
            this.ActionsPaneItems.Add(new MMCAction("Clear Cache", "Clear the credential cache for the selected applications", -1, "PurgeCache"));
            base.OnInitialize(status);

            // attach to scope node events
            this.ScopeNode.Refresh += ScopeNode_Refresh;
            this.ScopeNode.Action += ScopeNode_Action;
            this.ScopeNode.AddPropertyPages += ScopeNode_AddPropertyPages;
        }

        protected override void OnShow()
        {
            this.RefreshFields();
        }

        protected override void OnRefresh(AsyncStatus status)
        {
            this.RefreshFields();
            base.OnRefresh(status);
        }

        protected override void OnDelete(SyncStatus status)
        {
            try
            {
                if (MsgBoxUtil.Confirm(this.SnapIn, Properties.Resources.DeleteFieldMessage) == true)
                {
                    // get SSO application fields
                    SSOAppFieldCollection appFields = SSOManager.GetApplicationFields(this.ScopeNode.DisplayName);

                    // delete selected ones
                    foreach (ResultNode resultNode in this.SelectedNodes)
                    {
                        appFields.Remove(resultNode.DisplayName);
                        this.ResultNodes.Remove(resultNode);
                    }

                    // save fields
                    SSOManager.UpdateApplicationFields(this.ScopeNode.DisplayName, appFields, true);
                    // refresh view
                    this.RefreshFields();
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
                    case "AddField":
                        this.ScopeNode.ShowPropertySheet("Add Field");
                        break;

                    case "Export":
                        string appName = this.ScopeNode.DisplayName;
                        this.Export(appName);
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
            int count = this.SelectedNodes.Count;
            if (count == 0)
            {
                this.SelectionData.Clear();
                this.SelectionData.ActionsPaneItems.Clear();
            }
            else
            {
                this.SelectionData.Update(this.SelectedNodes[0], count > 1, null, null);
                this.SelectionData.ActionsPaneItems.Clear();
                this.SelectionData.EnabledStandardVerbs = StandardVerbs.Delete;
                this.SelectionData.EnabledStandardVerbs |= StandardVerbs.Properties;
            }
        }

        protected override void OnSelectionAction(MMCAction action, AsyncStatus status)
        {
            try
            {
                switch ((string)action.Tag)
                {
                    case "Properties":
                        this.SelectionData.ShowPropertySheet("Field Properties");
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
            if (this.SelectedNodes.Count == 0)
                throw new Exception("There should be at least one field selected.");

            // get selected node information
            string appName = this.ScopeNode.DisplayName;
            string fieldName = this.SelectedNodes[0].DisplayName;
            string fieldValue = this.SelectedNodes[0].SubItemDisplayNames[0];
            string identifier = this.SelectedNodes[0].SubItemDisplayNames[1];

            // create property page and attach event handlers
            AppFieldPropertyPage propPage = new AppFieldPropertyPage(appName, fieldName, fieldValue, identifier);
            propPage.Saved += PropPage_Saved;

            // add the page to the collection
            propertyPageCollection.Add(propPage);
        }
        #endregion

        #region Methods
        private void RefreshFields()
        {
            // get current config settings
            SSOAppFieldCollection appFields = SSOManager.GetApplicationFields(this.ScopeNode.DisplayName);

            this.ResultNodes.Clear();
            foreach (SSOAppField field in appFields)
            {
                if (string.IsNullOrEmpty(field.Name) == false)
                {
                    ResultNode resultNode = new ResultNode();
                    resultNode.DisplayName = field.Name;
                    resultNode.SubItemDisplayNames.Add(field.Value);
                    resultNode.SubItemDisplayNames.Add(field.Identifier);
                    this.ResultNodes.Add(resultNode);
                }
            }
        }
        #endregion

        #region Event Handlers
        private void ScopeNode_Refresh(object sender, EventArgs e)
        {
            this.RefreshFields();
        }

        void ScopeNode_Action(object sender, EventArgs<string> e)
        {
            switch (e.Value)
            {
                default:
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

        void PropPage_Saved(object sender, EventArgs<SSOAppField> e)
        {
            SSOAppField appField = e.Value;
            if (this.SelectedNodes.Count > 0)
            {
                this.SelectedNodes[0].DisplayName = appField.Name;
                this.SelectedNodes[0].SubItemDisplayNames[0] = appField.Value;
                this.SelectedNodes[0].SubItemDisplayNames[1] = appField.Identifier;
            }
            else
            {
                ResultNode resultNode = new ResultNode();
                resultNode.DisplayName = appField.Name;
                resultNode.SubItemDisplayNames.Add(appField.Value);
                this.ResultNodes.Add(resultNode);
            }
        }
        #endregion
    }
}
