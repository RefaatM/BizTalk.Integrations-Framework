using System;
using System.Windows.Forms;

using Microsoft.ManagementConsole;
using Microsoft.ManagementConsole.Advanced;

using GT.BizTalk.SSO.AdminMMC.UI.PropertyPages;

namespace GT.BizTalk.SSO.AdminMMC.UI.Controls
{
    public partial class AppFieldPropertyControl : SSOPropertyControl
    {
        #region Fields
        private string fieldName;
        private string fieldValue;
        private string fieldIdentifier;
        #endregion

        #region Constructor
        public AppFieldPropertyControl()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        public string FieldName
        {
            get { return this.txtKey.Text; }
            set
            {
                if (this.txtKey.Text.Trim() != value)
                {
                    this.txtKey.Text = value;
                    this.fieldName = value;
                }
            }
        }

        public string FieldValue
        {
            get { return this.txtValue.Text; }
            set
            {
                if (this.txtValue.Text.Trim() != value)
                {
                    this.txtValue.Text = value;
                    this.fieldValue = value;
                }
            }
        }

        public string FieldIdentifier
        {
            get { return this.txtIdentifier.Text; }
            set
            {
                if (this.txtIdentifier.Text.Trim() != value)
                {
                    this.txtIdentifier.Text = value;
                    this.fieldIdentifier = value;
                }
            }
        }
        #endregion

        #region SSOPropertyControl Overrides
        public override bool IsDirty()
        {
            return
                (this.fieldName != this.txtKey.Text.Trim())
                || (this.fieldValue != this.txtValue.Text.Trim())
                || (this.fieldIdentifier != this.txtIdentifier.Text.Trim());
        }

        public override bool IsValid()
        {
            return 
                (this.txtKey.Text.Trim().Length > 0)
                && (this.txtIdentifier.Text.Trim().Length > 0);
        }
        #endregion

        #region Event Handlers
        private void txtKey_TextChanged(object sender, EventArgs e)
        {
            this.OnDataChanged(e);
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            this.OnDataChanged(e);
        }

        private void txtIdentifier_TextChanged(object sender, EventArgs e)
        {
            this.OnDataChanged(e);
        }
        #endregion
    }
}
