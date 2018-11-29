using System;
using System.Xml.Serialization;

namespace GT.BizTalk.SSO.AdminMMC.Management
{
    /// <summary>
    /// Holds the complete configuration information about an SSO affiliate application,
    /// including the application general metadata and the fields.
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Constants.Namespace)]
    [XmlRoot("ssoAppConfig", Namespace = Constants.Namespace, IsNullable = false)]
    public class SSOAppConfig
    {
        #region Constructors
        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public SSOAppConfig()
        {
            this.AppInfo = new SSOAppInfo();
            this.AppFields = new SSOAppFieldCollection();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets/sets the SSOAppInfo containing general information about the SSO application.
        /// </summary>
        [XmlElement("appInfo")]
        public SSOAppInfo AppInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the list of SSOAppField containing information about each of the application
        /// fields.
        /// </summary>
        [XmlArray("appFields")]
        [XmlArrayItem("field", IsNullable = false)]
        public SSOAppFieldCollection AppFields
        {
            get;
            set;
        }
        #endregion
    }
}
