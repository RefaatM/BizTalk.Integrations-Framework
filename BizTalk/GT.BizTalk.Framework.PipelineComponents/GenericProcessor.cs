using GT.BizTalk.Framework.BizTalk.Pipeline;
using GT.BizTalk.Framework.Core.Tracing;
using GT.BizTalk.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;
using System.Xml;

namespace GT.BizTalk.Framework.PipelineComponents
{
    /// <summary>
    /// Pipeline component that consumes the message.
    ///
    /// The pipeline component can be placed into any receive or send
    /// pipeline stage.
    /// </summary>
    [System.Runtime.InteropServices.Guid(COMPONENT_GUID)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Any)]
    public class GenericProcessor : BasePipelineComponent, IBaseComponent
    {
        #region Constants
        private const string COMPONENT_GUID = "3DF03F32-A674-4DB6-A864-1ACD9271D3DA";
        #endregion Constants

        #region Fields
        private string _propertyBagString;
        private Dictionary<string, Dictionary<string, object>> _propertyBag = new Dictionary<string, Dictionary<string, object>>();
        #endregion Fields

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public GenericProcessor()
            : base(Resources.ResourceManager
            , Resources.GenericProcessorName
            , Resources.GenericProcessorDescription
            , Resources.GenericProcessorVersion
            , Resources.GenericProcessorIcon)
        {
        }

        #endregion Constructor

        #region Design-time properties

        /// <summary>
        /// Get and set fully qualified assembly name
        /// </summary>
        [BtsPropertyName("HandlerAssemblyNamePropertyName")]
        [BtsDescription("HandlerAssemblyNamePropertyDescription")]
        public string HandlerAssemblyName
        {
            get;
            set;
        }

        /// <summary>
        /// Set the property bag values to be passed to the handler
        /// </summary>
        [BtsPropertyName("PropertyBagPropertyName")]
        [BtsDescription("PropertyBagPropertyDescription")]
        public string PropertyBag
        {
            get { return _propertyBagString; }
            set { _propertyBagString = value; ParsePropertyBag(value); }
        }

        #endregion Design-time properties

        #region BasePipelineComponent Members

        /// <summary>
        /// Load component properties from a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="errorLog">Error log level</param>
        protected override void LoadProperties(IPropertyBag propertyBag, int errorLog)
        {
            this.HandlerAssemblyName = this.ReadPropertyValue<string>(propertyBag, "HandlerAssemblyName", this.HandlerAssemblyName);
            this.PropertyBag = this.ReadPropertyValue<string>(propertyBag, "PropertyBag", this.PropertyBag);
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected override void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            this.WritePropertyValue(propertyBag, "HandlerAssemblyName", this.HandlerAssemblyName);
            this.WritePropertyValue(propertyBag, "PropertyBag", this.PropertyBag);
        }

        /// <summary>
        /// Validates the component properties.
        /// </summary>
        /// <returns>
        /// A list of error and/or warning messages encounter during validation
        /// of this component.
        /// </returns>
        protected override List<string> Validate()
        {
            return null;
        }

        /// <summary>
        /// Send request message back to caller as response
        /// </summary>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="inputMessage">Input message</param>
        /// <returns>Original input message</returns>
        protected override IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            if (!string.IsNullOrEmpty(this.HandlerAssemblyName))
            {
                TraceProvider.Logger.TraceInfo("Activating IIBaseMessageProcessor...");

                var type = Type.GetType(this.HandlerAssemblyName);
                var handler = (IIBaseMessageProcessor)Activator.CreateInstance(type);

                TraceProvider.Logger.TraceInfo("Using IIBaseMessageProcessor {0}:{1} <version {2}>", handler.Name, handler.Description, handler.Version);

                return handler.Execute(pipelineContext, inputMessage, this.PropertyBag);
            }

            TraceProvider.Logger.TraceInfo("GenericProcessor is passthru (no handler is defined)");

            return inputMessage;
        }

        #endregion BasePipelineComponent Members

        #region

        private void ParsePropertyBag(string propertyBagString)
        {
            if (string.IsNullOrEmpty(propertyBagString)) return;

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(propertyBagString);
            var xmlNodes = xmlDoc.SelectNodes("/PropertyBag/Property");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                var name = string.Empty;
                var value = xmlNode.InnerText;
                Dictionary<string, object> attributeValuePairs = new Dictionary<string, object>();

                foreach (XmlAttribute attr in xmlNode.Attributes)
                {
                    if (attr.Name != "name")
                    {
                        attributeValuePairs.Add(attr.Name, attr.Value);
                    }
                    else
                    {
                        name = attr.Name;
                    }
                }

                if (!string.IsNullOrEmpty(name))
                {
                    attributeValuePairs.Add("InnerText", value);
                    _propertyBag.Add(name, attributeValuePairs);
                }
            }
        }

        #endregion
    }
}