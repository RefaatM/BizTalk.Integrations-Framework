using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;

namespace DE.DAXFSA.Framework.PipelineComponents
{
    [ComVisible(false)]
    public class XmlValidatingStream : XmlTranslatorStream
    {
        // Methods
        public XmlValidatingStream(Stream data)
            : base(null)
        {
            base.m_validationErrors = new XmlValidationErrors();

            XmlTextReader reader = new XmlTextReader(data);
            base.m_reader = new XmlValidatingReader(reader);

            (base.m_reader as XmlValidatingReader).ValidationEventHandler += new ValidationEventHandler(ValidationCallback);
        }

        // Properties
        public XmlSchemaCollection Schemas
        {
            get
            {
                return (base.m_reader as XmlValidatingReader).Schemas;
            }
        }

        public void ValidationCallback(object sender, ValidationEventArgs args)
        {
            XmlReader reader = (XmlReader)sender;
            XmlValidationErrorType err = new XmlValidationErrorType();
            err.severity = args.Severity.ToString();
            err.nodeName = reader.LocalName;

            //Only if an element contains invalid value according datatype ErrorType is Pattern else it is Structural.
            if (args.Message.Contains(Constants.PatternXMLErrMsgStr))
            {
                err.nodeValue = base.m_textValue;
                err.errorType = XmlValidationErrorType.ErrorType.Pattern;
            }
            else
            {
                err.nodeValue = string.Empty;
                err.errorType = XmlValidationErrorType.ErrorType.Structural;
            }

            err.description = args.Message;
            err.nodeNameSpace = reader.NamespaceURI;
            base.m_textValue = string.Empty;
            base.m_validationErrors.Add(err);
        }
    }
}