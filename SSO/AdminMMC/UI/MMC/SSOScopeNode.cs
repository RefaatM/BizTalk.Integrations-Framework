using System;

using Microsoft.ManagementConsole;
using MMCAction = Microsoft.ManagementConsole.Action;

namespace GT.BizTalk.SSO.AdminMMC.UI.MMC
{
    /// <summary>
    /// Base class for the snapin scope nodes.
    /// </summary>
    public class SSOScopeNode : ScopeNode
    {
        #region Events
        public event EventHandler Refresh;
        public event EventHandler Delete;
        public event EventHandler<EventArgs<string>> Action;
        public event EventHandler<EventArgs<PropertyPageCollection>> AddPropertyPages;
        #endregion

        #region ScopeNode Overrides
        protected override void OnRefresh(AsyncStatus status)
        {
            base.OnRefresh(status);
            if (this.Refresh != null)
            {
                this.Refresh(this, EventArgs.Empty);
            }
        }

        protected override void OnDelete(SyncStatus status)
        {
            base.OnDelete(status);
            if (this.Delete != null)
            {
                this.Delete(this, EventArgs.Empty);
            }
        }

        protected override void OnAction(MMCAction action, AsyncStatus status)
        {
            base.OnAction(action, status);
            if (this.Action != null)
            {
                this.Action(this, new EventArgs<string>((string)action.Tag));
            }
        }

        protected override void OnAddPropertyPages(PropertyPageCollection propertyPageCollection)
        {
            base.OnAddPropertyPages(propertyPageCollection);
            if (this.AddPropertyPages != null)
            {
                this.AddPropertyPages(this, new EventArgs<PropertyPageCollection>(propertyPageCollection));
            }
            this.CurrentPropertySheetTitle = null;
        }

        public new bool ShowPropertySheet(string title)
        {
            this.CurrentPropertySheetTitle = title;
            return base.ShowPropertySheet(title);
        }
        #endregion

        #region Properties
        public string CurrentPropertySheetTitle
        {
            get;
            private set;
        }
        #endregion
    }
}
