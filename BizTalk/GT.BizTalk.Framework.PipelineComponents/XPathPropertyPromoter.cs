using DE.DAXFSA.Framework.Core.Tracing;
using DE.DAXFSA.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace DE.DAXFSA.Framework.PipelineComponents
{
    /// <summary>
    /// Pipeline component which can be placed into any receive or send
    /// pipeline stage and do a property promotion / distinguished fields
    /// writing based on arbitrary XPath.
    /// </summary>
    [System.Runtime.InteropServices.Guid("8164DF16-9932-400A-9870-BC40863261E4")]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Any)]
    public class XPathPropertyPromoter : BasePipelineComponent
    {
        #region Constants

        private const string RAISE_XPATH_EXCEPTIONS_PROP_NAME = "RaiseXPathExceptions";
        private const string PROPERTIES_PROP_NAME = "XPathProperties";

        #endregion Constants

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public XPathPropertyPromoter()
            : base(Resources.ResourceManager, Resources.XPathPropertyPromoterName, Resources.XPathPropertyPromoterDescription, Resources.XPathPropertyPromoterVersion, Resources.XPathPropertyPromoterIcon)
        {
            this.XPathProperties = new List<XPathProperty>();
        }

        #endregion Constructor

        #region Design-time properties

        /// <summary>
        /// Gets o sets the context value collection.
        /// </summary>
        [BtsPropertyName("XPathPropertiesPropName")]
        [BtsDescription("XPathPropertiesPropDescription")]
        [Editor(typeof(System.ComponentModel.Design.CollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public List<XPathProperty> XPathProperties
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets a value indicating whether to raise XPath exceptions.
        /// </summary>
        [BtsPropertyName("XPathPropertyRaiseXPathExPropName")]
        [BtsDescription("XPathPropertyRaiseXPathExPropDescription")]
        public bool RaiseXPathExceptions
        {
            get;
            set;
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
            this.RaiseXPathExceptions = this.ReadPropertyValue<bool>(propertyBag, RAISE_XPATH_EXCEPTIONS_PROP_NAME, this.RaiseXPathExceptions);

            string xml = this.ReadPropertyValue<string>(propertyBag, PROPERTIES_PROP_NAME, null);
            if (string.IsNullOrEmpty(xml) == false)
            {
                XPathPropertySerializer serializer = new XPathPropertySerializer();
                serializer.Deserialize(xml);
                this.XPathProperties = serializer.Properties;
            }
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected override void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            this.WritePropertyValue(propertyBag, RAISE_XPATH_EXCEPTIONS_PROP_NAME, this.RaiseXPathExceptions);

            string xml = null;
            if (this.XPathProperties != null)
            {
                XPathPropertySerializer serializer = new XPathPropertySerializer(this.XPathProperties);
                xml = serializer.Serialize();
            }
            this.WritePropertyValue(propertyBag, PROPERTIES_PROP_NAME, xml);
        }

        /// <summary>
        /// Validates the component properties.
        /// </summary>
        /// <remarks>
        /// Throws an exception if any of the properties is invalid.
        /// </remarks>
        protected override List<string> Validate()
        {
            List<string> errors = new List<string>();

            if (this.XPathProperties == null || this.XPathProperties.Count == 0)
                errors.Add(Resources.XPathPropertiesValidationMessage);

            for (int i = 0; i < this.XPathProperties.Count; i++)
            {
                XPathProperty xpathProperty = this.XPathProperties[i];
                if (string.IsNullOrEmpty(xpathProperty.Name) == true)
                    errors.Add(string.Format("Property {0} - Name cannot be empty.", i));
                if (string.IsNullOrEmpty(xpathProperty.Namespace) == true)
                    errors.Add(string.Format("Property {0} - Namespace cannot be empty.", i));
                if (string.IsNullOrEmpty(xpathProperty.XPath) == true)
                    errors.Add(string.Format("Property {0} - XPath expression cannot be empty.", i));
            }
            return errors;
        }

        /// <summary>
        /// Promotes the configured properties into the message context.
        /// </summary>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="inputMessage">Input message</param>
        /// <returns>Original input message</returns>
        protected override IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            return this.PromoteProperties(pipelineContext, inputMessage);
        }

        #endregion BasePipelineComponent Members

        #region Private Members

        private IBaseMessage PromoteProperties(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            // check if the message has a body part
            if (inputMessage.BodyPart == null)
            {
                TraceProvider.Logger.TraceInfo("Message has no body part, exiting");
                return inputMessage;
            }

            // create seekable stream to work on the message
            Stream messageStream = new ReadOnlySeekableStream(inputMessage.BodyPart.GetOriginalDataStream());
            inputMessage.BodyPart.Data = messageStream;
            // save the stream position
            long position = messageStream.Position;

            // create an xml reader to prevent the xpath document from closing it
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = false;
            settings.CloseInput = false;
            using (XmlReader reader = XmlReader.Create(messageStream, settings))
            {
                XPathDocument xpathDoc = new XPathDocument(reader);
                XPathNavigator xpathNav = xpathDoc.CreateNavigator();

                if (this.XPathProperties != null)
                {
                    TraceProvider.Logger.TraceInfo("Using {0} XPath properties", this.XPathProperties.Count);
                    foreach (XPathProperty xpathProperty in this.XPathProperties)
                    {
                        // evaluate xpath expression
                        object propertyValue = Evaluate(xpathProperty, xpathNav);
                        // check if we found the value
                        if (propertyValue != null)
                        {
                            // promote or write the xpath property evaluation result
                            if (xpathProperty.Promote == true)
                            {
                                TraceProvider.Logger.TraceInfo("Promoting xpath property into context: {0} = {1}; Namespace = {2}, XPath = {3}", xpathProperty.Name, propertyValue, xpathProperty.Namespace, xpathProperty.XPath);
                                inputMessage.Context.Promote(xpathProperty.Name, xpathProperty.Namespace, propertyValue);
                            }
                            else
                            {
                                TraceProvider.Logger.TraceInfo("Writing xpath property into context: {0} = {1}; Namespace = {2}, XPath = {3}", xpathProperty.Name, propertyValue, xpathProperty.Namespace, xpathProperty.XPath);
                                inputMessage.Context.Write(xpathProperty.Name, xpathProperty.Namespace, propertyValue);
                            }
                        }
                    }
                }
            }

            // restore the stream position
            messageStream.Position = position;

            // prevent the message stream from being GC collected
            pipelineContext.ResourceTracker.AddResource(messageStream);

            // return
            return inputMessage;
        }

        /// <summary>
        /// Evaluates an xpath property.
        /// </summary>
        /// <param name="xpathProperty">XPathProperty instance.</param>
        /// <param name="navigator">XPathNavigator instance.</param>
        /// <returns>XPath expression result.</returns>
        private object Evaluate(XPathProperty xpathProperty, XPathNavigator navigator)
        {
            object result = null;
            // create an xpath expression
            XPathExpression expression = XPathExpression.Compile(xpathProperty.XPath);

            switch (expression.ReturnType)
            {
                case XPathResultType.String:
                case XPathResultType.Number:
                    result = navigator.Evaluate(expression);
                    break;

                case XPathResultType.NodeSet:
                    XPathNodeIterator ni = navigator.Select(expression);
                    if (ni.Count == 0 && this.RaiseXPathExceptions == true)
                    {
                        throw new ApplicationException(string.Format(Resources.XPathNodeNotFoundException, xpathProperty.XPath));
                    }
                    if (ni.Count > 1 && this.RaiseXPathExceptions == true)
                    {
                        throw new ApplicationException(string.Format(Resources.XPathMultipleResultsException, xpathProperty.XPath));
                    }
                    if (ni.Count == 1 && ni.MoveNext() == true)
                    {
                        result = ni.Current.ToString();
                    }
                    break;

                case XPathResultType.Boolean:
                    result = (bool)navigator.Evaluate(expression);
                    break;
            }
            return result;
        }

        #endregion Private Members
    }
}