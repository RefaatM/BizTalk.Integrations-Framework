using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GT.BizTalk.Framework.BizTalk.Serialization
{
    /// <summary>
    /// Used to serialize the XPathProperty collection as XML.
    /// </summary>
    [XmlRoot(ElementName = "ItineraryProperty", Namespace = "http://GT.BizTalk.Framework.BizTalk.Serialization/2015")]
    public class ItineraryPropertySerializer : XmlPropertyCollectionSerializer<ItineraryProperty>
    {
        public ItineraryPropertySerializer()
            : base()
        {
        }

        public ItineraryPropertySerializer(List<ItineraryProperty> properties)
            : base(properties)
        {
        }
    }


    /*
     *   public ItineraryProperty()
        {
        }

        public ItineraryProperty(string propertyName, string propertyValue)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            if (string.IsNullOrEmpty(propertyValue))
            {
                throw new ArgumentNullException("propertyValue");
            }

            Name = propertyName;
            Value = propertyValue;
        }

        public ItineraryProperty(string property)
        {
            if (string.IsNullOrEmpty(property))
            {
                throw new ArgumentNullException("property");
            }

            if (!property.Contains("#"))
            {
                throw new ArgumentException("The property path {0} is not valid", property);
            }

            Value = property.Split('#')[0];
            Name = property.Split('#')[1];
        }
     * */

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ItineraryProperty
    {
      

        #region Properties
        /// <summary>
        /// The context property key name.
        /// </summary>
        [Category("Itinerary Property")]
        [DefaultValue("")]
        [Description("The Itinerary property name.")]
        [NotifyParentProperty(true)]
        public string Name { get; set; }

        /// <summary>
        /// The context property value.
        /// </summary>
        [Category("Itinerary Property")]
        [DefaultValue("")]
        [Description("The Itinerary property value.")]
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
