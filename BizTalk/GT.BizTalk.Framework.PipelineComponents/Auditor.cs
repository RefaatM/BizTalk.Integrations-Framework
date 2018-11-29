using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;

using DE.DAXFSA.Framework.BizTalk;
using DE.DAXFSA.Framework.Core.Tracing;
using DE.DAXFSA.Framework.PipelineComponents.Properties;

namespace DE.DAXFSA.Framework.PipelineComponents
{
    /// <summary>
    /// Implements a pipeline component that archives messages.
    /// 
    /// It supports the BizTalk %SourceFileName% and %MessageID% macros and a 
    /// custom date/time macro to specify any date/time format.
    /// 
    /// The custom date/time macro has the following syntax: %datetime:format%
    /// where: format is any valid .NET date formatting string.
    /// 
    /// Pattern example: %datetime:yyyMMddHHmmss%_%MessageID%_%SourceFileName%.bak
    /// which will output something like this: 20130101120030_{d1a62e55-7924-476c-9da8-bc538573ae47}_Invoice.csv.bak
    /// </summary>
    [System.Runtime.InteropServices.Guid("B777D597-0225-4C92-B060-3C63F22A2D48")]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PartyResolver)]
    public class Auditor : BasePipelineComponent
    {
        #region Constants
        private const int BUFFER_SIZE = 4096;
        private const string AUDIT_PATH_PROP_NAME = "AuditPath";
        private const string AUDIT_FILE_NAME_PROP_NAME = "AuditFileName";
        private const string OPTIMIZED_PROP_NAME = "Optimized";
        #endregion

        #region Fields
        private string auditFileFullPath = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public Auditor()
            : base(Resources.ResourceManager, Resources.AuditName, Resources.AuditDescription, Resources.AuditVersion, Resources.AuditIcon)
        {
        }
        #endregion

        #region Design-time properties
        /// <summary>
        /// Gets/sets the archiving directory.
        /// </summary>
        [BtsPropertyName("AuditPathPropertyName")]
        [BtsDescription("AuditPathPropertyDescription")]
        public string AuditPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the archive file name.
        /// </summary>
        [BtsPropertyName("AuditFileNamePropertyName")]
        [BtsDescription("AuditFileNamePropertyDescription")]
        public string AuditFileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets a value indicating whether the the archiving will use optimization.
        /// </summary>
        [BtsPropertyName("OptimizedPropertyName")]
        [BtsDescription("OptimizedPropertyDescription")]
        public bool Optimized
        {
            get;
            set;
        }
        #endregion

        #region BasePipelineComponent Members
        /// <summary>
        /// Load component properties from a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="errorLog">Error log level</param>
        protected override void LoadProperties(IPropertyBag propertyBag, int errorLog)
        {
            this.AuditPath = this.ReadPropertyValue<string>(propertyBag, AUDIT_PATH_PROP_NAME, this.AuditPath);
            this.AuditFileName = this.ReadPropertyValue<string>(propertyBag, AUDIT_FILE_NAME_PROP_NAME, this.AuditFileName);
            this.Optimized = this.ReadPropertyValue<bool>(propertyBag, OPTIMIZED_PROP_NAME, this.Optimized);
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected override void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            this.WritePropertyValue(propertyBag, AUDIT_PATH_PROP_NAME, this.AuditPath);
            this.WritePropertyValue(propertyBag, AUDIT_FILE_NAME_PROP_NAME, this.AuditFileName);
            this.WritePropertyValue(propertyBag, OPTIMIZED_PROP_NAME, this.Optimized);
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
            if (this.Enabled == true && string.IsNullOrEmpty(this.AuditFileName) == true)
                errors.Add(Resources.AuditInvalidArchiveFileName);

            if (this.Enabled == true && string.IsNullOrEmpty(this.AuditPath) == true)
                errors.Add(string.Format(Resources.AuditPathNotFoundError, this.AuditPath));

            return errors;
        }

        /// <summary>
        /// Returns a value indicating whether the pipeline component is enabled.
        /// </summary>
        /// <remarks>
        /// Checks the BAMTraker.Enabled context property; otherwise returns the Enabled pipeline 
        /// configuration setting.
        /// </remarks>
        /// <param name="inputMessage">Input message</param>
        /// <returns><b>true</b> if the component is enabled; <b>false</b> otherwise.</returns>
        protected override bool IsEnabled(IBaseMessage inputMessage)
        {
            return inputMessage.Context.Read<bool>(ArchiverProperties.Enabled.Name, ArchiverProperties.Enabled.Namespace, this.Enabled);
        }

        /// <summary>
        /// Archives the input message into the specified archiving location.
        /// </summary>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="inputMessage">Input message</param>
        /// <returns>Original input message</returns>
        protected override IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            if (inputMessage.BodyPart != null)
            {
                // expand macros and build the archive file path
                this.auditFileFullPath = this.GetAuditFilePath(inputMessage);

                // get original stream
                Stream messageStream = inputMessage.BodyPart.GetOriginalDataStream();

                // check if the optimized flag is on
                bool optimized = inputMessage.Context.Read<bool>(ArchiverProperties.Optimized.Name, ArchiverProperties.Optimized.Namespace, this.Optimized);
                TraceProvider.Logger.TraceInfo("Auditing file: Optimized = {0}; FilePath = {1}", optimized, this.auditFileFullPath);
                if (optimized == true)
                {
                    // create a forward-only eventing stream
                    messageStream = this.CreateForwardOnlyEventingStream(messageStream);
                    inputMessage.BodyPart.Data = messageStream;
                }
                else
                {
                    // create seekable stream to work on the message
                    messageStream = new ReadOnlySeekableStream(messageStream);
                    inputMessage.BodyPart.Data = messageStream;
                    // save the stream position
                    long position = messageStream.Position;
                    // copy stream to file
                    this.CopyStreamToFile(messageStream);
                    // restore the stream position
                    messageStream.Position = position;
                }

                // prevent the message stream from being GC collected
                pipelineContext.ResourceTracker.AddResource(messageStream);
            }
            else
            {
                TraceProvider.Logger.TraceInfo("Message has no body part, exiting");
            }
            // return original message
            return inputMessage;
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Builds the file path for the archive file.
        /// </summary>
        /// <param name="inmsg">BizTalk input message.</param>
        /// <returns>The file path for the archive file.</returns>
        private string GetAuditFilePath(IBaseMessage inputMessage)
        {
            // generate the new filename using the specified filename pattern
            string archiveFileName = inputMessage.Context.Read<string>(ArchiverProperties.ArchiveFileName.Name, ArchiverProperties.ArchiveFileName.Namespace, this.AuditFileName);
            string fileName = FileNameMacroHelper.ResolveMacros(inputMessage, archiveFileName);
            // combine archive path with file name
            string archivePath = inputMessage.Context.Read<string>(ArchiverProperties.ArchivePath.Name, ArchiverProperties.ArchivePath.Namespace, this.AuditPath);
            return Path.Combine(archivePath, fileName);
        }

        /// <summary>
        /// Performs an immediate copy of the file to disk - a 'non-optimized' copy. 
        /// </summary>
        /// <param name="dataStream">Stream to archive.</param>
        private void CopyStreamToFile(Stream dataStream)
        {
            try
            {
                using (FileStream fileArchiveStream = new FileStream(this.auditFileFullPath, FileMode.Create, FileAccess.Write))
                {
                    // read the stream and write to the archive file
                    byte[] buffer = new byte[BUFFER_SIZE];
                    int sizeRead;
                    while ((sizeRead = dataStream.Read(buffer, 0, BUFFER_SIZE)) != 0)
                    {
                        fileArchiveStream.Write(buffer, 0, sizeRead);
                    }
                }
            }
            catch (Exception ex)
            {
                // log a warning but do not rethrow to avoid stopping main message processing
                TraceProvider.Logger.TraceWarning(Resources.ArchiverExceptionMessage, this.auditFileFullPath, ex.Message);
            }
        }

        /// <summary>
        /// Create a forward-only eventing stream.
        /// </summary>
        /// <param name="originalStream">Stream containing the original data stream.</param>
        /// <returns>Stream wrapped forward-only eventing stream.</returns>
        private Stream CreateForwardOnlyEventingStream(Stream originalStream)
        {
            // create the forward-only eventing stream
            CForwardOnlyEventingReadStream eventingStream = new CForwardOnlyEventingReadStream(originalStream);
            eventingStream.ReadEvent += StreamOnReadEvent;

            return eventingStream;
        }

        /// <summary>
        /// Stream 'On Read Event'. Called as downstream components or the BizTalk Messaging Agent itself 
        /// reads the message. This method spools the portion of the stream read to disk. An 'optimized' copy.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="args"></param>
        private void StreamOnReadEvent(object src, EventArgs args)
        {
            try
            {
                ReadEventArgs rargs = args as ReadEventArgs;

                if (rargs != null)
                {
                    using (FileStream fileArchiveStream = new FileStream(this.auditFileFullPath, FileMode.Append, FileAccess.Write))
                    {
                        fileArchiveStream.Write(rargs.buffer, rargs.offset, rargs.bytesRead);
                    }
                }
            }
            catch (Exception ex)
            {
                // log a warning but do not rethrow to avoid stopping main message processing
                TraceProvider.Logger.TraceWarning(Resources.ArchiverExceptionMessage, this.auditFileFullPath, ex.Message);
            }
        }
        #endregion
    }
}
