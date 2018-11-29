using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DE.DAXFSA.Framework.PipelineComponents
{
    /// <summary>
    /// Used to serialize the XPathProperty collection as XML.
    /// </summary>
    [XmlRoot(ElementName = "BAMEventSource", Namespace = "https://schemas.DirectEnergy.com/Unify/Common/pipelinecomponents/2015")]
    public class BAMEventSourceSerializer : XmlPropertyCollectionSerializer<BAMEventSource>
    {
        public BAMEventSourceSerializer()
            : base()
        {
        }

        public BAMEventSourceSerializer(List<BAMEventSource> properties)
            : base(properties)
        {
        }
    }

    /// <summary>
    /// Represents a BAM event source.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BAMEventSource
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
        [Description("The context property name.")]
        [NotifyParentProperty(true)]
        public string ContextPropertyName { get; set; }

        /// <summary>
        /// The context property namespace.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The context property namespace.")]
        [NotifyParentProperty(true)]
        public string ContextPropertyNamespace { get; set; }

        #endregion Properties

        #region Object Overrides

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return string.Format("Name = {0}, Namespace = {1}", this.ContextPropertyName, this.ContextPropertyNamespace);
        }

        #endregion Object Overrides
    }
}