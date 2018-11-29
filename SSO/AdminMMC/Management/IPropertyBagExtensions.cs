using System;

using Microsoft.EnterpriseSingleSignOn.Interop;

namespace GT.BizTalk.SSO.AdminMMC.Management
{
    /// <summary>
    /// Provides Read/Write extensions methods to the IPropertyBag type.
    /// </summary>
    public static class IPropertyBagExtensions
    {
        /// <summary>
        /// Reads property value from property bag.
        /// </summary>
        /// <param name="propBag">Property bag.</param>
        /// <param name="propName">Name of property.</param>
        /// <returns>Value of the property.</returns>
        public static object Read(this IPropertyBag propBag, string propName)
        {
            object val = null;
            try
            {
                propBag.Read(propName, out val, 0);
            }
            catch (ArgumentException)
            {
                return val;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return val;
        }

        /// <summary>
        /// Reads the property value from the property bag into a value of type T.
        /// If the conversion cannot be performed, a default value will be returned.
        /// </summary>
        /// <typeparam name="T">Type to which the string value will be converted to.</typeparam>
        /// <param name="propertyBag">Property bag.</param>
        /// <param name="propertyName">Name of property.</param>
        /// <param name="defaultValue">Optional default value returned if the conversion cannot be performed.</param>
        /// <returns>Value of the property.</returns>
        public static T Read<T>(this IPropertyBag propertyBag, string propertyName, T defaultValue = default(T))
        {
            if (propertyBag == null)
                throw new ArgumentNullException("propertyBag");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");
            try
            {
                object value = propertyBag.Read(propertyName);
                if (value != null)
                {
                    if (typeof(T).IsEnum == true)
                    {
                        return (T)Enum.Parse(typeof(T), value.ToString());
                    }
                    else
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                }
                return defaultValue;
            }
            catch (ArgumentException)
            {
                // return default value, if property cannot be read
                return defaultValue;
            }
        }

        /// <summary>
        /// Writes a property to the property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="value">Value.</param>
        public static void Write(this IPropertyBag propertyBag, string propertyName, object value)
        {
            propertyBag.Write(propertyName, ref value);
        }
    }
}
