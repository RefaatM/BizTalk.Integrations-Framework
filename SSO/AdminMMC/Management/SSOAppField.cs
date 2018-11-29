using System;
using System.Xml.Serialization;

namespace GT.BizTalk.SSO.AdminMMC.Management
{
    /// <summary>
    /// Holds the configuration information for an application field.
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Constants.Namespace)]
    public partial class SSOAppField
    {
        #region Properties
        /// <summary>
        /// Gets/sets the field ordinal position.
        /// </summary>
        [XmlAttribute("ordinal")]
        public int Ordinal
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the field name.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets a value indicating if the field value is masked.
        /// </summary>
        [XmlAttribute("masked")]
        public bool Masked
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the field value.
        /// </summary>
        [XmlAttribute("value")]
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the field identifier.
        /// </summary>
        [XmlAttribute("identifier")]
        public string Identifier
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the value of the field converting to the specified type.
        /// </summary>
        /// <typeparam name="T">Type the value will be converted to.</typeparam>
        /// <returns>Value converted to the specified type.</returns>
        public T GetValue<T>()
        {
            try
            {
                if (string.IsNullOrEmpty(this.Value) == false)
                    return (T)Convert.ChangeType(this.Value, typeof(T));
                else
                    return default(T);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Sets the value of the field.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">The new value.</param>
        public void SetValue<T>(T value)
        {
            if (value != null)
                this.Value = value.ToString();
            else
                this.Value = null;
        }
        #endregion
    }
}
