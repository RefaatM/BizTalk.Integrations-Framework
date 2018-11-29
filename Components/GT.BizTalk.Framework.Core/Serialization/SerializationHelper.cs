using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GT.BizTalk.Framework.Core
{
    /// <summary>
    /// Serialization helper methods.
    /// </summary>
    public class SerializationHelper
    {
        #region Binary

        /// <summary>
        /// Serialize an object using the BinaryFormatter.
        /// </summary>
        /// <param name="item">The object that must be serialized</param>
        /// <returns>A Base64 string.</returns>
        public static string BinarySerialize(object item)
        {
            if (item != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, item);
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            return null;
        }

        /// <summary>
        /// Deserialize an Base64 string into an object using the BinaryFormatter.
        /// </summary>
        /// <typeparam name="T">The type of the serialized object.</typeparam>
        /// <param name="item">The string that must be deserialized.</param>
        /// <returns>The object deserialized.</returns>
        public static T BinaryDeserialize<T>(string item)
        {
            if (item != null && item.Length > 0)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(item);
                if (buffer != null && buffer.Length > 0)
                {
                    using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(item)))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        return (T)binaryFormatter.Deserialize(memoryStream);
                    }
                }
            }
            return default(T);
        }

        #endregion Binary

        #region Xml

        /// <summary>
        /// Serialize an object using the XmlSerializer.
        /// </summary>
        /// <param name="item">The object that must be serialized</param>
        /// <returns>A XML string.</returns>
        public static string XmlSerialize(object item)
        {
            if (item != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (StreamWriter stringWriter = new StreamWriter(memoryStream, System.Text.UTF8Encoding.ASCII))
                    {
                        XmlSerializer serializer = new XmlSerializer(item.GetType());
                        serializer.Serialize(stringWriter, item);
                        return Encoding.UTF8.GetString(memoryStream.ToArray());
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Deserializes an XML string into an object using the XmlSerializer.
        /// </summary>
        /// <typeparam name="T">The type of the serialized object.</typeparam>
        /// <param name="item">The string that must be deserialized.</param>
        /// <returns>The object deserialized.</returns>
        public static T XmlDeserialize<T>(string item)
        {
            if (item != null && item.Length > 0)
            {
                using (StringReader stringReader = new StringReader(item))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stringReader);
                }
            }
            return default(T);
        }

        /// <summary>
        /// Deserializes an XML file into an object using the XmlSerializer.
        /// </summary>
        /// <typeparam name="T">The type of the serialized object.</typeparam>
        /// <param name="pathName">The path to the file to be deserialized.</param>
        /// <returns>The object deserialized.</returns>
        public static T LoadXml<T>(string pathName)
        {
            using (XmlTextReader xmlTextReader = new XmlTextReader(pathName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(xmlTextReader);
            }
        }

        #endregion Xml

        #region DataContract

        /// <summary>
        /// Serialize an object using the DataContractSerializer.
        /// </summary>
        /// <param name="item">The object that must be serialized</param>
        /// <returns>A XML string.</returns>
        public static string DataContractSerialize(object item)
        {
            if (item != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    DataContractSerializer serializer = new DataContractSerializer(item.GetType());
                    serializer.WriteObject(memoryStream, item);
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
            return null;
        }

        /// <summary>
        /// Deserialize an XML string into an object using the DataContractSerializer.
        /// </summary>
        /// <typeparam name="T">The type of the serialized object.</typeparam>
        /// <param name="item">The string that must be deserialized.</param>
        /// <returns>The object deserialized.</returns>
        public static T DataContractDeserialize<T>(string item)
        {
            if (item != null && item.Length > 0)
            {
                XmlDictionaryReader xmlDictionaryReader = null;
                try
                {
                    xmlDictionaryReader = XmlDictionaryReader.CreateTextReader(Encoding.UTF8.GetBytes(item), XmlDictionaryReaderQuotas.Max);
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    return (T)serializer.ReadObject(xmlDictionaryReader, false);
                }
                finally
                {
                    if (xmlDictionaryReader != null)
                    {
                        xmlDictionaryReader.Close();
                    }
                }
            }
            return default(T);
        }

        #endregion DataContract
    }
}