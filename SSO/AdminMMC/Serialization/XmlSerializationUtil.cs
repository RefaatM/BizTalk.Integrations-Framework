using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GT.BizTalk.SSO.AdminMMC.Serialization
{
    /// <summary>
    /// Utility methods for serializing/deserializing objects in XML format.
    /// </summary>
    public static class XmlSerializationUtil
    {
        /// <summary>
        /// Deserializes an XML file into an object using the XmlSerializer.
        /// </summary>
        /// <typeparam name="T">The type of the serialized object.</typeparam>
        /// <param name="filePath">The path to the file to be deserialized.</param>
        /// <returns>The object deserialized.</returns>
        public static T LoadXml<T>(string filePath)
        {
            using (XmlTextReader xmlTextReader = new XmlTextReader(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(xmlTextReader);
            }
        }

        /// <summary>
        /// Serializes the specified object instance in XML format and saves
        /// it into the specified file.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="data">The object to serialize.</param>
        /// <param name="filePath">The path to the file where the XML will be saved.</param>
        public static void SaveXml<T>(T data, string filePath)
        {
            using (XmlTextWriter xmlWriter = new XmlTextWriter(filePath, Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.Indentation = 2;
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(xmlWriter, data);
            }
        }
    }
}
