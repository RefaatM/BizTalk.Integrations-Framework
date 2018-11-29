namespace GT.BizTalk.Framework.Core.Mail
{
    /// <summary>
    /// Defines the interface for the attachment information.
    /// </summary>
    public interface IAttachmentInfo
    {
        #region Properties

        /// <summary>
        /// Gets the attachment filename.
        /// </summary>
        string FileName
        {
            get;
        }

        /// <summary>
        /// Gets the attachment media type.
        /// </summary>
        string MediaType
        {
            get;
        }

        #endregion Properties
    }
}