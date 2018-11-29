using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using Microsoft.EnterpriseSingleSignOn.Interop;

namespace GT.BizTalk.SSO.AdminMMC.Management
{
    /// <summary>
    /// Holds the general metadata information about an SSO affiliate application.
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Constants.Namespace)]
    public class SSOAppInfo : IPropertyBag
    {
        #region Fields
        /// <summary>
        /// The standard generic dictionary used to store the application properties (metadata).
        /// In most cases, offers better performance than a HybridDictionary.
        /// </summary>
        private Dictionary<string, object> properties = new Dictionary<string, object>();
        #endregion

        #region Constructor
        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public SSOAppInfo()
        {
            // mark it as a Config Store type of application
            this.Flags |= SSOFlag.SSO_FLAG_APP_CONFIG_STORE;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets/sets the application name.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return this.Read<string>("appName"); }
            set { this.Write("appName", value); }
        }

        /// <summary>
        /// Gets/sets the application description.
        /// </summary>
        [XmlAttribute("description")]
        public string Description
        {
            get { return this.Read<string>("description"); }
            set { this.Write("description", value); }
        }

        /// <summary>
        /// Gets/sets the application contact.
        /// </summary>
        [XmlAttribute("contact")]
        public string Contact
        {
            get { return this.Read<string>("contact"); }
            set { this.Write("contact", value); }
        }

        /// <summary>
        /// Gets/sets a delimited list of accounts that can access the 
        /// application (users of the application).
        /// </summary>
        [XmlAttribute("userAccounts")]
        public string UserAccounts
        {
            get { return this.Read<string>("appUserAccount"); }
            set { this.Write("appUserAccount", value); }
        }

        /// <summary>
        /// Gets/sets a delimited list of accounts that can manage/administer
        /// the application.
        /// </summary>
        [XmlAttribute("adminAccounts")]
        public string AdminAccounts
        {
            get { return this.Read<string>("appAdminAccount"); }
            set { this.Write("appAdminAccount", value); }
        }

        /// <summary>
        /// Gets/sets the application flags.
        /// </summary>
        [XmlIgnore]
        public int Flags
        {
            get { return Convert.ToInt32(this.Read<uint>("flags")); }
            set
            {
                this.Write("flags", Convert.ToUInt32(value));
            }
        }

        /// <summary>
        /// Gets/sets the flag indicating whether the application configuration
        /// allows specifying local Windows accounts as users.
        /// </summary>
        /// <remarks>
        /// This sets/gets the appropriate flag in the Flags field.
        /// </remarks>
        [XmlAttribute("allowLocalAccounts")]
        public bool AllowLocalAccounts
        {
            get
            {
                return (this.Flags & SSOFlag.SSO_FLAG_APP_ALLOW_LOCAL) == SSOFlag.SSO_FLAG_APP_ALLOW_LOCAL;
            }
            set
            {
                if (value)
                    this.Flags |= SSOFlag.SSO_FLAG_APP_ALLOW_LOCAL;
                else
                    this.Flags &= ~SSOFlag.SSO_FLAG_APP_ALLOW_LOCAL;
            }
        }

        /// <summary>
        /// Gets/sets the flag indicating whether the application configuration
        /// uses the default SSO Affiliate Administrators as this application's
        /// administrators.
        /// </summary>
        /// <remarks>
        /// This sets/gets the appropriate flag in the Flags field.
        /// </remarks>
        [XmlAttribute("useSSOAffiliateAdmins")]
        public bool UseSSOAffiliateAdmins
        {
            get
            {
                return (this.Flags & SSOFlag.SSO_FLAG_APP_ADMIN_SAME) == SSOFlag.SSO_FLAG_APP_ADMIN_SAME;
            }
            set
            {
                if (value)
                    this.Flags |= SSOFlag.SSO_FLAG_APP_ADMIN_SAME;
                else
                    this.Flags &= ~SSOFlag.SSO_FLAG_APP_ADMIN_SAME;
            }
        }

        /// <summary>
        /// Gets/sets the flag indicating whether the application is enabled.
        /// </summary>
        /// <remarks>
        /// This sets/gets the appropriate flag in the Flags field.
        /// </remarks>
        [XmlAttribute("enabled")]
        public bool Enabled
        {
            get
            {
                return (this.Flags & SSOFlag.SSO_FLAG_ENABLED) == SSOFlag.SSO_FLAG_ENABLED;
            }
            set
            {
                if (value)
                    this.Flags |= SSOFlag.SSO_FLAG_ENABLED;
                else
                    this.Flags &= ~SSOFlag.SSO_FLAG_ENABLED;
            }
        }

        /// <summary>
        /// Gets the type of application.
        /// </summary>
        /// <remarks>
        /// The information is determined from the Flags field.
        /// </remarks>
        [XmlIgnore]
        public string Type
        {
            get { return GetType(this.Flags); }
        }

        /// <summary>
        /// Gets the Status of the application.
        /// </summary>
        /// <remarks>
        /// The information is determined from the Flags field.
        /// </remarks>
        [XmlIgnore]
        public string Status
        {
            get { return GetStatus(this.Flags); }
        }
        #endregion

        #region IPropertyBag Members
        /// <summary>
        /// Reads a property value from the configuration property bag.
        /// </summary>
        /// <param name="propName">Property name.</param>
        /// <param name="propValue">Property value.</param>
        /// <param name="errFlag">Error flag.</param>
        void IPropertyBag.Read(string propName, out object ptrVar, int errorLog)
        {
            // initialize the output value
            ptrVar = null;
            // check if the property is in the collection to return its value
            if (this.properties.ContainsKey(propName) == true)
            {
                ptrVar = this.properties[propName];
            }
        }

        /// <summary>
        /// Writes a property value.
        /// </summary>
        /// <param name="propName">Property name.</param>
        /// <param name="propValue">Property value.</param>
        void IPropertyBag.Write(string propName, ref object ptrVar)
        {
            this.properties[propName] = ptrVar;
        }
        #endregion

        #region Helpers
        private static string GetType(int flags)
        {
            if ((flags & SSOFlag.SSO_FLAG_APP_ADAPTER) == SSOFlag.SSO_FLAG_APP_ADAPTER)
            {
                if ((flags & SSOFlag.SSO_FLAG_APP_GROUP) == SSOFlag.SSO_FLAG_APP_GROUP)
                    return Properties.Resources.PasswordSyncGroupAdapterAppType;
                else
                    return Properties.Resources.PasswordSyncAdapterAppType;
            }
            else
            {
                if ((flags & SSOFlag.SSO_FLAG_APP_CONFIG_STORE) == SSOFlag.SSO_FLAG_APP_CONFIG_STORE)
                    return Properties.Resources.ConfigStoreAppType;
                if ((flags & SSOFlag.SSO_FLAG_APP_GROUP) == SSOFlag.SSO_FLAG_APP_GROUP)
                {
                    if ((flags & SSOFlag.SSO_FLAG_SSO_EXTERNAL_TO_WINDOWS) == SSOFlag.SSO_FLAG_SSO_EXTERNAL_TO_WINDOWS)
                        return Properties.Resources.HostGroupAppType;
                    else
                        return Properties.Resources.GroupAppType;
                }
                else
                    return Properties.Resources.IndividualAppType;
            }
        }

        private static string GetStatus(int flags)
        {
            if ((flags & SSOFlag.SSO_FLAG_ENABLED) == SSOFlag.SSO_FLAG_ENABLED)
                return Properties.Resources.StatusEnabled;
            else
                return Properties.Resources.StatusDisabled;
        }
        #endregion
    }
}
