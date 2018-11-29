using DE.DAXFSA.Framework.BizTalk.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DE.DAXFSA.Framework.BizTalk.BAM
{
    /// <summary>
    /// Used to serialize the XPathProperty collection as XML.
    /// </summary>
    [XmlRoot(ElementName = "BAMPropertyEventSource", Namespace = "http://holtrenfrew/ecommerce/eai/bam/2015")]
    public class BAMPropertyEventSourceSerializer : XmlPropertyCollectionSerializer<BAMPropertyEventSource>
    {
        public BAMPropertyEventSourceSerializer()
            : base()
        {
        }

        public BAMPropertyEventSourceSerializer(List<BAMPropertyEventSource> properties)
            : base(properties)
        {
        }
    }

    /// <summary>
    /// Defines the property source of the BAM event source.
    /// </summary>
    public enum BAMPropertyEventSourceSource
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
        /// The source is a XPath expression.
        /// </summary>
        XPath
    }

    /// <summary>
    /// Represents a context property.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BAMPropertyEventSource
    {
        #region Properties

        ///// <summary>
        ///// Indicates if the value should be promoted to the context or just written.
        ///// </summary>
        //[Category("Behavior")]
        //[DefaultValue(false)]
        //[Description("Indicates if the value should be promoted to the context or just written.")]
        //[NotifyParentProperty(true)]
        //public bool Promote { get; set; }

        ///// <summary>
        ///// The context property key name.
        ///// </summary>
        //[Category("Behavior")]
        //[DefaultValue("")]
        //[Description("The context property name.")]
        //[NotifyParentProperty(true)]
        //public string Name { get; set; }

        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The item name as defined in the BAM activity.")]
        [NotifyParentProperty(true)]
        public string ActivityItem { get; set; }

        /// <summary>
        /// The context property value.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(BAMPropertyEventSourceSource.Literal)]
        [Description("The context property source.")]
        [NotifyParentProperty(true)]
        public BAMPropertyEventSourceSource Source { get; set; }

        ///// <summary>
        ///// The context property namespace.
        ///// </summary>
        //[Category("Behavior")]
        //[DefaultValue("")]
        //[Description("The context property namespace.")]
        //[NotifyParentProperty(true)]
        //public string Namespace { get; set; }

        /// <summary>
        /// The context property value.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The context property value.")]
        [NotifyParentProperty(true)]
        public string Value { get; set; }

        /// <summary>
        /// The context property value.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        [DisplayName("Ignore Null/Empty")]
        [Description("Indicates if promoting/writing the property to the context should be ignored if the value is null or empty.")]
        [NotifyParentProperty(true)]
        public bool IgnoreNullOrEmptyValue { get; set; }

        #endregion Properties

        #region Object Overrides

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return string.Format("ActivityItem = {0}, Value = {1}({2})", this.ActivityItem, this.Source, this.Value);
        }

        #endregion Object Overrides
    }
}