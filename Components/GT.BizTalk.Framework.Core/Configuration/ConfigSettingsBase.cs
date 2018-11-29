using System;
using System.Configuration;

namespace GT.BizTalk.Framework.Core.Configuration
{
    /// <summary>
    /// Base class for custom configuration setting sections.
    /// </summary>
    [Serializable]
    public class ConfigSettingsBase<T> : ConfigurationSection
        where T : ConfigSettingsBase<T>
    {
        #region Constructors

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public ConfigSettingsBase()
        {
        }

        #endregion Constructors

        #region Protected Helper

        /// <summary>
        /// Gets the instance to the specified section.
        /// </summary>
        /// <param name="sectionName">Section name.</param>
        protected static T GetSection(string sectionName)
        {
            // get the specified section
            return (T)ConfigurationManager.GetSection(sectionName);
        }

        #endregion Protected Helper
    }
}