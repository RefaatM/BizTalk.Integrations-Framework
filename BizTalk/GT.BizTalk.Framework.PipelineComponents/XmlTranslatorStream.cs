using Microsoft.BizTalk.Streaming;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace DE.DAXFSA.Framework.PipelineComponents
{
    [ComVisible(false)]
    public class XmlTranslatorStream : XmlBufferedReaderStream
    {
        // Fields
        protected XmlReader m_reader;

        private bool m_terse;
        protected XmlTextWriter m_writer;
        protected XmlValidationErrors m_validationErrors;
        protected string m_textValue;

        // Methods
        public XmlTranslatorStream(XmlReader reader)
            : this(reader, Encoding.UTF8)
        {
        }

        public XmlTranslatorStream(XmlReader reader, Encoding encoding)
            : this(reader, encoding, null)
        {
        }

        public XmlTranslatorStream(XmlReader reader, Encoding encoding, MemoryStream outputStream)
            : base(outputStream)
        {
            this.m_reader = reader;
            this.m_writer = new XmlTextWriter(base.m_outputStream, encoding);
        }

        public override void Close()
        {
            base.Close();
            if (this.m_reader != null)
            {
                this.m_reader.Close();
                this.m_reader = null;
            }
        }

        protected override int ProcessXmlNodes(int count)
        {
            while ((base.m_outputStream.Length < count) && this.m_reader.Read())
            {
                this.TranslateXmlNode();
                this.m_writer.Flush();
            }

            if (m_validationErrors.Count > 0 && (base.m_outputStream.Length < count))
            {
                XmlValidationException valException = new XmlValidationException();
                valException.ErrorCollection = m_validationErrors;
                valException.ErrorCode = "10002";
                throw valException;
            }

            return (int)base.m_outputStream.Length;
        }

        protected virtual void TranslateAttribute()
        {
            if (!this.m_reader.IsDefault)
            {
                string prefix = this.m_reader.Prefix;
                string localName = this.m_reader.LocalName;
                string namespaceURI = this.m_reader.NamespaceURI;
                this.TranslateStartAttribute(prefix, localName, namespaceURI);
                while (this.m_reader.ReadAttributeValue())
                {
                    if (this.m_reader.NodeType == XmlNodeType.EntityReference)
                    {
                        this.TranslateEntityRef(this.m_reader.Name);
                    }
                    else
                    {
                        this.TranslateAttributeValue(prefix, localName, namespaceURI, this.m_reader.Value);
                    }
                }
                this.m_writer.WriteEndAttribute();
            }
        }

        protected virtual void TranslateAttributes()
        {
            if (this.m_reader.MoveToFirstAttribute())
            {
                do
                {
                    this.TranslateAttribute();
                }
                while (this.m_reader.MoveToNextAttribute());
                this.m_reader.MoveToElement();
            }
        }

        protected virtual void TranslateAttributeValue(string prefix, string localName, string nsURI, string val)
        {
            this.m_writer.WriteString(val);
        }

        protected virtual void TranslateCData(string data)
        {
            this.m_writer.WriteCData(data);
        }

        protected virtual void TranslateComment(string comment)
        {
            this.m_writer.WriteComment(comment);
        }

        protected virtual void TranslateDocType(string name, string pubAttr, string systemAttr, string subset)
        {
            this.m_writer.WriteDocType(name, pubAttr, systemAttr, subset);
        }

        protected virtual void TranslateElement()
        {
            this.m_textValue = string.Empty;
            this.TranslateStartElement(this.m_reader.Prefix, this.m_reader.LocalName, this.m_reader.NamespaceURI);
            this.TranslateAttributes();
            if (this.m_reader.IsEmptyElement)
            {
                this.TranslateEndElement(false);
            }
        }

        protected virtual void TranslateEndElement(bool full)
        {
            if (full)
            {
                this.m_writer.WriteFullEndElement();
            }
            else
            {
                this.m_writer.WriteEndElement();
            }
        }

        protected virtual void TranslateEntityRef(string name)
        {
            this.m_writer.WriteEntityRef(name);
        }

        protected virtual void TranslateProcessingInstruction(string target, string val)
        {
            this.m_writer.WriteProcessingInstruction(target, val);
        }

        protected virtual void TranslateStartAttribute(string prefix, string localName, string nsURI)
        {
            if (prefix == null)
            {
                this.m_writer.WriteStartAttribute(localName, nsURI);
            }
            else
            {
                this.m_writer.WriteStartAttribute(prefix, localName, nsURI);
            }
        }

        protected virtual void TranslateStartElement(string prefix, string localName, string nsURI)
        {
            if (prefix == null)
            {
                this.m_writer.WriteStartElement(localName, nsURI);
            }
            else
            {
                this.m_writer.WriteStartElement(prefix, localName, nsURI);
            }
        }

        protected virtual void TranslateText(string s)
        {
            this.m_textValue = s;
            this.m_writer.WriteString(s);
        }

        protected virtual void TranslateWhitespace(string space)
        {
            //SK: 11/02/2008 m_textValue doesn't pass space if pattern fails
            this.m_textValue = space;
            this.m_writer.WriteWhitespace(space);
        }

        protected virtual void TranslateXmlDeclaration(string target, string val)
        {
        }

        protected virtual bool TranslateXmlNode()
        {
            switch (this.m_reader.NodeType)
            {
                case XmlNodeType.None:
                case XmlNodeType.Entity:
                case XmlNodeType.EndEntity:
                    break;

                case XmlNodeType.Element:
                    this.TranslateElement();
                    break;

                case XmlNodeType.Text:
                    this.TranslateText(this.m_reader.Value);
                    break;

                case XmlNodeType.CDATA:
                    this.TranslateCData(this.m_reader.Value);
                    break;

                case XmlNodeType.EntityReference:
                    this.TranslateEntityRef(this.m_reader.Name);
                    break;

                case XmlNodeType.ProcessingInstruction:
                    this.TranslateProcessingInstruction(this.m_reader.Name, this.m_reader.Value);
                    break;

                case XmlNodeType.Comment:
                    if (!this.m_terse)
                    {
                        this.TranslateComment(this.m_reader.Value);
                    }
                    break;

                case XmlNodeType.DocumentType:
                    this.TranslateDocType(this.m_reader.Name, this.m_reader.GetAttribute("PUBLIC"), this.m_reader.GetAttribute("SYSTEM"), this.m_reader.Value);
                    break;

                case XmlNodeType.Whitespace:
                    if (!this.m_terse)
                    {
                        this.TranslateWhitespace(this.m_reader.Value);
                    }
                    break;

                case XmlNodeType.SignificantWhitespace:
                    this.TranslateWhitespace(this.m_reader.Value);
                    break;

                case XmlNodeType.EndElement:
                    this.TranslateEndElement(true);
                    break;

                case XmlNodeType.XmlDeclaration:
                    this.TranslateXmlDeclaration(this.m_reader.Name, this.m_reader.Value);
                    break;

                default:
                    throw new XmlException("Unrecognized xml node");
            }
            return true;
        }

        // Properties
        public bool Terse
        {
            get
            {
                return this.m_terse;
            }
            set
            {
                this.m_terse = value;
            }
        }
    }
}