using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Windows.Forms;
using System.Xml;

using Microsoft.EnterpriseSingleSignOn.Interop;
using Microsoft.Win32;

using GT.BizTalk.SSO.AdminMMC.Extensions;
using GT.BizTalk.SSO.AdminMMC.Serialization;

namespace GT.BizTalk.SSO.AdminMMC.Management
{
    public static class SSOManager
    {
        #region Constants
        private const string CONTACT_INFO_FORMAT = "BizTalkAdmin@{0}.com";
        #endregion

        #region Fields
        private static string SSOSecrectServer = string.Empty;
        private static string SSODBServer = string.Empty;
        private static string SSODatabase = string.Empty;
        public static string SSOAffiliateAdminAccounts = string.Empty;
        public static string SSOAdminAccounts = string.Empty;

        public static readonly string DefaultContact = "{F1991B82-C64B-4421-8A89-F19C08D5D791}";
        public static readonly string ConfigIdentifier = "ConfigProperties";
        #endregion

        #region Constructor
        static SSOManager()
        {
            SSOManager.DefaultContact = string.Format(SSOManager.CONTACT_INFO_FORMAT, ConfigurationManager.AppSettings["CompanyName"]);
            SSOManager.LoadSSOServerInfo();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Enlists the specified SSO object into the specified transaction.
        /// </summary>
        /// <remarks>
        /// The IPropertyBag interface is supported on the Single Sign-On objects: 
        ///     SSOAdmin, SSOMapper, SSOMapping, SSOLookup, SSOConfigStore.
        /// Note:
        ///     This methods sets a DTC ITransaction pointer (as a VT_UNKNOWN) on the object model 
        ///     instance so that subsequent actions using that object are scoped within the DTC 
        ///     transaction.
        /// </remarks>
        /// <param name="ssoObj">Instance of the SSO object implementing the IPropertyBag interface to be enlisted.</param>
        /// <param name="tx">Transaction.</param>
        private static void Enlist(IPropertyBag ssoObj, Transaction tx)
        {
            object dtcTx = TransactionInterop.GetDtcTransaction(tx);
            object secretServerObj = SSOManager.SSOSecrectServer;

            ssoObj.Write("CurrentSSOServer", ref secretServerObj);
            ssoObj.Write("Transaction", ref dtcTx);
        }

        /// <summary>
        /// Loads the SSO server information.
        /// </summary>
        private static void LoadSSOServerInfo()
        {
            try
            {
                int flags;
                int auditAppDeleteMax;
                int auditMappingDeleteMax;
                int auditNtpLookupMax;
                int auditXpLookupMax;
                int ticketTimeout;
                int credCacheTimeout;

                ISSOAdmin2 ssoAdmin = new ISSOAdmin2();
                ssoAdmin.GetGlobalInfo(
                    out flags,
                    out auditAppDeleteMax,
                    out auditMappingDeleteMax,
                    out auditNtpLookupMax,
                    out auditXpLookupMax,
                    out ticketTimeout,
                    out credCacheTimeout,
                    out SSOManager.SSOSecrectServer,
                    out SSOManager.SSOAdminAccounts,
                    out SSOManager.SSOAffiliateAdminAccounts);

                SSOManager.SSODBServer = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\ENTSSO\\SQL", "Server", "") as string;
                SSOManager.SSODatabase = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\ENTSSO\\SQL", "Database", "") as string;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("HR - SSO MMC Snap - LoadSSOServerInfo", ex.Message);
            }
        }
        #endregion

        #region Application Management
        /// <summary>
        /// Gets the list of SSO applications. Only the application metadata information is returned.
        /// </summary>
        /// <returns>List of SSO applications.</returns>
        public static List<SSOAppInfo> GetApplications()
        {
            ISSOMapper2 ssoMapper = new ISSOMapper2();
            IPropertyBag propBag = ssoMapper as IPropertyBag;

            // prepare filter to only include config store application
            uint appFilterFlagMask = SSOFlag.SSO_FLAG_APP_FILTER_BY_TYPE;
            uint appFilterFlags = (uint)AffiliateApplicationType.ConfigStore;
            object appFilterFlagsObj = (object)appFilterFlags;
            object appFilterFlagMaskObj = (object)appFilterFlagMask;

            // set filter in the mapper
            propBag.Write("AppFilterFlags", ref appFilterFlagsObj);
            propBag.Write("AppFilterFlagMask", ref appFilterFlagMaskObj);

            // declare output arrays
            string[] applications = null;
            string[] descriptions = null;
            string[] contactInfo = null;
            string[] userAccounts = null;
            string[] adminAccounts = null;
            int[] flags = null;

            // get applications
            ssoMapper.GetApplications2(out applications, out descriptions, out contactInfo, out userAccounts, out adminAccounts, out flags);

            List<SSOAppInfo> applicationList = new List<SSOAppInfo>();
            for (int index = 0; index < applications.Length; ++index)
            {
                // include only applications created by this tool
                if (SSOManager.DefaultContact.Equals(contactInfo[index], StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    SSOAppInfo appInfo = new SSOAppInfo()
                    {
                        Name = applications[index],
                        Description = descriptions[index],
                        AdminAccounts = adminAccounts[index],
                        UserAccounts = userAccounts[index],
                        Contact = contactInfo[index],
                        Flags = flags[index],
                    };
                    applicationList.Add(appInfo);
                }
            }
            return applicationList;
        }

        /// <summary>
        /// Checks whether the specified application exists.
        /// </summary>
        /// <param name="appName">The name of the application.</param>
        /// <returns><b>true</b> if the application exists; otherwise <b>false</b>.</returns>
        public static bool ApplicationExists(string appName)
        {
            SSOAppInfo appInfo = SSOManager.GetApplicationInfo(appName);
            return (appInfo != null);
        }

        /// <summary>
        /// Retrieves the specified application configuration from the SSO store,
        /// including both, the application metadata information and the application
        /// fields.
        /// </summary>
        /// <param name="appName">The name of the application to retrieve.</param>
        /// <returns>The instance of the SSOAppConfig.</returns>
        public static SSOAppConfig GetApplicationConfig(string appName)
        {
            return new SSOAppConfig()
            {
                AppInfo = SSOManager.GetApplicationInfo(appName),
                AppFields = SSOManager.GetApplicationFields(appName)
            };
        }

        /// <summary>
        /// Retrieves the specified application metadata information from the SSO store.
        /// </summary>
        /// <param name="appName">The name of the application to retrieve.</param>
        /// <returns>The instance of the SSOAppInfo.</returns>
        public static SSOAppInfo GetApplicationInfo(string appName)
        {
            try
            {
                SSOAppInfo appInfo = new SSOAppInfo() { Name = appName };
                ISSOAdmin2 ssoAdmin = new ISSOAdmin2();
                ssoAdmin.GetApplicationInfo2(appName, appInfo);
                return appInfo; 
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves the specified application fields from the SSO store.
        /// </summary>
        /// <param name="appName">The name of the application to retrieve.</param>
        /// <returns>The instance of the SSOAppFieldCollection.</returns>
        public static SSOAppFieldCollection GetApplicationFields(string appName)
        {
            SSOAppFieldCollection appFields = new SSOAppFieldCollection();
            ISSOConfigStore configStore = new ISSOConfigStore();
            configStore.GetConfigInfo(appName, SSOManager.ConfigIdentifier, SSOFlag.SSO_FLAG_RUNTIME, appFields);
            foreach(SSOAppField field in appFields)
            {
                field.Identifier = SSOManager.ConfigIdentifier;
            }
            return appFields;
        }

        /// <summary>
        /// Enables/disables the specified application.
        /// </summary>
        /// <param name="appName">The name of the application.</param>
        /// <param name="enable">Value indicating whether to enable or disable the application.</param>
        public static void EnableApplication(string appName, bool enable)
        {
            ISSOAdmin2 ssoAdmin = new ISSOAdmin2();
            int flagMask = SSOFlag.SSO_FLAG_ENABLED;
            int flags = enable ? SSOFlag.SSO_FLAG_ENABLED : SSOFlag.SSO_FLAG_NONE;
            ssoAdmin.UpdateApplication(appName, null, null, null, null, flags, flagMask);
        }

        /// <summary>
        /// Creates a new application in the SSO store using specified
        /// the configuration information. Creates both, the application 
        /// and fields.
        /// </summary>
        /// <param name="appConfig">Configuration information used to create the application.</param>
        public static void CreateApplication(SSOAppConfig appConfig)
        {
            // create a transaction
            using (TransactionScope transactionScope = new TransactionScope())
            {
                // create SSO objects
                ISSOAdmin2 ssoAdmin = new ISSOAdmin2();

                // enlist them in the transaction
                SSOManager.Enlist(ssoAdmin as IPropertyBag, Transaction.Current);

                // create the sso application
                SSOManager.CreateApplication(ssoAdmin, appConfig);

                // commit the transaction
                transactionScope.Complete();
            }

            // update the application fields
            ISSOConfigStore ssoConfigStore = new ISSOConfigStore();
            //SSO.Enlist(ssoConfigStore as IPropertyBag, Transaction.Current);
            ssoConfigStore.SetConfigInfo(appConfig.AppInfo.Name, SSOManager.ConfigIdentifier, appConfig.AppFields);
        }

        /// <summary>
        /// Creates a new application in the SSO store using specified
        /// ISSOAdmin2 object instance and the configuration information. 
        /// Creates both, the application and fields.
        /// </summary>
        /// <remarks>
        /// For internal use only. Assumes a TransactionScope is created before 
        /// calling this method.
        /// </remarks>
        /// <param name="ssoAdmin">ISSOAdmin2 instance.</param>
        /// <param name="appConfig">Configuration information used to create the application.</param>
        private static void CreateApplication(ISSOAdmin2 ssoAdmin, SSOAppConfig appConfig)
        {
            // set default contact
            appConfig.AppInfo.Contact = SSOManager.DefaultContact;

            // fix the fieldCount when creating an application with no fields
            int fieldCount = Math.Max(1, appConfig.AppFields.Count);

            // create the sso application
            ssoAdmin.CreateApplication(
                appConfig.AppInfo.Name,
                appConfig.AppInfo.Description,
                appConfig.AppInfo.Contact,
                appConfig.AppInfo.UserAccounts,
                appConfig.AppInfo.AdminAccounts,
                appConfig.AppInfo.Flags & ~SSOFlag.SSO_FLAG_ENABLED, // the SSO_FLAG_ENABLED flag cannot be specified when creating an SSO application
                fieldCount);

            // create dummy field in the first slot
            ssoAdmin.CreateFieldInfo(appConfig.AppInfo.Name, "(unused)", SSOFlag.SSO_FLAG_NONE);
            // create the actual fields
            foreach (SSOAppField field in appConfig.AppFields)
            {
                // set field flags
                int fieldFlags = SSOFlag.SSO_FLAG_NONE;
                if (field.Masked == true)
                    fieldFlags |= SSOFlag.SSO_FLAG_FIELD_INFO_MASK;
                // create it
                ssoAdmin.CreateFieldInfo(appConfig.AppInfo.Name, field.Name, fieldFlags);
            }

            // enable application
            if (appConfig.AppInfo.Enabled == true)
            {
                ssoAdmin.UpdateApplication(appConfig.AppInfo.Name, null, null, null, null, SSOFlag.SSO_FLAG_ENABLED, SSOFlag.SSO_FLAG_ENABLED);
            }
        }

        /// <summary>
        /// Updates the specified application including both, metadata information 
        /// and fields.
        /// </summary>
        /// <param name="appConfig">Configuration information used to update the application.</param>
        /// <param name="recreate">Value indicating wheter to recreate the application.</param>
        public static void UpdateApplication(SSOAppConfig appConfig, bool recreate)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                // create SSO objects
                ISSOAdmin2 ssoAdmin = new ISSOAdmin2();
                ISSOConfigStore ssoConfigStore = new ISSOConfigStore();

                // enlist them in the transaction
                SSOManager.Enlist(ssoAdmin as IPropertyBag, Transaction.Current);
                SSOManager.Enlist(ssoConfigStore as IPropertyBag, Transaction.Current);

                // check if the application needs to be recreated or just updated
                if (recreate == true)
                {
                    // delete and recreate
                    SSOManager.DeleteApplication(ssoAdmin, appConfig.AppInfo.Name);
                    SSOManager.CreateApplication(ssoAdmin, appConfig);
                }
                else
                {
                    // just update the application metadata
                    SSOManager.UpdateApplicationInfo(ssoAdmin, appConfig.AppInfo);
                }

                // update the application fields
                ssoConfigStore.SetConfigInfo(appConfig.AppInfo.Name, SSOManager.ConfigIdentifier, appConfig.AppFields);
                // commit the transaction
                transactionScope.Complete();
            }
        }

        /// <summary>
        /// Updates the application metadata information.
        /// </summary>
        /// <param name="appInfo">Application metadata information.</param>
        public static void UpdateApplicationInfo(SSOAppInfo appInfo)
        {
            ISSOAdmin2 ssoAdmin = new ISSOAdmin2();
            SSOManager.UpdateApplicationInfo(ssoAdmin, appInfo);
        }

        /// <summary>
        /// Updates the application metadata information.
        /// </summary>
        /// <remarks>
        /// For internal use only. Assumes a TransactionScope is created before 
        /// calling this method.
        /// </remarks>
        /// <param name="ssoAdmin">ISSOAdmin2 instance.</param>
        /// <param name="appInfo">Application metadata information.</param>
        private static void UpdateApplicationInfo(ISSOAdmin2 ssoAdmin, SSOAppInfo appInfo)
        {
            // create a separate instance of the SSOAppInfo object
            // to only copy the information that is updatable
            SSOAppInfo appInfoUpdate = new SSOAppInfo();
            appInfoUpdate.Description = appInfo.Description;
            appInfoUpdate.Contact = appInfo.Contact;
            appInfoUpdate.UserAccounts = appInfo.UserAccounts;
            if (appInfo.UseSSOAffiliateAdmins == false)
                appInfoUpdate.AdminAccounts = appInfo.AdminAccounts;

            // update the application
            ssoAdmin.UpdateApplication2(appInfo.Name, appInfoUpdate);
        }

        public static void UpdateApplicationFields(string appName, SSOAppFieldCollection appFields, bool recreate)
        {
            if (recreate == true)
            {
                SSOAppConfig appConfig = new SSOAppConfig()
                {
                    // load the current application metadata information
                    AppInfo = SSOManager.GetApplicationInfo(appName),
                    // and use the application fields provided
                    AppFields = appFields
                };

                // update/recreate the application including the fields
                SSOManager.UpdateApplication(appConfig, true);
            }
            else
            {
                // just update the application fields
                ISSOConfigStore ssoConfigStore = new ISSOConfigStore();
                ssoConfigStore.SetConfigInfo(appName, SSOManager.ConfigIdentifier, appFields);
            }
        }

        /// <summary>
        /// Deletes the specified application.
        /// </summary>
        /// <param name="appName">The name of the application to be deleted.</param>
        public static void DeleteApplication(string appName)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                ISSOAdmin2 ssoAdmin = new ISSOAdmin2();
                SSOManager.Enlist(ssoAdmin as IPropertyBag, Transaction.Current);
                SSOManager.DeleteApplication(ssoAdmin, appName);
                transactionScope.Complete();
            }
        }

