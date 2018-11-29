using GT.BizTalk.Framework.Core.Tracing;
using GT.BizTalk.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.ScalableTransformation;
using Microsoft.BizTalk.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace GT.BizTalk.Framework.PipelineComponents
{
    /// <summary>
    /// Implements a pipeline component that applies Xsl Transformations to XML messages.
    /// </summary>
    /// <remarks>
    /// XslTransformer class implements pipeline components that can be used in send pipelines
    /// to convert XML messages to HTML format for sending using SMTP transport. Component can
    /// be placed only in the Encoding stage of send pipeline.
    /// </remarks>
    [System.Runtime.InteropServices.Guid("EFC64FB1-6220-4D32-B4C5-0FD962F69C9C")]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Encoder)]
    public class XslTransformer : BasePipelineComponent
    {
        #region Constants

        private const string XSLT_FILE_PATH_PROP_NAME = "XsltFilePath";

        #endregion Constants

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public XslTransformer()
            : base(Resources.ResourceManager, Resources.XslTransformerName, Resources.XslTransformerDescription, Resources.XslTransformerVersion, Resources.XslTransformerIcon)
        {
        }

        #endregion Constructor

        #region Design-time properties

        /// <summary>
        /// Gets/sets the location of xsl transformation file.
        /// </summary>
        [BtsPropertyName("XsltFilePathPropertyName")]
        [BtsDescription("XsltFilePathPropertyDescription")]
        public string XsltFilePath
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
            this.XsltFilePath = this.ReadPropertyValue<string>(propertyBag, XSLT_FILE_PATH_PROP_NAME, this.XsltFilePath);
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected override void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            this.WritePropertyValue(propertyBag, XSLT_FILE_PATH_PROP_NAME, this.XsltFilePath);
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
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(this.XsltFilePath) == true)
                errors.Add(Resources.XslTransformerInvalidXsltFilePath);

            if (File.Exists(this.XsltFilePath) == false)
                errors.Add(string.Format(Resources.XslTransformerXsltFileNotFoundError, this.XsltFilePath));

            return errors;
        }

        /// <summary>
        /// Converts XML messages to HTML messages using provided Xslt file.
        /// It also sets the content type of the message part to be "text/html"
        /// which is necessary for client mail applications to correctly render
        /// the message.
        /// </summary>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="inputMessage">Input message</param>
        /// <returns>The input message converted to HTML.</returns>
        protected override IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            if (inputMessage.BodyPart != null)
            {
                TraceProvider.Logger.TraceInfo("Applying XSLT transformation: {0}", this.XsltFilePath);
                inputMessage.BodyPart.Data = this.TransformMessage(inputMessage.BodyPart.Data);
                inputMessage.BodyPart.ContentType = "text/html";
                pipelineContext.ResourceTracker.AddResource(inputMessage.BodyPart.Data);
            }
            else
            {
                TraceProvider.Logger.TraceInfo("Message has no body part, exiting");
            }

            // return original message
            return inputMessage;
        }

        #endregion BasePipelineComponent Members

        #region Helper functions

        /// <summary>
        /// Transforms XML message in input stream to HTML message
        /// </summary>
        /// <param name="inputStream">Stream with input XML message.</param>
        /// <returns>Stream with output HTML message</returns>
        private Stream TransformMessage(Stream inputStream)
        {
            if (File.Exists(this.XsltFilePath) == false)
                throw new ArgumentException(string.Format(Resources.XslTransformerXsltFileNotFoundError, this.XsltFilePath), XSLT_FILE_PATH_PROP_NAME);

           // XmlTextReader stylesheet = new XmlTextReader(this.XsltFilePath);
            // Load transform 
            XslTransform transform = new XslTransform();
            transform.Load(this.XsltFilePath); 
            //Load Xml stream in XmlDocument. 
            XmlDocument doc = new XmlDocument();
            doc.Load(inputStream);

            //Create memory stream to hold transformed data. 
           var  ms = new MemoryStream();

            //Preform transform 
            transform.Transform(doc, null, ms, null);
            ms.Seek(0, SeekOrigin.Begin); 
            return ms;
          /*  // Load transform
            BTSXslTransform trans = new BTSXslTransform();
            trans.Load(stylesheet);

            XmlTextReader inputReader = new XmlTextReader(inputStream);

            // create virtual stream to hold transformed data.
            VirtualStream outputStream = new VirtualStream();

            // perform transform
            trans.ScalableTransform(inputReader, null, outputStream, new XmlUrlResolver(), false);

            outputStream.Seek(0, SeekOrigin.Begin);
            return outputStream;*/
        }

        #endregion Helper functions
    }
}