using GT.BizTalk.Framework.BizTalk;
using GT.BizTalk.Framework.BizTalk.Message;
using GT.BizTalk.Framework.Core.Tracing;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Resources;

namespace GT.BizTalk.Framework.PipelineComponents
{
    /// <summary>
    /// Implements BizTalk interfaces and functionality required by all
    /// pipeline components.
    /// </summary>
    public class BasePipelineComponent :
        Microsoft.BizTalk.Component.BaseCustomTypeDescriptor,
        Microsoft.BizTalk.Component.Interop.IBaseComponent,
        Microsoft.BizTalk.Component.Interop.IComponent,
        Microsoft.BizTalk.Component.Interop.IComponentUI,
        Microsoft.BizTalk.Component.Interop.IPersistPropertyBag
    {
        #region Constants

        private const string ENABLED_PROP_NAME = "Enabled";

        #endregion Constants

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public BasePipelineComponent(
            ResourceManager resourceManager,
            string name,
            string description,
            string version,
            Bitmap iconImage)
            : base(resourceManager)
        {
            this.Name = name;
            this.Description = description;
            this.Version = version;
            this.IconImage = iconImage;
            this.Enabled = true;
        }

        #endregion Constructor

        #region Design-time Properties

        /// <summary>
        /// Gets o sets the context value collection.
        /// </summary>
        [BtsPropertyName("EnabledPropertyName")]
        [BtsDescription("EnabledPropertyDescription")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get;
            set;
        }

        #endregion Design-time Properties

        #region Overridable Members

        /// <summary>
        /// Initializes the component.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Get component class ID.
        /// </summary>
        /// <param name="classID"></param>
        protected virtual void GetClassID(out Guid classID)
        {
            classID = this.GetType().GUID;
        }

        /// <summary>
        /// Load component properties from a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="errorLog">Error log level</param>
        protected virtual void LoadProperties(IPropertyBag propertyBag, int errorLog)
        {
            this.Enabled = this.ReadPropertyValue<bool>(propertyBag, ENABLED_PROP_NAME, this.Enabled);
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected virtual void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            this.WritePropertyValue(propertyBag, ENABLED_PROP_NAME, this.Enabled);
        }

        /// <summary>
        /// The Validate method is called by the BizTalk Editor during the build
        /// of a BizTalk project.
        /// </summary>
        /// <returns>
        /// A list of error and/or warning messages encounter during validation
        /// of this component.
        /// </returns>
        protected virtual List<string> Validate()
        {
            return null;
        }

        /// <summary>
        /// Returns a value indicating whether the pipeline component is enabled.
        /// </summary>
        /// <remarks>
        /// Derived classes can override this method to determine whether the component
        /// is enabled based on the message context.
        /// </remarks>
        /// <param name="inputMessage">Input message</param>
        /// <returns><b>true</b> if the component is enabled; <b>false</b> otherwise.</returns>
        protected virtual bool IsEnabled(IBaseMessage inputMessage)
        {
            return this.Enabled;
        }

        /// <summary>
        /// Executes the component processing.
        /// </summary>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="inputMessage">Input message</param>
        /// <returns>Original input message</returns>
        /// <remarks>
        /// This method is used to initiate the processing of the message in this pipeline component.
        /// </remarks>
        protected virtual IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            return inputMessage;
        }

        #endregion Overridable Members

        #region IBaseComponent Members

        /// <summary>
        /// Gets a component name.
        /// </summary>
        [Browsable(false)]
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a component description.
        /// </summary>
        [Browsable(false)]
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a component version.
        /// </summary>
        [Browsable(false)]
        public string Version
        {
            get;
            private set;
        }

        #endregion IBaseComponent Members

        #region IComponentUI

        /// <summary>
        /// Component icon to use in BizTalk Editor.
        /// </summary>
        protected Bitmap IconImage
        {
            get;
            private set;
        }

        /// <summary>
        /// Component icon to use in BizTalk Editor.
        /// </summary>
        [Browsable(false)]
        IntPtr IComponentUI.Icon
        {
            get
            {
                return this.IconImage.GetHicon();
            }
        }

        /// <summary>
        /// The Validate method is called by the BizTalk Editor during the build
        /// of a BizTalk project.
        /// </summary>
        /// <param name="obj">Project system.</param>
        /// <returns>
        /// A list of error and/or warning messages encounter during validation
        /// of this component.
        /// </returns>
        IEnumerator IComponentUI.Validate(object projectSystem)
        {
            if (projectSystem == null)
                throw new System.ArgumentNullException("projectSystem");

            List<string> errors = null;

            if (this.Enabled == true)
            {
                try
                {
                    // call virtual implementation
                    errors = this.Validate();
                }
                catch (Exception e)
                {
                    errors = new List<string>();
                    errors.Add(e.Message);
                }
            }
            else
            {
                TraceProvider.Logger.TraceInfo("Pipeline component is disabled. Skipping validation!");
            }

            IEnumerator enumerator = null;
            if (errors != null && errors.Count > 0)
            {
                enumerator = errors.GetEnumerator();
            }
            return enumerator;
        }

