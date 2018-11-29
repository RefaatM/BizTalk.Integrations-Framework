using System;

using Microsoft.ManagementConsole;

using GT.BizTalk.SSO.AdminMMC.UI.Controls;

namespace GT.BizTalk.SSO.AdminMMC.UI.PropertyPages
{
    public class SSOPropertyPage<T> : PropertyPage
    {
        #region Constructor
        protected SSOPropertyPage()
        {
        }
        #endregion

        #region Properties
        protected SSOPropertyControl PropertyControl
        {
            get { return this.Control as SSOPropertyControl; }
            set { this.Control = value; }
        }
        #endregion

        #region Public Overridable Methods
        public virtual bool IsDirty()
        {
            SSOPropertyControl propertyControl = this.PropertyControl;
            return (propertyControl != null ? propertyControl.IsDirty() :  true);
        }

        public virtual bool IsValid()
        {
            SSOPropertyControl propertyControl = this.PropertyControl;
            return (propertyControl != null ? propertyControl.IsValid() : true);
        }

        public virtual void Refresh(T data)
        {
        }

        public virtual void Update(T data)
        {
        }
        #endregion

        #region PropertyPage Overrides
        protected override void OnInitialize()
        {
            base.OnInitialize();
            var e = EventArgs.Empty;
            this.OnLoad(e);
        }

        protected override bool OnApply()
        {
            var e = new ResultEventArgs<bool>();
            this.OnSave(e);
            return e.Result;
        }

        protected override bool OnOK()
        {
            var e = new ResultEventArgs<bool>();
            this.OnSave(e);
            return e.Result;
        }

        protected override bool QueryCancel()
        {
            return true;
        }
        #endregion

        #region Events
        public event EventHandler Load;

        private void OnLoad(EventArgs e)
        {
            if (this.Load != null)
            {
                this.Load(this, e);
            }
        }

        public event EventHandler<ResultEventArgs<bool>> Save;

        private void OnSave(ResultEventArgs<bool> e)
        {
            if (this.Save != null)
            {
                this.Save(this, e);
            }
            else
            {
                // set the result to "true" if there are no subscribers
                e.Result = true;
            }
        }
        #endregion
    }
}
