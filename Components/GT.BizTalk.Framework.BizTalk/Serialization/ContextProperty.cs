using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GT.BizTalk.Framework.BizTalk.Serialization
{
    /// <summary>
    /// Used to serialize the XPathProperty collection as XML.
    /// </summary>
    [XmlRoot(ElementName = "ContextProperty", Namespace = "http://GT.BizTalk.Framework.BizTalk.Serialization/2015")]
    public class ContextPropertySerializer : XmlPropertyCollectionSerializer<ContextProperty>
    {
        public ContextPropertySerializer()
            : base()
        {
        }

        public ContextPropertySerializer(List<ContextProperty> properties)
            : base(properties)
        {
        }
    }

    /// <summary>
    /// Defines the source of the context property.
    /// </summary>
    public enum ContextPropertySource
    {
        /// <summary>
        /// The source is a literal value specified in the Value field.
        /// </summary>
        Literal,

        /// <summary>
        /// The source is a value from the message context.
        /// </summary>
        Context,

        /// <summary>
        /// The source is a C# expression.
        /// </summary>
        Expression,

        /// <summary>
        /// The source is an x-path expression.
        /// </summary>
        XPath
    }

    /// <summary>
    /// Represents a context property.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ContextProperty
    {
        public ContextProperty()
        {
        }

        public ContextProperty(string propertyName, string propertyNamespace)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            if (string.IsNullOrEmpty(propertyNamespace))
            {
                throw new ArgumentNullException("propertyNamespace");
            }

            Name = propertyName;
            Namespace = propertyNamespace;
        }

        public ContextProperty(string property)
        {
            if (string.IsNullOrEmpty(property))
            {
                throw new ArgumentNullException("property");
            }

            if (!property.Contains("#"))
            {
                throw new ArgumentException("The property path {0} is not valid", property);
            }

            Namespace = property.Split('#')[0];
            Name = property.Split('#')[1];
        }

        #region Properties

        /// <summary>
        /// Indicates if the value should be promoted to the context or just written.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Indicates if the value should be promoted to the context or just written.")]
        [NotifyParentProperty(true)]
        public bool Promote { get; set; }

        /// <summary>
        /// The context property key name.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The context property name.")]
        [NotifyParentProperty(true)]
        public string Name { get; set; }

        /// <summary>
        /// The context property value.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(ContextPropertySource.Literal)]
        [Description("The context property source.")]
        [NotifyParentProperty(true)]
        public ContextPropertySource Source { get; set; }

        /// <summary>
        /// The context property namespace.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The context property namespace.")]
        [NotifyParentProperty(true)]
        public string Namespace { get; set; }

        /// <summary>
        /// The context property value.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The context property value.")]
        [NotifyParentProperty(true)]
        public string Value { get; set; }

        /// <summary>
        /// Ignore Null or Empty Value
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [DisplayName("Ignore Null/Empty")]
        [Description("Indicates if promoting/writing the property to the context should be ignored if the value is null or empty.")]
        [NotifyParentProperty(true)]
        public bool IgnoreNullOrEmptyValue { get; set; }

        /// <summary>
        /// Indicates if the value has been changed.
        /// </summary>
        public bool Dirty { get; set; }

        /// <summary>
        /// The Data Type of the property
        /// </summary>
        public string DataType { get; set; }

        #endregion Properties

        #region Object Overrides

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return string.Format("Name = {0}, Namespace = {1}", this.Name, this.Namespace);
        }

        #endregion Object Overrides
    }
}