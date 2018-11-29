using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GT.BizTalk.Framework.BizTalk.Serialization
{
    /// <summary>
    /// Used to serialize the XPathProperty collection as XML.
    /// </summary>
    [XmlRoot(ElementName = "PropertyBag", Namespace = "http://GT.BizTalk.Framework.BizTalk.Serialization/2015")]
    public class PropertyBagSerializer : XmlPropertyCollectionSerializer<PropertyBag>
    {
        public PropertyBagSerializer()
            : base()
        {
        }

        public PropertyBagSerializer(List<PropertyBag> properties)
            : base(properties)
        {
        }
    }

    /// <summary>
    /// Represents a context property.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PropertyBag
    {
        #region Properties

        /// <summary>
        /// The property bag key name.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The property bag name.")]
        [NotifyParentProperty(true)]
        public string Name { get; set; }

        /// <summary>
        /// The property bag value.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The property bag value.")]
        [NotifyParentProperty(true)]
        public string Value { get; set; }

        #endregion Properties

        #region Object Overrides

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return string.Format("Name = {0}, Value = {1}", this.Name, this.Value);
        }

        #endregion Object Overrides
    }
}