using GT.BizTalk.Framework.Core.Tracing;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;

namespace GT.BizTalk.Framework.BizTalk
{
    /// <summary>
    /// Provides Read/Write extensions methods to the IBaseMessage type.
    /// </summary>
    public static class IBasePropertyBagExtensions
    {
        /// <summary>
        /// Reads property value from message context.
        /// </summary>
        /// <param name="context">Message context.</param>
        /// <param name="propertyName">Name of property.</param>
        /// <param name="propertyNamespace">Namespace of the property.</param>
        /// <param name="defaultValue">Default value returned if the conversion cannot be performed.</param>
        /// <returns>Value of the property.</returns>
        public static T Read<T>(this IBasePropertyBag context, string propertyName, string propertyNamespace, T defaultValue)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");
            if (string.IsNullOrEmpty(propertyNamespace))
                throw new ArgumentNullException("propertyNamespace");
            try
            {
                object value = context.Read(propertyName, propertyNamespace);
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
            catch
            {
                // return default value, if property cannot be read
                return defaultValue;
            }
        }

        /// <summary>
        /// Reads all the context properties from the message context into the given dictionary.
        /// </summary>
        /// <param name="context">Message context.</param>
        /// <param name="contextProperties">Dictionary where the context properties will be copied to.</param>
        public static void ReadAll(this IBasePropertyBag context, Dictionary<string, object> contextProperties)
        {
            for (int i = 0; i < context.CountProperties; i++)
            {
                string propName = string.Empty;
                string propNamespace = string.Empty;
                string key = string.Empty;
                try
                {
                    object val = context.ReadAt(i, out propName, out propNamespace);
                    key = string.Format("{0}#{1}", propNamespace, propName);
                    // add or update value
                    contextProperties[key] = val;
                }
                catch (Exception ex)
                {
                    TraceProvider.Logger.TraceError("Namespace: {0}\nName: {1}\n caused an exception:\n{2}\nThis item was not added to the collection.", propNamespace, propName, ex.Message);
                }
            }
        }
    }
}