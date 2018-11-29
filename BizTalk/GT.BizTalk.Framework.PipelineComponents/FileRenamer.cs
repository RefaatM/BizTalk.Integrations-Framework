using DE.DAXFSA.Framework.BizTalk;
using DE.DAXFSA.Framework.Core.Tracing;
using DE.DAXFSA.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System.Collections.Generic;

namespace DE.DAXFSA.Framework.PipelineComponents
{
    /// <summary>
    /// Pipeline component that allows renaming the received file name using
    /// a custom pattern.
    ///
    /// It supports the BizTalk %SourceFileName% and %MessageID% macros and a
    /// custom date/time macro to specify any date/time format.
    ///
    /// The custom date/time macro has the following syntax: %datetime:format%
    /// where: format is any valid .NET date formatting string.
    ///
    /// Pattern example: %datetime:yyyMMddHHmmss%_%MessageID%_%SourceFileName%.bak
    /// which will output something like this: 20130101120030_{d1a62e55-7924-476c-9da8-bc538573ae47}_Invoice.csv.bak
    ///
    /// The pipeline component can be placed into any receive or send
    /// pipeline stage.
    /// </summary>
    [System.Runtime.InteropServices.Guid("AAEA3F2D-7F9E-42E0-81EF-2912016F5D24")]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Any)]
    public class FileRenamer : BasePipelineComponent
    {
        #region Constants

        private const string FILE_NAME_PATTERN_PROP_NAME = "FileNamePattern";

        #endregion Constants

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public FileRenamer()
            : base(Resources.ResourceManager, Resources.FileRenamerName, Resources.FileRenamerDescription, Resources.FileRenamerVersion, Resources.FileRenamerIcon)
        {
        }

        #endregion Constructor

        #region Design-time properties

        /// <summary>
        /// Gets/sets the file name pattern.
        /// </summary>
        [BtsPropertyName("FileNamePatternPropertyName")]
        [BtsDescription("FileNamePatternPropertyDescription")]
        public string FileNamePattern
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
            this.FileNamePattern = this.ReadPropertyValue<string>(propertyBag, FILE_NAME_PATTERN_PROP_NAME, this.FileNamePattern);
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected override void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            this.WritePropertyValue(propertyBag, FILE_NAME_PATTERN_PROP_NAME, this.FileNamePattern);
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

            if (string.IsNullOrEmpty(this.FileNamePattern) == true)
                errors.Add(string.Format(Resources.FileRenamerInvalidFileNamePattern, this.FileNamePattern));

            return errors;
        }

        /// <summary>
        /// Overrides the "ReceivedFileName" context property with a new filename generated using the specified filename pattern.
        /// </summary>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="inputMessage">Input message</param>
        /// <returns>Original input message</returns>
        protected override IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            // generate the new filename using the specified filename pattern
            string outputFileName = FileNameMacroHelper.ResolveMacros(inputMessage, this.FileNamePattern);
            TraceProvider.Logger.TraceInfo("Renaming ReceivedFileName to: {0}", outputFileName);

            // write generated filename back into the "ReceivedFileName" context property
            inputMessage.Context.Write(BtsProperties.ReceivedFileName.Name, BtsProperties.ReceivedFileName.Namespace, outputFileName);
            // return original message
            return inputMessage;
        }

        #endregion BasePipelineComponent Members
    }
}