using GT.BizTalk.Framework.BizTalk;
using GT.BizTalk.Framework.BizTalk.Archive;
using GT.BizTalk.Framework.Core.Tracing;
using GT.BizTalk.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GT.BizTalk.Framework.PipelineComponents
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
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Encoder)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Decoder)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Validate)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PartyResolver)]

    public class Archiver : BasePipelineComponent
    {
        #region Constants

        private const int BUFFER_SIZE = 4096;
        private const string ARCHIVE_PATH_PROP_NAME = "ArchivePath";
     
        private const string ARCHIVE_TODB_ENABLED = "DBArchiveEnabled";
        private const string ARCHIVE_DBCONNSTR = "DbConnStr";
        private const string ARCHIVE_FILE_NAME_PROP_NAME = "FileArchiveFileName";
        private const string ARCHIVE_TOFILE_ENABLED = "FileArchivingEnabled";
        private const string ARCHIVE_FILE_PATH = "FileArchiveBackupFolder";
        private const string ARCHIVE_FILE_OVERWRITE = "FileArchiveIsOverwriteFiles";
        private const string ARCHIVE_USER_NAME=  "FileArchiveUserName";
        private const string ARCHIVE_USER_DOMAIN =  "FileArchiveUserDomain";
        private const string ARCHIVE_USER_PASS = "FileArchiveUserPwd";

        /// <summary>
        /// Provider Name: System.Data.SqlClient
        /// </summary>
        private const string DbConnProvider = "System.Data.SqlClient";

        /// <summary>
        /// Name of stored procedure for archiving, it should accept Xml, and VarBinaryData
        /// </summary>
        private const string DbSPName = "InsMessages";

        #endregion Constants

        #region Fields

        private string archiveFileFullPath = null;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public Archiver()
            : base(Resources.ResourceManager, Resources.ArchiverName, Resources.ArchiverDescription, Resources.ArchiverVersion, Resources.ArchiverIcon)
        {
        }

        #endregion Constructor

        #region Design-time properties

       

        /// <summary>
        /// Gets/sets the archive file name.
        /// </summary>
        [BtsPropertyName("ArchiveFileNamePropertyName")]
        [BtsDescription("ArchiveFileNamePropertyDescription")]
        public string ArchiveFileName
        {
            get;
            set;
        }

        /*/// <summary>
        /// Gets/sets a value indicating whether the the archiving will use optimization.
        /// </summary>
        [BtsPropertyName("OptimizedPropertyName")]
        [BtsDescription("OptimizedPropertyDescription")]
        public bool Optimized
        {
            get;
            set;
        }
        */
        /// <summary>
        /// Enabled flag for Db Archive
        /// </summary>
        [BtsPropertyName("IsArchiveToDbPropertyName")]
        [BtsDescription("IsArchiveToDbPropertyDescription")]
        public bool IsArchiveToDb { get; set; }

        /// <summary>
        /// ConnectionString
        /// </summary>
        [BtsPropertyName("ArchiverDbConnStrPropertyName")]
        [BtsDescription("ArchiverDbConnStrPropertyDescription")]
        public string DbConnStr { get; set; }

        /// <summary>
        /// File Archive Enabled Flag
        /// </summary>
        [BtsPropertyName("IsArchiveToFilePropertyName")]
        [BtsDescription("IsArchiveToFilePropertyDescription")]
        public bool IsArchiveToFile { get; set; }

        /// <summary>
        /// Backup Folder
        /// </summary>
        [BtsPropertyName("FileArchiveBackupFolderPropertyName")]
        [BtsDescription("FileArchiveBackupFolderPropertyDescription")]
        public string FileArchiveBackupFolder { get; set; }

        /// <summary>
        /// Connection User Name
        /// </summary>
        [BtsPropertyName("FileArchiveUserPropertyName")]
        [BtsDescription("FileArchiveUserPropertyDescription")]
        public string FileArchiveUserName { get; set; }

        /// <summary>
        /// Connection Domain
        /// </summary>
        [BtsPropertyName("FileArchiveUserDomainPropertyName")]
        [BtsDescription("FileArchiveUserDomainPropertyDescription")]
        public string FileArchiveUserDomain { get; set; }

        /// <summary>
        /// Connection Password
        /// </summary>
        [BtsPropertyName("FileArchiveUserPwdPropertyName")]
        [BtsDescription("FileArchiveUserPwdPropertyDescription")]
        public string FileArchiveUserPwd { get; set; }

        [BtsPropertyName("FileArchiveIsOverwriteFilesPropertyName")]
        [BtsDescription("FileArchiveIsOverwriteFilesPropertyDescription")]
        public bool FileArchiveIsOverwriteFiles { get; set; }

        #endregion Design-time properties

        #region BasePipelineComponent Members
       

        /// <summary>
        /// Load component properties from a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="errorLog">Error log level</param>
        protected override void LoadProperties(IPropertyBag propertyBag, int errorLog)
        {
            base.LoadProperties(propertyBag, errorLog);
          //  this.Optimized = this.ReadPropertyValue<bool>(propertyBag, OPTIMIZED_PROP_NAME, this.Optimized);
            this.IsArchiveToDb = this.ReadPropertyValue<bool>(propertyBag, ARCHIVE_TODB_ENABLED, this.IsArchiveToDb);
            this.DbConnStr = this.ReadPropertyValue<string>(propertyBag, ARCHIVE_DBCONNSTR, this.DbConnStr);
            this.IsArchiveToFile = this.ReadPropertyValue<bool>(propertyBag, ARCHIVE_TOFILE_ENABLED, this.IsArchiveToFile);
            this.ArchiveFileName = this.ReadPropertyValue<string>(propertyBag, ARCHIVE_FILE_NAME_PROP_NAME, this.ArchiveFileName);
            this.FileArchiveBackupFolder = this.ReadPropertyValue<string>(propertyBag, ARCHIVE_FILE_PATH, this.FileArchiveBackupFolder);
            this.FileArchiveIsOverwriteFiles = this.ReadPropertyValue<bool>(propertyBag, ARCHIVE_FILE_OVERWRITE, this.FileArchiveIsOverwriteFiles);
            this.FileArchiveUserName = this.ReadPropertyValue<string>(propertyBag, ARCHIVE_USER_NAME, this.FileArchiveUserName);
            this.FileArchiveUserDomain = this.ReadPropertyValue<string>(propertyBag, ARCHIVE_USER_DOMAIN, this.FileArchiveUserDomain);
            this.FileArchiveUserPwd = this.ReadPropertyValue<string>(propertyBag, ARCHIVE_USER_PASS, this.FileArchiveUserPwd);
        }

       
        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected override void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            base.SaveProperties(propertyBag, clearDirty, saveAllProperties);
            this.WritePropertyValue(propertyBag, ARCHIVE_TODB_ENABLED, this.IsArchiveToDb);
            this.WritePropertyValue(propertyBag, ARCHIVE_DBCONNSTR, this.DbConnStr);
            this.WritePropertyValue(propertyBag, ARCHIVE_TOFILE_ENABLED, this.IsArchiveToFile);
            this.WritePropertyValue(propertyBag, ARCHIVE_FILE_NAME_PROP_NAME, this.ArchiveFileName);
            this.WritePropertyValue(propertyBag, ARCHIVE_FILE_PATH, this.FileArchiveBackupFolder);
            this.WritePropertyValue(propertyBag, ARCHIVE_FILE_OVERWRITE, this.FileArchiveIsOverwriteFiles);
            this.WritePropertyValue(propertyBag, ARCHIVE_USER_NAME, this.FileArchiveUserName);
            this.WritePropertyValue(propertyBag, ARCHIVE_USER_DOMAIN, this.FileArchiveUserDomain);
            this.WritePropertyValue(propertyBag, ARCHIVE_USER_PASS, this.FileArchiveUserPwd);
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
            if (this.Enabled)
            {
                if (this.IsArchiveToFile)
                {
                    if (string.IsNullOrWhiteSpace(this.ArchiveFileName) == true)
                        errors.Add(Resources.ArchiverInvalidArchiveFileName);

                    if (string.IsNullOrWhiteSpace(this.FileArchiveBackupFolder) == true)
                        errors.Add(string.Format(Resources.ArchiverPathNotFoundError, this.FileArchiveBackupFolder));
                }
                if(this.IsArchiveToDb)
                {
                    if(string.IsNullOrWhiteSpace(this.DbConnStr) == true)
                    {
                        errors.Add(Resources.NoDBConnStrError);
                    }
                }
            }
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
            TraceProvider.Logger.TraceInfo("Archiving Message Start...");
            if (IsEnabled(inputMessage))
            {
                // get original stream
                Stream messageStream = inputMessage.BodyPart.GetOriginalDataStream();
                if (inputMessage.BodyPart != null)
                {
                    try
                    {
                        //Get Message Id
                        Guid msgId = inputMessage.MessageID;
                        
                        // create seekable stream to work on the message
                        messageStream = new ReadOnlySeekableStream(messageStream);
                        inputMessage.BodyPart.Data = messageStream;
                        long position = messageStream.Position;
                        //Get Provider
                        var provider = GetProvider();

                        //Check if Db Archive is enabled
                        if (IsArchiveToDb)
                        {
                            TraceProvider.Logger.TraceInfo("Archiving Message to DB");
                            string xmlStringProperties = GetMessageProperties(inputMessage.Context);
                            ArchiveToDb(provider, inputMessage.BodyPart.Data, xmlStringProperties, msgId);
                        }
                        //Archive to File
                        if (IsArchiveToFile)
                        {
                            TraceProvider.Logger.TraceInfo("Archiving Message to File");
                            ArchiveToFile(provider, msgId, inputMessage.Context, inputMessage.BodyPart.Data, inputMessage);
                        }
                        // restore the stream position
                        messageStream.Position = position;
                        // prevent the message stream from being GC collected
                        pipelineContext.ResourceTracker.AddResource(messageStream);
                    }
                    catch (Exception exc)
                    {
                        TraceProvider.Logger.TraceError("BizTalk Message Archiving Component", "Encountered an error: '{0}' : '{1}'", exc.Message, exc.ToString());
                    }
                    // get original stream
                    inputMessage.BodyPart.Data.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    TraceProvider.Logger.TraceInfo("Message has no body part, exiting");
                }
            }
            TraceProvider.Logger.TraceInfo("Archiving Message Exit."); 
            // return original message
            return inputMessage;
        }

        #endregion BasePipelineComponent Members

        #region Archive Methods
        /// <summary>
        /// Archive To File
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="msgId"></param>
        /// <param name="context"></param>
        /// <param name="msgStream"></param>
        protected virtual void ArchiveToFile(IMessageArchiveProvider provider, Guid msgId, IBaseMessageContext context, Stream msgStream, IBaseMessage inputMessage)
        {
            //Get Original FileName using FTP 
            string ftpNs = @"http://schemas.microsoft.com/BizTalk/2003/ftp-properties";
            string fileNs = @"http://schemas.microsoft.com/BizTalk/2003/file-properties";
            string name = "ReceivedFileName";

            //Get Filename by FileAdapter NS
            string fileName = GetPropertyContext(context, name, fileNs);
            if (string.IsNullOrEmpty(fileName))
            {
                //Get Filename by FTP NS
                fileName = GetPropertyContext(context, name, ftpNs);
            }
            string origFileName = "NoFileName.txt";
            string origFileNameExt = ".txt";
            if (string.IsNullOrEmpty(fileName))
            {

                fileName = origFileName;
            }
            else
            {
                //Get Filename without paths
                FileInfo fInfo = new FileInfo(fileName);
                origFileName = fInfo.Name;
                origFileNameExt = fInfo.Extension;
            }
            //Remove Extension
            origFileName = origFileName.Replace(origFileNameExt, "");

            //Update Filename based on macro's
            string archiveFileName =  FileNameMacroHelper.ResolveMacros(inputMessage,  ArchiveFileName);
               

            //Set the fileName to message Id if blank
            if (string.IsNullOrEmpty(archiveFileName))
                archiveFileName = msgId.ToString() + ".part";
            else
            {
                //Apply Extension
                archiveFileName += origFileNameExt;
            }
            provider.ArchiveToFile(archiveFileName, FileArchiveBackupFolder, FileArchiveIsOverwriteFiles, msgStream, FileArchiveUserName, FileArchiveUserPwd, FileArchiveUserDomain);
        }

        /// <summary>
        /// Archives the Msg to database
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="msgStream"></param>
        /// <param name="props"></param>
        /// <param name="msgId"></param>
        protected virtual void ArchiveToDb(IMessageArchiveProvider provider, Stream msgStream, string props, Guid msgId)
        {
            //Set Variables
            Stream compressedStream = null;
            long length = 0;

            msgStream.Position = 0;
            length = msgStream.Length;

            compressedStream = new MemoryStream(new byte[msgStream.Length]);
            msgStream.CopyTo(compressedStream);

            // Todo: Add try/catch and logging here
            //Archive to Database
            provider.ArchiveToDb(
                DbConnStr,
                DbConnProvider,
                DbSPName,
                msgId,
                compressedStream,
                props,
                length);
        }

        /// <summary>
        /// Returns the Message Properties based on DbPropList
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected string GetMessageProperties(IBaseMessageContext context)
        {
            //Get Msg Properties
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<MessageProperties>");
            string name;
            string nspace;
            for (int loop = 0; loop < context.CountProperties; loop++)
            {
                context.ReadAt(loop, out name, out nspace);
                string value = context.Read(name, nspace).ToString();
                sb.AppendLine(string.Format("<{0}>{1}</{0}>", name, value));
            }

            sb.AppendLine("</MessageProperties>");
            return sb.ToString();
        }

        #endregion Archive Methods

        #region Helper Methods

        /// <summary>
        /// Retrieves the value of Property from Message Context by Name and namespace
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected string GetPropertyContext(IBaseMessageContext context, string name, string ns)
        {
            string result = string.Empty;
            if (context != null)
            {
                try
                {
                    var res = context.Read(name, ns);
                    if (res != null)
                    {
                        result = res.ToString();
                    }
                }
                catch
                {
                    //Ignore and return empty string
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the Provider to be used Default or Built-in
        /// </summary>
        /// 
        /// <returns></returns>
        protected virtual IMessageArchiveProvider GetProvider()
        {
            IMessageArchiveProvider provider = null;
            try
            {

                provider = new MessageArchiver();
            }
            catch (Exception exc)
            {
                throw new Exception(string.Format("Unable to create Message Archive Provider, Message: '{10}', Details: '{1}'", exc.Message, exc.ToString()));
            }
            return provider;
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
                    using (FileStream fileArchiveStream = new FileStream(this.archiveFileFullPath, FileMode.Append, FileAccess.Write))
                    {
                        fileArchiveStream.Write(rargs.buffer, rargs.offset, rargs.bytesRead);
                    }
                }
            }
            catch (Exception ex)
            {
                // log a warning but do not rethrow to avoid stopping main message processing
                TraceProvider.Logger.TraceWarning(Resources.ArchiverExceptionMessage, this.archiveFileFullPath, ex.Message);
            }
        }

        #endregion Helpers
    }
}