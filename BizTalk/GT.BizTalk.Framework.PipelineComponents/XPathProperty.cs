using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GT.BizTalk.Framework.PipelineComponents
{
    /// <summary>
    /// Used to serialize the XPathProperty collection as XML.
    /// </summary>
    [XmlRoot(ElementName = "XPathProperties", Namespace = "https://schemas.DirectEnergy.com/Unify/Common/pipelinecomponents/2015")]
    public class XPathPropertySerializer : XmlPropertyCollectionSerializer<XPathProperty>
    {
        public XPathPropertySerializer()
            : base()
        {
        }

        public XPathPropertySerializer(List<XPathProperty> properties)
            : base(properties)
        {
        }
    }

    /// <summary>
    /// Represents an xpath property.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class XPathProperty
    {
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
        /// The context property namespace.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The context property namespace.")]
        [NotifyParentProperty(true)]
        public string Namespace { get; set; }

        /// <summary>
        /// The context property xpath expression.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The xpath expression to extract the property from the message content.")]
        [NotifyParentProperty(true)]
        public string XPath { get; set; }

        #endregion Properties

        #region Object Overrides

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return string.Format("Name = {0}, Promote = {1}, Namespace = {2}, XPath = {3}", this.Name, this.Promote, this.Namespace, this.XPath);
        }

        #endregion Object Overrides
    }
}