        /// <summary>
        /// Deletes the specified application.
        /// </summary>
        /// <remarks>
        /// For internal use only. Assumes a TransactionScope is created before 
        /// calling this method.
        /// </remarks>
        /// <param name="ssoAdmin">ISSOAdmin2 instance.</param>
        /// <param name="appName">The name of the application to be deleted.</param>
        private static void DeleteApplication(ISSOAdmin2 ssoAdmin, string appName)
        {
            ssoAdmin.DeleteApplication(appName);
        }

        /// <summary>
        /// Purges the cached credentials for an application on all Enterprise Single Sign-On (SSO) servers.
        /// </summary>
        /// <param name="appName">The name of the application.</param>
        public static void PurgeApplicationCache(string appName)
        {
            ISSOAdmin2 ssoAdmin = new ISSOAdmin2();
            ssoAdmin.PurgeCacheForApplication(appName);
        }
        #endregion

        #region Import/Export
        /// <summary>
        /// Exports the specified SSO application into the specified file in XML format.
        /// </summary>
        /// <param name="appName">The name of the application to be deleted.</param>
        /// <param name="filePath">Export file path.</param>
        public static void ExportApplication(string appName, string filePath)
        {
            // load the specified SSO application configuration
            SSOAppConfig appConfig = SSOManager.GetApplicationConfig(appName);
            // save it into the specified file
            XmlSerializationUtil.SaveXml(appConfig, filePath);
        }

