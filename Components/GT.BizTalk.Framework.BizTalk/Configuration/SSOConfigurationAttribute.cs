using System;
using System.Runtime.InteropServices;

namespace GT.BizTalk.Framework.BizTalk.Configuration
{
    /// <summary>
    /// Defines the name of the SSO Application containing the configuration settings of the assembly.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Assembly, Inherited = false)]
    [ComVisibleAttribute(true)]
    public class SSOConfigurationAttribute : Attribute
    {
        #region Constructor

        /// <summary>
        /// Instance constructor.
        /// </summary>
        /// <param name="appName">The name of the SSO Application containing the configuration settings of the assembly.</param>
        public SSOConfigurationAttribute(string appName)
        {
            this.ApplicationName = appName;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the name of the SSO Application containing the configuration settings of the assembly.
        /// </summary>
        public string ApplicationName
        {
            get;
            private set;
        }

        #endregion Properties
    }
}