        #endregion IComponentUI

        #region IComponent members

        /// <summary>
        /// Implements IComponent.Execute method.
        /// </summary>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="inputMessage">Input message</param>
        /// <returns>Original input message</returns>
        /// <remarks>
        /// IComponent.Execute method is used to initiate
        /// the processing of the message in this pipeline component.
        /// </remarks>
        IBaseMessage Microsoft.BizTalk.Component.Interop.IComponent.Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            var callToken = TraceProvider.Logger.TraceIn(this.Name);
            try
            {
                if (this.IsEnabled(inputMessage) == true)
                {
                    // check arguments
                    if (pipelineContext == null)
                        throw new ArgumentNullException("pipelineContext");
                    if (inputMessage == null)
                        throw new ArgumentNullException("inputMessage");

                    // call virtual implementation
                    return this.Execute(pipelineContext, inputMessage);
                }
                else
                {
                    TraceProvider.Logger.TraceInfo("Pipeline component is disabled. Skipping execution!");
                    return inputMessage;
                }
            }
            catch (Exception ex)
            {
                // put component name as a source information in this exception,
                // so the event log in message could reflect this
                ex.Source = this.Name;
                TraceProvider.Logger.TraceError(ex);
                throw ex;
            }
            finally
            {
                TraceProvider.Logger.TraceOut(callToken, this.Name);
            }
        }

        #endregion IComponent members

        #region IPersistPropertyBag Members

        /// <summary>
        /// Initialize component.
        /// </summary>
        void IPersistPropertyBag.InitNew()
        {
            // call virtual implementation
            this.Initialize();
        }

        /// <summary>
        /// Get component class ID.
        /// </summary>
        /// <param name="classID"></param>
        void IPersistPropertyBag.GetClassID(out Guid classID)
        {
            // call virtual implementation
            this.GetClassID(out classID);
        }

        /// <summary>
        /// Load component properties from a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="errorLog">Error log level</param>
        void IPersistPropertyBag.Load(IPropertyBag propertyBag, int errorLog)
        {
            if (null == propertyBag)
                throw new ArgumentNullException("propertyBag");

            this.Enabled = this.ReadPropertyValue<bool>(propertyBag, ENABLED_PROP_NAME, this.Enabled);

            // call virtual implementation
            this.LoadProperties(propertyBag, errorLog);
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        void IPersistPropertyBag.Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            if (null == propertyBag)
                throw new ArgumentNullException("propertyBag");

            this.WritePropertyValue(propertyBag, ENABLED_PROP_NAME, this.Enabled);

            // call virtual implementation
            this.SaveProperties(propertyBag, clearDirty, saveAllProperties);
        }

        #endregion IPersistPropertyBag Members

        #region Helpers

        #region PropertyBag

        /// <summary>
        /// Reads property value from property bag.
        /// </summary>
        /// <param name="propBag">Property bag.</param>
        /// <param name="propName">Name of property.</param>
        /// <returns>Value of the property.</returns>
        protected object ReadPropertyValue(IPropertyBag propBag, string propName)
        {
            return propBag.Read(propName);
        }

        /// <summary>
        /// Reads the property value from the property bag into a value of type T.
        /// If the conversion cannot be performed, a default value will be returned.
        /// </summary>
        /// <typeparam name="T">Type to which the string value will be converted to.</typeparam>
        /// <param name="propBag">Property bag.</param>
        /// <param name="propName">Name of property.</param>
        /// <param name="defaultValue">Default value returned if the conversion cannot be performed.</param>
        /// <returns>Value of the property.</returns>
        protected T ReadPropertyValue<T>(IPropertyBag propBag, string propName, T defaultValue)
        {
            return propBag.Read<T>(propName, defaultValue);
        }

        /// <summary>
        /// Writes a property to th eproperty bag.
        /// </summary>
        /// <param name="propBag">Property bag.</param>
        /// <param name="propName">Property name.</param>
        /// <param name="val">Value.</param>
        protected void WritePropertyValue(IPropertyBag propBag, string propName, object val)
        {
            propBag.Write(propName, val);
        }

        #endregion PropertyBag

        #region Message Creation

        /// <summary>
        /// Creates a new message by cloning the source message, but using the specified output stream instead.
        /// </summary>
        /// <param name="pipelineContext">Pipeline context.</param>
        /// <param name="baseMessage">Base (original) message.</param>
        /// <param name="newMessageStream">Message stream.</param>
        /// <returns>New instance of the message.</returns>
        protected IBaseMessage CreateNewMessage(IPipelineContext pipelineContext, IBaseMessage baseMessage, Stream newMessageStream)
        {
            return MessageHelper.CreateNewMessage(pipelineContext, baseMessage, newMessageStream);
        }

        #endregion Message Creation

        #endregion Helpers
    }
}