        /// <summary>
        /// Imports the specified file containing an XML configuration of the SSO application.
        /// </summary>
        /// <param name="filePath">Import file path.</param>
        /// <param name="overrideApp">Value indicating whether to override the application if it already exists.</param>
        /// <returns><b>true</b> if the application imported successfully; otherwise <b>false</b>.</returns>
        public static bool ImportApplication(string filePath, bool overrideApp)
        {
            // load the app configuration from the file
            SSOAppConfig appConfig = XmlSerializationUtil.LoadXml<SSOAppConfig>(filePath);
            // check if the application already exists
            if (SSOManager.ApplicationExists(appConfig.AppInfo.Name) == true)
            {
                if (overrideApp == true)
                {
                    // update/recreate the application
                    SSOManager.UpdateApplication(appConfig, true);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                // create a new application
                SSOManager.CreateApplication(appConfig);
            }
            return true;
        }
        #endregion

        #region Encryption
        private static string Encrypt(string toEncrypt, string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(key));
            TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider.Key = hash;
            cryptoServiceProvider.Mode = CipherMode.ECB;
            cryptoServiceProvider.Padding = PaddingMode.PKCS7;
            byte[] inArray = cryptoServiceProvider.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            return Convert.ToBase64String(inArray, 0, inArray.Length);
        }

        private static string Decrypt(string toDecrypt, string key)
        {
            byte[] inputBuffer = Convert.FromBase64String(toDecrypt);
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(key));
            TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider.Key = hash;
            cryptoServiceProvider.Mode = CipherMode.ECB;
            cryptoServiceProvider.Padding = PaddingMode.PKCS7;
            return Encoding.UTF8.GetString(cryptoServiceProvider.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length));
        }
        #endregion
    }
}
