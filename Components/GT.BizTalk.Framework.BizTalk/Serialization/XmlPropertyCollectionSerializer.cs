using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GT.BizTalk.Framework.BizTalk.Serialization
{
    /// <summary>
    /// Provides xml serialization of a collection of properties.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XmlPropertyCollectionSerializer<T>
            where T : class, new()
    {
        public XmlPropertyCollectionSerializer()
        {
            this.Properties = new List<T>();
        }

        public XmlPropertyCollectionSerializer(List<T> properties)
        {
            this.Properties = properties;
        }

        public XmlPropertyCollectionSerializer(T[] properties)
        {
            this.Properties = new List<T>(properties);
        }

        /// <summary>
        /// Gets/sets the property collection.
        /// </summary>
        [XmlArrayItem("Property")]
        public List<T> Properties
        {
            get;
            set;
        }

        public void Deserialize(string xmlString)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreProcessingInstructions = true;
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString), settings))
            {
                XmlSerializer serializer = new XmlSerializer(this.GetType());
                XmlPropertyCollectionSerializer<T> instance = (XmlPropertyCollectionSerializer<T>)serializer.Deserialize(reader);
                this.Properties = instance.Properties;
            }
        }

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                XmlSerializer serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
            }
            return sb.ToString();
        }
    }
}