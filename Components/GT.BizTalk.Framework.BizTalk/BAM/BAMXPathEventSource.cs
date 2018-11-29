using DE.DAXFSA.Framework.BizTalk.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DE.DAXFSA.Framework.BizTalk.BAM
{
    /// <summary>
    /// Used to serialize the XPathProperty collection as XML.
    /// </summary>
    [XmlRoot(ElementName = "BAMXPathEventSource", Namespace = "http://holtrenfrew/ecommerce/eai/bam/2015")]
    public class BAMXPathEventSourceSerializer : XmlPropertyCollectionSerializer<BAMXPathEventSource>
    {
        public BAMXPathEventSourceSerializer()
            : base()
        {
        }

        public BAMXPathEventSourceSerializer(List<BAMXPathEventSource> properties)
            : base(properties)
        {
        }

        public BAMXPathEventSourceSerializer(BAMXPathEventSource[] properties)
            : base(properties)
        {
        }
    }

    /// <summary>
    /// Represents a BAM event source.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BAMXPathEventSource
    {
        #region Properties

        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The item name as defined in the BAM activity.")]
        [NotifyParentProperty(true)]
        public string ActivityItem { get; set; }

        /// <summary>
        /// The context property key name.
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
            return string.Format("Name = {0}, XPath = {1}", this.ActivityItem, this.XPath);
        }

        #endregion Object Overrides
    }
}