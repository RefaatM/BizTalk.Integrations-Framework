using Microsoft.BizTalk.SSOClient.Interop;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GT.BizTalk.Framework.BizTalk.Configuration
{
    #region ConfigurationPropertyBag class

    /// <summary>
    /// Implements a configuration property bag.
    /// </summary>
    public class ConfigurationPropertyBag : IPropertyBag, IEnumerable
    {
        #region Fields

        /// <summary>
        /// The standard generic dictionary, in most cases, offers better performance than a HybridDictionary.
        /// </summary>
        private Dictionary<string, object> properties;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        internal ConfigurationPropertyBag()
        {
            properties = new Dictionary<string, object>();
        }

        #endregion Constructor

        #region IPropertyBag implementation

        /// <summary>
        /// Reads a property value from the configuration property bag.
        /// </summary>
        /// <param name="propName">Property name.</param>
        /// <param name="propValue">Property value.</param>
        /// <param name="errFlag">Error flag.</param>
        public void Read(string propName, out object propValue, int errFlag)
        {
            propValue = properties[propName];
        }

        /// <summary>
        /// Writes a property value.
        /// </summary>
        /// <param name="propName">Property name.</param>
        /// <param name="propValue">Property value.</param>
        public void Write(string propName, ref object propValue)
        {
            properties[propName] = propValue;
        }

        #endregion IPropertyBag implementation

        #region IEnumerable Members

        /// <summary>
        /// Gets the enumerator associated to this collection.
        /// </summary>
        /// <returns>Property names enumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return properties.Keys.GetEnumerator();
        }

        #endregion IEnumerable Members

        #region Additional methods

        /// <summary>
        /// Gets or sets the value associated with the specified property.
        /// </summary>
        /// <param name="propName">Property name to get or set.</param>
        /// <returns>The value associated with the specified property name.</returns>
        public object this[string propName]
        {
            get { return properties[propName]; }
            set { properties[propName] = value; }
        }

        /// <summary>
        /// Gets the number of configuration properties stored in the configuration property bag.
        /// </summary>
        public int Count
        {
            get { return properties.Count; }
        }

        /// <summary>
        /// Adds the specified property to the configuration property bag.
        /// </summary>
        /// <param name="propName">Property name to add.</param>
        /// <param name="propValue">Property value.</param>
        public void Add(string propName, object propValue)
        {
            properties.Add(propName, propValue);
        }

        /// <summary>
        /// Determines whether the configuration property bag contains the specified property.
        /// </summary>
        /// <param name="propName">Property name.</param>
        /// <returns>True if the configuration property bag contains a property with the specified name, false otherwise.</returns>
        public bool Contains(string propName)
        {
            return properties.ContainsKey(propName);
        }

        /// <summary>
        /// Removes the property with the specified name from the configuration property bag.
        /// </summary>
        /// <param name="propName">Property name.</param>
        public void Remove(string propName)
        {
            properties.Remove(propName);
        }

        #endregion Additional methods
    }

    #endregion ConfigurationPropertyBag class

    /// <summary>
    /// SSO Configuration Helper class.
    /// </summary>
    public static class SSOConfigHelper
    {
        #region Constants

        private const string IDENTIFIER_GUID = "ConfigProperties";

        #endregion Constants

        #region Fields

        private static Dictionary<string, ConfigurationPropertyBag> appConfigSettings = new Dictionary<string, ConfigurationPropertyBag>();
        private static readonly object syncLock = new object();

        #endregion Fields

        #region Public methods

        /// <summary>
        /// Reads the value of a configuration property from the specified application.
        /// </summary>
        /// <param name="appName">The name of the affiliate application.</param>
        /// <param name="propName">The property name to read.</param>
        /// <returns>The value of the property stored in the given affiliate application if the property exists, null otherwise.</returns>
        public static string Read(string appName, string propName)
        {
            return Read<string>(appName, propName, null);
        }

        /// <summary>
        /// Reads the value of a configuration property from the specified application.
        /// </summary>
        /// <typeparam name="T">Type of value to read.</typeparam>
        /// <param name="appName">The name of the affiliate application.</param>
        /// <param name="propName">The property name to read.</param>
        /// <returns>The value of the property stored in the given affiliate application if the property exists, null otherwise.</returns>
        public static T Read<T>(string appName, string propName)
        {
            return Read<T>(appName, propName, default(T));
        }

        /// <summary>
        /// Reads the value of a configuration property from the specified application.
        /// </summary>
        /// <typeparam name="T">Type of value to read.</typeparam>
        /// <param name="appName">The name of the affiliate application.</param>
        /// <param name="propName">The property name to read.</param>
        /// <param name="defaultValue">Default property value to return if the property is not found or is null.</param>
        /// <returns>The value of the property stored in the given affiliate application if the property exists, the default value otherwise.</returns>
        public static T Read<T>(string appName, string propName, T defaultValue)
        {
            try
            {
                // load configuration info from SSO store
                LoadSSOConfigInfo(appName);

                // read the property value from the SSO store
                object propValue = null;
                if (appConfigSettings.ContainsKey(appName) == true)
                {
                    // get application property bag
                    ConfigurationPropertyBag configPropertyBag = appConfigSettings[appName];
                    configPropertyBag.Read(propName, out propValue, 0);
                }

                // return property value if property exists, otherwise the default for type T
                if (propValue != null)
                {
                    if (typeof(T).IsEnum == true)
                    {
                        return (T)Enum.Parse(typeof(T), propValue.ToString());
                    }
                    else
                    {
                        return (T)Convert.ChangeType(propValue, typeof(T));
                    }
                }
                return defaultValue;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Reads all the configuration properties from teh specified application.
        /// </summary>
        /// <param name="appName">The name of the affiliate application.</param>
        /// <returns>The configuration properties for the specified SSO application.</returns>
        public static ConfigurationPropertyBag ReadAll(string appName)
        {
            try
            {
                // load configuration info from SSO store
                LoadSSOConfigInfo(appName);
                // get configuration property bag
                ConfigurationPropertyBag configPropertyBag = null;
                if (appConfigSettings.TryGetValue(appName, out configPropertyBag) == true)
                    return configPropertyBag;
                // not found
                return null;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Writes a property to the configuration store.
        /// </summary>
        /// <param name="appName">The name of the affiliate application.</param>
        /// <param name="propName">The property name to write.</param>
        /// <param name="propValue">The property value to write.</param>
        public static void Write(string appName, string propName, string propValue)
        {
            try
            {
                // load configuration info from SSO store
                LoadSSOConfigInfo(appName);
                // get configuration properties for specified application
                ConfigurationPropertyBag configPropertyBag = appConfigSettings[appName];
                // write property value to configuration property bag
                object tempProp = propValue;
                configPropertyBag.Write(propName, ref tempProp);
                // update property bag in the SSO store
                SSOConfigStore ssoStore = new SSOConfigStore();
                ((ISSOConfigStore)ssoStore).SetConfigInfo(appName, IDENTIFIER_GUID, configPropertyBag);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Writes a set of properties to the configuration store.
        /// </summary>
        /// <param name="appName">The name of the affiliate application.</param>
        /// <param name="configProperties">The configuration properties to write.</param>
        public static void WriteAll(string appName, ConfigurationPropertyBag configProperties)
        {
            try
            {
                lock (syncLock)
                {
                    // update property bag in the SSO store
                    SSOConfigStore ssoStore = new SSOConfigStore();
                    ((ISSOConfigStore)ssoStore).SetConfigInfo(appName, IDENTIFIER_GUID, configProperties);
                    // force a refresh of the cached configuration properties for the specified application
                    appConfigSettings.Remove(appName);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                throw;
            }
        }

        #endregion Public methods

        #region Helper methods

        /// <summary>
        /// Loads the configuration information for the specified affiliate application.
        /// </summary>
        /// <param name="appName">The name of the affiliate application.</param>
        private static void LoadSSOConfigInfo(string appName)
        {
            // NOTE: This is a classic implementation of the double-lock pattern in .NET
            // check if the application settings are in the cache
            if (appConfigSettings.ContainsKey(appName) == false)
            {
                // acquire a lock
                lock (syncLock)
                {
                    if (appConfigSettings.ContainsKey(appName) == false)
                    {
                        // get the configuration info from the SSO store
                        SSOConfigStore ssoStore = new SSOConfigStore();
                        ConfigurationPropertyBag configPropertyBag = new ConfigurationPropertyBag();
                        ((ISSOConfigStore)ssoStore).GetConfigInfo(appName, IDENTIFIER_GUID, SSOFlag.SSO_FLAG_RUNTIME, (IPropertyBag)configPropertyBag);
                        // add application configuration to cache
                        appConfigSettings.Add(appName, configPropertyBag);
                    }
                }
            }
        }

        #endregion Helper methods
    }
}