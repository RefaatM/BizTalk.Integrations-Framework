using DE.DAXFSA.Framework.Core.Tracing;
using DE.DAXFSA.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using System;
using System.Collections.Generic;
using System.Text;

namespace DE.DAXFSA.Framework.PipelineComponents
{
    /// <summary>
    /// Pipeline component to filter message base on XPATH Query
    ///
    /// The pipeline component can be placed into any receive or send
    /// pipeline stage.
    /// </summary>
    [System.Runtime.InteropServices.Guid(COMPONENT_GUID)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Any)]
    public class XPathQueryFiltering : BasePipelineComponent, IBaseComponent
    {
        #region Constants

        private const string COMPONENT_GUID = "017C9D7A-C06A-43DC-B6C9-297D2E0341DF";

        #endregion Constants

        #region private vars

        private bool isQuery1FoundAndMatch = false;
        private bool isQuery2FoundAndMatch = false;
        private bool isQuery3FoundAndMatch = false;
        private bool isQuery4FoundAndMatch = false;
        private bool isQuery5FoundAndMatch = false;

        #endregion private vars

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public XPathQueryFiltering()
            : base(Resources.ResourceManager
            , Resources.XPathQueryFilteringName
            , Resources.XPathQueryFilteringDescription
            , Resources.XPathQueryFilteringVersion
            , Resources.XPathQueryFilteringIcon)
        {
        }

        #endregion Constructor

        #region Design-time properties

        /// <summary>
        /// Get and set the XPath Query used to filter message
        /// </summary>
        [BtsPropertyName("XPathQueryPropertyName1")]
        [BtsDescription("XPathQuery1PropertyDescription1")]
        public string XPathQuery1
        {
            get;
            set;
        }

        /// <summary>
        /// Get and set the XPath Query used to filter message
        /// </summary>
        [BtsPropertyName("XPathQueryPropertyName2")]
        [BtsDescription("XPathQuery1PropertyDescription2")]
        public string XPathQuery2
        {
            get;
            set;
        }

        /// <summary>
        /// Get and set the XPath Query used to filter message
        /// </summary>
        [BtsPropertyName("XPathQueryPropertyName3")]
        [BtsDescription("XPathQuery1PropertyDescription3")]
        public string XPathQuery3
        {
            get;
            set;
        }

        /// <summary>
        /// Get and set the XPath Query used to filter message
        /// </summary>
        [BtsPropertyName("XPathQueryPropertyName4")]
        [BtsDescription("XPathQuery1PropertyDescription4")]
        public string XPathQuery4
        {
            get;
            set;
        }

        /// <summary>
        /// Get and set the XPath Query used to filter message
        /// </summary>
        [BtsPropertyName("XPathQueryPropertyName5")]
        [BtsDescription("XPathQuery1PropertyDescription5")]
        public string XPathQuery5
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
            this.XPathQuery1 = this.ReadPropertyValue<string>(propertyBag, "XPathQuery1", this.XPathQuery1);
            this.XPathQuery2 = this.ReadPropertyValue<string>(propertyBag, "XPathQuery2", this.XPathQuery2);
            this.XPathQuery3 = this.ReadPropertyValue<string>(propertyBag, "XPathQuery3", this.XPathQuery3);
            this.XPathQuery4 = this.ReadPropertyValue<string>(propertyBag, "XPathQuery4", this.XPathQuery4);
            this.XPathQuery5 = this.ReadPropertyValue<string>(propertyBag, "XPathQuery5", this.XPathQuery5);
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected override void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            this.WritePropertyValue(propertyBag, "XPathQuery1", this.XPathQuery1);
            this.WritePropertyValue(propertyBag, "XPathQuery2", this.XPathQuery2);
            this.WritePropertyValue(propertyBag, "XPathQuery3", this.XPathQuery3);
            this.WritePropertyValue(propertyBag, "XPathQuery4", this.XPathQuery4);
            this.WritePropertyValue(propertyBag, "XPathQuery5", this.XPathQuery5);
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
            if (inputMessage.BodyPart != null)
            {
                TraceProvider.Logger.TraceInfo("Filtering message based on XPath Query");

                SetupMessageContentQuery(pipelineContext, ref inputMessage);

                if (!IsSendMessage)
                {
                    TraceProvider.Logger.TraceInfo("Message is being filtered");
                    return null;
                }

                TraceProvider.Logger.TraceInfo("Message is qualified to be sent");
                return inputMessage;
            }
            else
            {
                TraceProvider.Logger.TraceInfo("Message has no body part, exiting");
            }
            // return original message
            return inputMessage;
        }

        #endregion BasePipelineComponent Members

        #region custom methods

        /// <summary>
        /// Setup Message Content Query for filtering messages
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pInMsg"></param>
        private void SetupMessageContentQuery(IPipelineContext pContext, ref IBaseMessage pInMsg)
        {
            //make sure the stream is seekable, otherwise, wrap it in seekable stream
            System.IO.Stream streamWrapper = pInMsg.BodyPart.GetOriginalDataStream();
            if (!pInMsg.BodyPart.GetOriginalDataStream().CanSeek)
            {
                streamWrapper = new ReadOnlySeekableStream(pInMsg.BodyPart.GetOriginalDataStream());
                pInMsg.BodyPart.Data = streamWrapper;
                pContext.ResourceTracker.AddResource(streamWrapper);
            }

            //clone the stream to prevent XPathNavigator closing the stream
            System.IO.Stream streamClone = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(new System.IO.StreamReader(streamWrapper).ReadToEnd()));
            streamClone.Seek(0, System.IO.SeekOrigin.Begin);
            System.Xml.XPath.XPathNavigator navigator = new System.Xml.XPath.XPathDocument(streamClone).CreateNavigator();

            //perform the queries
            System.Xml.XPath.XPathExpression query = null;
            if (!string.IsNullOrEmpty(XPathQuery1))
            {
                query = navigator.Compile(XPathQuery1);
                isQuery1FoundAndMatch = Convert.ToBoolean(navigator.Evaluate(query));
            }
            if (!string.IsNullOrEmpty(XPathQuery2))
            {
                query = navigator.Compile(XPathQuery2);
                isQuery2FoundAndMatch = Convert.ToBoolean(navigator.Evaluate(query));
            }
            if (!string.IsNullOrEmpty(XPathQuery3))
            {
                query = navigator.Compile(XPathQuery3);
                isQuery3FoundAndMatch = Convert.ToBoolean(navigator.Evaluate(query));
            }
            if (!string.IsNullOrEmpty(XPathQuery4))
            {
                query = navigator.Compile(XPathQuery4);
                isQuery4FoundAndMatch = Convert.ToBoolean(navigator.Evaluate(query));
            }
            if (!string.IsNullOrEmpty(XPathQuery5))
            {
                query = navigator.Compile(XPathQuery5);
                isQuery5FoundAndMatch = Convert.ToBoolean(navigator.Evaluate(query));
            }

            streamWrapper.Seek(0, System.IO.SeekOrigin.Begin);
            pContext.ResourceTracker.AddResource(streamWrapper);
        }

        /// <summary>
        /// indicate if we should sent out the message
        /// </summary>
        private bool IsSendMessage
        {
            get
            {
                //if anyone of the XPathQuery Matches, we should not send the message
                if (isQuery1FoundAndMatch || isQuery2FoundAndMatch || isQuery3FoundAndMatch || isQuery4FoundAndMatch || isQuery5FoundAndMatch)
                    return false;
                else
                    return true;
            }
        }

        #endregion custom methods
    }
}