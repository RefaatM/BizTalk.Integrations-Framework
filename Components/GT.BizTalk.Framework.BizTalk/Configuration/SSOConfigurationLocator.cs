using System;
using System.Reflection;

namespace GT.BizTalk.Framework.BizTalk.Configuration
{
    /// <summary>
    /// Helper class to locate the SSO Application name containing the configuration information.
    /// </summary>
    public static class SSOConfigurationLocator
    {
        /// <summary>
        /// Gets the name of the SSO Application containing the configuration settings.
        /// </summary>
        /// <remarks>
        /// Gets the SSO Application name from the calling assembly.
        /// </remarks>
        /// <returns>The name of the SSO Application containing the configuration settings.</returns>
        public static string GetApplicationName()
        {
            // get calling assembly
            Assembly assembly = Assembly.GetCallingAssembly();

            // get SSOConfiguration attributes
            SSOConfigurationAttribute[] attributes = (SSOConfigurationAttribute[])assembly.GetCustomAttributes(typeof(SSOConfigurationAttribute), false);
            if (attributes.Length == 1)
            {
                // get the name of the SSO Application containing the configuration settings
                return attributes[0].ApplicationName;
            }

            // SSOConfiguration attribute not found in calling assembly
            throw new InvalidOperationException("Could not find SSOConfiguration attribute in the calling assembly.");
        }
    }
}