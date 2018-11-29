using DE.DAXFSA.Framework.Core.Streaming;
using DE.DAXFSA.Framework.Core.Tracing;
using DE.DAXFSA.Framework.PipelineComponents.Design;
using DE.DAXFSA.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace DE.DAXFSA.Framework.PipelineComponents
{
    /// <summary>
    /// Pipeline component that transcodes the message stream from a source encoding to a target encoding.
    ///
    /// The pipeline component can be placed into the encoding and/or decoding pipeline stages.
    /// </summary>
    [System.Runtime.InteropServices.Guid("BB1B8B71-4673-48B7-B84D-403D686AF0AA")]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Decoder)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Encoder)]
    public class EncodingTranscoder : BasePipelineComponent
    {
        #region Constants

        private const string SOURCE_ENCODING_PROP_NAME = "SourceEncoding";
        private const string TARGET_ENCODING_PROP_NAME = "TargetEncoding";
        private const string BUFFER_SIZE_PROP_NAME = "BufferSize";
        private const int DEFAULT_BUFFER_SIZE = 4096;

        #endregion Constants

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public EncodingTranscoder()
            : base(Resources.ResourceManager, Resources.EncodingTranscoderName, Resources.EncodingTranscoderDescription, Resources.EncodingTranscoderVersion, Resources.EncodingTranscoderIcon)
        {
            this.SourceEncoding = Encoding.GetEncoding("Windows-1252"); // ANSI
            this.TargetEncoding = Encoding.UTF8;
            this.BufferSize = DEFAULT_BUFFER_SIZE;
        }

        #endregion Constructor

        #region Design-time properties

        /// <summary>
        /// Gets/sets the source encoding.
        /// </summary>
        [BtsPropertyName("SourceEncodingPropertyName")]
        [BtsDescription("SourceEncodingPropertyDescription")]
        [Editor(typeof(EncodingTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(EncodingTypeConverter))]
        public Encoding SourceEncoding
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the target encoding.
        /// </summary>
        [BtsPropertyName("TargetEncodingPropertyName")]
        [BtsDescription("TargetEncodingPropertyDescription")]
        [Editor(typeof(EncodingTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(EncodingTypeConverter))]
        public Encoding TargetEncoding
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the buffer size to use when reading the stream.
        /// </summary>
        [BtsPropertyName("BufferSizePropertyName")]
        [BtsDescription("BufferSizePropertyDescription")]
        public int BufferSize
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
            this.SourceEncoding = System.Text.Encoding.GetEncoding(this.ReadPropertyValue<string>(propertyBag, SOURCE_ENCODING_PROP_NAME, this.SourceEncoding.WebName));
            this.TargetEncoding = System.Text.Encoding.GetEncoding(this.ReadPropertyValue<string>(propertyBag, TARGET_ENCODING_PROP_NAME, this.TargetEncoding.WebName));
            this.BufferSize = this.ReadPropertyValue<int>(propertyBag, BUFFER_SIZE_PROP_NAME, this.BufferSize);
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected override void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            this.WritePropertyValue(propertyBag, SOURCE_ENCODING_PROP_NAME, this.SourceEncoding.WebName);
            this.WritePropertyValue(propertyBag, TARGET_ENCODING_PROP_NAME, this.TargetEncoding.WebName);
            this.WritePropertyValue(propertyBag, BUFFER_SIZE_PROP_NAME, this.BufferSize);
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
        /// Removes the specified characters from the message.
        /// </summary>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="inputMessage">Input message</param>
        /// <returns>Original input message</returns>
        protected override IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            if (inputMessage.BodyPart != null)
            {
                if (this.SourceEncoding != this.TargetEncoding)
                {
                    TraceProvider.Logger.TraceInfo("Transcoding message stream from '{0}' to '{1}'", this.SourceEncoding.WebName, this.TargetEncoding.WebName);

                    // assign a new TranscodingStream to the incoming message
                    Stream originalStream = inputMessage.BodyPart.GetOriginalDataStream();
                    Stream transcodeStream = new TranscodingStream(originalStream, this.SourceEncoding, this.TargetEncoding, this.BufferSize);

                    // return the message for downstream pipeline components (further down in the pipeline)
                    inputMessage.BodyPart.Data = transcodeStream;
                    pipelineContext.ResourceTracker.AddResource(transcodeStream);
                }
                else
                {
                    TraceProvider.Logger.TraceInfo("Source and target encoding are the same. Skip transcoding: Souce = '{0}', Target = '{1}'", this.SourceEncoding.WebName, this.TargetEncoding.WebName);
                }
            }
            else
            {
                TraceProvider.Logger.TraceInfo("Message has no body part, exiting");
            }
            // return original message
            return inputMessage;
        }

        #endregion BasePipelineComponent Members
    }
}