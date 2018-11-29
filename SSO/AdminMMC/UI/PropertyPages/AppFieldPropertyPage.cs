using System;

using Microsoft.ManagementConsole;
using Microsoft.ManagementConsole.Advanced;

using GT.BizTalk.SSO.AdminMMC.Management;
using GT.BizTalk.SSO.AdminMMC.UI.Controls;

namespace GT.BizTalk.SSO.AdminMMC.UI.PropertyPages
{
    public class AppFieldPropertyPage : SSOPropertyPage<SSOAppField>
    {
        #region Fields
        private string appName = string.Empty;
        private string originalFieldName = string.Empty;
        private SSOAppField appField = new SSOAppField();
        private AppFieldPropertyControl propertyControl;
        #endregion

        #region Constructor
        /// <summary>
        /// Instance constructor. Initializes the instance using the specified application name.
        /// </summary>
        /// <remarks>
        /// This constructor should be used for new fields.
        /// </remarks>
        /// <param name="appName">SSO application name.</param>
        public AppFieldPropertyPage(string appName)
            : this(appName, string.Empty, string.Empty, SSOManager.ConfigIdentifier)
        {
        }

        /// <summary>
        /// Instance constructor. Initializes the instance using the specified application name,
        /// field name and field Value.
        /// </summary>
        /// <param name="appName">SSO application name.</param>
        /// <param name="fieldName">Field name.</param>
        /// <param name="fieldValue">Field value.</param>
        /// <param name="identifier">Identifier.</param>
        public AppFieldPropertyPage(string appName, string fieldName, string fieldValue, string identifier)
        {
            this.Title = "Field Settings";
            this.appName = appName;
            this.originalFieldName = fieldName; // remember the original field name to check if the field is being renamed
            this.appField.Name = fieldName;
            this.appField.Value = fieldValue;
            this.appField.Identifier = identifier;
            
            // initialize property control
            this.propertyControl = new AppFieldPropertyControl();
            this.propertyControl.DataChanged += PropertyControl_DataChanged;
            this.PropertyControl = this.propertyControl;

            // attach the property page events
            this.Load += PropertyPage_Load;
            this.Save += PropertyPage_Save;
        }
        #endregion

        #region SSOPropertyPage Overrides
        public override void Refresh(SSOAppField appField)
        {
            base.Refresh(appField);

            // refresh UI
            this.propertyControl.FieldName = appField.Name;
            this.propertyControl.FieldValue = appField.Value;
            this.propertyControl.FieldIdentifier = appField.Identifier;
        }

        public override void Update(SSOAppField appField)
        {
            base.Update(appField);

            // update app field with new values from the UI
            appField.Name = this.propertyControl.FieldName;
            appField.Value = this.propertyControl.FieldValue;
            appField.Identifier = this.propertyControl.FieldIdentifier;
        }
        #endregion

        #region Event Handlers
        private void PropertyPage_Load(object sender, EventArgs e)
        {
            this.Refresh(this.appField);
        }

        private void PropertyPage_Save(object sender, ResultEventArgs<bool> e)
        {
            if (this.IsValid() == false)
            {
                MsgBoxUtil.Show(this.ParentSheet, "Some information is missing or incorrect. Please review and correct the information entered on the page.");
                e.Result = false;
            }
            else
            {
                if (this.IsDirty() == true)
                {
                    // get the new field information entered on the page
                    this.Update(this.appField);

                    // get the SSO application fields
                    SSOAppFieldCollection appFields = SSOManager.GetApplicationFields(this.appName);

                    // flag to indicate whether the application must be recreated
                    bool recreate = false;

                    // check if the field is new or was renamed
                    if (string.IsNullOrEmpty(this.originalFieldName) == true ||
                        this.propertyControl.FieldName.Equals(this.originalFieldName, StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        // the field is new or was renamed, ensure the new field name does not exist
                        if (appFields.Contains(this.propertyControl.FieldName) == true)
                        {
                            MsgBoxUtil.Show(this.ParentSheet, string.Format("The field name {0} already exists.", this.propertyControl.FieldName));
                            e.Result = false;
                            return;
                        }

                        // need to recreate the application
                        recreate = true;

                        // remove the field before writing it using the new name (it will cause to add it)
                        if (appFields.Contains(this.propertyControl.FieldName) == true)
                        {
                            appFields.Remove(this.propertyControl.FieldName);
                        }
                    }

                    // write the field value (if the field was renamed, a new one will be added)
                    appFields.Write(this.propertyControl.FieldName, this.propertyControl.FieldValue);
                    // update the sso application
                    SSOManager.UpdateApplicationFields(this.appName, appFields, recreate);

                    // update the result node
                    ResultNode resultNode = (ResultNode)this.ParentSheet.SelectionObject;
                    this.OnSaved(new EventArgs<SSOAppField>(this.appField));
                }
                e.Result = true;
            }
        }

        void PropertyControl_DataChanged(object sender, System.EventArgs e)
        {
            this.Dirty = this.propertyControl.IsDirty();
        }
        #endregion

        #region Events
        public event EventHandler<EventArgs<SSOAppField>> Saved;

        protected void OnSaved(EventArgs<SSOAppField> e)
        {
            if (this.Saved != null)
                this.Saved(this, e);
        }
        #endregion
    }
}
