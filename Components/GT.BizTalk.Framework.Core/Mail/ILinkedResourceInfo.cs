namespace GT.BizTalk.Framework.Core.Mail
{
    /// <summary>
    /// Defines the interface for the linked resource information.
    /// </summary>
    public interface ILinkedResourceInfo
    {
        #region Properties

        /// <summary>
        /// Gets the linked resource filename.
        /// </summary>
        string FileName
        {
            get;
        }

        /// <summary>
        /// Gets the linked resource media type.
        /// </summary>
        string MediaType
        {
            get;
        }

        /// <summary>
        /// Gets the linked resource ContentId used to reference the resource within the message body.
        /// </summary>
        string ContentId
        {
            get;
        }

        #endregion Properties
    }
}