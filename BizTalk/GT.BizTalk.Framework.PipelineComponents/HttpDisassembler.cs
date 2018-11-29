using GT.BizTalk.Framework.BizTalk.Serialization;
using GT.BizTalk.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.XLANGs.RuntimeTypes;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace GT.BizTalk.Framework.PipelineComponents
{

    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.Guid("FE75A97A-EB7C-49AF-8778-136FA366A5F4")]
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    public class HttpDisassembler : BasePipelineComponent, IDisassemblerComponent
    {
        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public HttpDisassembler()
            : base(Resources.ResourceManager,
                Resources.HttpDisassemblerName,
                Resources.HttpDisassemblerDescription,
                Resources.HttpDisassemblerVersion,
                Resources.HttpDisassemblerIcon)
        {
        }

        #endregion Constructor

        private const string DocumentSpecNamePropertyName = "DocumentSpecName";
        private readonly Queue _outputQueue = new Queue();

        #region Design-time properties

        /// <summary>
        /// Gets/sets the message type. If not specified the BRE policy will be used to get it.
        /// </summary>
        [BtsPropertyName("DocumentSpecNamePropertyName")]
        [BtsDescription("DocumentSpecName Description")]
        public string DocumentSpecName
        {
            get;
            set;
        }

        #endregion Design-time properties
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyBag"></param>
        /// <param name="errorLog"></param>
        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            if (string.IsNullOrEmpty(DocumentSpecName))
            {
                DocumentSpecName = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag(propertyBag, DocumentSpecNamePropertyName), string.Empty);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyBag"></param>
        /// <param name="clearDirty"></param>
        /// <param name="saveAllProperties"></param>
        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(propertyBag, DocumentSpecNamePropertyName, DocumentSpecName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pInMsg"></param>

        public void Disassemble(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            //Get a reference to the BizTalk schema.
            var documentSpec = (DocumentSpec)pContext.GetDocumentSpecByName(DocumentSpecName);

            //Get a list of properties defined in the schema.
            var annotations = documentSpec.GetPropertyAnnotationEnumerator();
            var doc = new XmlDocument();
            var sw = new StringWriter(new StringBuilder());

            //Create a new instance of the schema.
            doc.Load(documentSpec.CreateXmlInstance(sw));
            sw.Dispose();

            //Write all properties to the message body.
            while (annotations.MoveNext())
            {
                var annotation = (IPropertyAnnotation)annotations.Current;
                var node = doc.SelectSingleNode(annotation.XPath);
                object propertyValue;

                if (pInMsg.Context.TryRead(new ContextProperty(annotation.Name, annotation.Namespace), out propertyValue))
                {
                    node.InnerText = propertyValue.ToString();
                }
            }

            var ms = new MemoryStream();
            doc.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);

            var outMsg = pInMsg;
            outMsg.BodyPart.Data = ms;

            //Promote message type and SchemaStrongName
            outMsg.Context.Promote(new ContextProperty(SystemProperties.MessageType), documentSpec.DocType);
            outMsg.Context.Promote(new ContextProperty(SystemProperties.SchemaStrongName), documentSpec.DocSpecStrongName);

            _outputQueue.Enqueue(outMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pContext"></param>
        /// <returns></returns>
        public IBaseMessage GetNext(IPipelineContext pContext)
        {
            if (_outputQueue.Count > 0)
            {
                return (IBaseMessage)_outputQueue.Dequeue();
            }

            return null;
        }
    }
}