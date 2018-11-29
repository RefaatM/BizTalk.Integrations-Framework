using System;
using System.Windows.Forms;

namespace GT.BizTalk.SSO.AdminMMC.UI.Controls
{
    public partial class SSOPropertyControl : UserControl
    {
        #region Constructor
        public SSOPropertyControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Overridable Methods
        public virtual bool IsDirty()
        {
            return true;
        }

        public virtual bool IsValid()
        {
            return true;
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
