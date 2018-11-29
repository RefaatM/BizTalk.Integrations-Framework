namespace GT.BizTalk.Framework.Core.Mail
{
    /// <summary>
    /// Stores information about a linked resource.
    /// </summary>
    public class LinkedResourceInfo : ILinkedResourceInfo
    {
        #region Properties

        /// <summary>
        /// Gets the linked resource filename.
        /// </summary>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the linked resource media type.
        /// </summary>
        public string MediaType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the linked resource ContentId used to reference the resource within the message body.
        /// </summary>
        public string ContentId
        {
            get;
            set;
        }

        #endregion Properties

        #region ILinkedResourceInfo Members

        string ILinkedResourceInfo.FileName
        {
            get { return this.FileName; }
        }

        string ILinkedResourceInfo.MediaType
        {
            get { return this.MediaType; }
        }

        string ILinkedResourceInfo.ContentId
        {
            get { return this.ContentId; }
        }

        #endregion ILinkedResourceInfo Members
    }
}