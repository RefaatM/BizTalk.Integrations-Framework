using System;
using System.Windows.Forms;

using GT.BizTalk.SSO.AdminMMC.Management;

namespace GT.BizTalk.SSO.AdminMMC.UI.Wizard
{
    public class AppWizardStep : UserControl
    {
        #region Public Overridable Methods
        public virtual bool IsValid()
        {
            return true;
        }

        public virtual void Refresh(SSOAppInfo appInfo)
        {
        }

        public virtual void Update(SSOAppInfo appInfo)
        {
        }
        #endregion

        #region Events
        public event EventHandler DataChanged;

        protected void OnDataChanged(EventArgs e)
        {
            if (this.DataChanged != null)
                this.DataChanged(this, e);
        }
        #endregion
    }
}
