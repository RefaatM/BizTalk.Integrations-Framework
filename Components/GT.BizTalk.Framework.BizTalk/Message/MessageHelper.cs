using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using Microsoft.Practices.ESB.GlobalPropertyContext;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using DECore = GT.BizTalk.Framework.Core.Tracing;

namespace GT.BizTalk.Framework.BizTalk.Message
{

    /// <summary>
    /// Helper class for cloning messages, creating new messages, copying message parts,
    /// accessing context properties and more.
    /// </summary>
    public static class MessageHelper
    {
        private const string itineraryNamespace = "http://schemas.microsoft.biztalk.practices.esb.com/itinerary/system-properties";

        private static List<Tuple<string, string, bool>> minMessageContext = new List<Tuple<string, string, bool>>
        {
            {new Tuple<string, string, bool>(new BTS.CorrelationToken().Name.Name, new BTS.CorrelationToken().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new XMLNORM.DocumentSpecName().Name.Name, new XMLNORM.DocumentSpecName().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.EpmRRCorrelationToken().Name.Name, new BTS.EpmRRCorrelationToken().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new BTS.InboundTransportLocation().Name.Name, new BTS.InboundTransportLocation().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new BTS.InboundTransportType().Name.Name, new BTS.InboundTransportType().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new BTS.InterchangeID().Name.Name, new BTS.InterchangeID().Name.Namespace, false)},
            //{new Tuple<string, string, bool>(new BTS.InterchangeSequenceNumber().Name.Name, new BTS.InterchangeSequenceNumber().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.LastInterchangeMessage().Name.Name, new BTS.LastInterchangeMessage().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new BTS.IsSolicitResponse().Name.Name, new BTS.IsSolicitResponse().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new BTS.IsRequestResponse().Name.Name, new BTS.IsRequestResponse().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new BTS.MessageDestination().Name.Name, new BTS.MessageDestination().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.MessageType().Name.Name, new BTS.MessageType().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new BTS.OutboundTransportCLSID().Name.Name, new BTS.OutboundTransportCLSID().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.OutboundTransportLocation().Name.Name, new BTS.OutboundTransportLocation().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.OutboundTransportType().Name.Name, new BTS.OutboundTransportType().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.ReceivePipelineConfig().Name.Name, new BTS.ReceivePipelineConfig().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.ReceivePipelineID().Name.Name, new BTS.ReceivePipelineID().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.ReceivePipelineResponseConfig().Name.Name, new BTS.ReceivePipelineResponseConfig().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.ReceivePortID().Name.Name, new BTS.ReceivePortID().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new BTS.ReceivePortName().Name.Name, new BTS.ReceivePortName().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new BTS.ReqRespTransmitPipelineID().Name.Name, new BTS.ReqRespTransmitPipelineID().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.RouteDirectToTP().Name.Name, new BTS.RouteDirectToTP().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new BTS.SchemaStrongName().Name.Name, new BTS.SchemaStrongName().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.SendPipelineConfig().Name.Name, new BTS.SendPipelineConfig().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.SendPipelineResponseConfig().Name.Name, new BTS.SendPipelineResponseConfig().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.SOAPAction().Name.Name, new BTS.SOAPAction().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new BTS.SPGroupID().Name.Name, new BTS.SPGroupID().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.SPName().Name.Name, new BTS.SPName().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new BTS.SPTransportID().Name.Name, new BTS.SPTransportID().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.SPTransportBackupID().Name.Name, new BTS.SPTransportBackupID().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.SSOTicket().Name.Name, new BTS.SSOTicket().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.SuspendMessageOnMappingFailure().Name.Name, new BTS.SuspendMessageOnMappingFailure().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.SuspendMessageOnRoutingFailure().Name.Name, new BTS.SuspendMessageOnRoutingFailure().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new BTS.WindowsUser().Name.Name, new BTS.WindowsUser().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new BTS.WorkID().Name.Name, new BTS.WorkID().Name.Namespace, false)},
            {new Tuple<string, string, bool>(new WCF.Action().Name.Name, new WCF.Action().Name.Namespace, true)},
            {new Tuple<string, string, bool>(new FILE.ReceivedFileName().Name.Name, new FILE.ReceivedFileName().Name.Namespace, true)},
        };

       

        public static IBaseMessage ShallowCloneMessage(IPipelineContext pipelineContext, IBaseMessage message)
        {
            if (pipelineContext == null)
                throw new ArgumentNullException("pipelineContext");
            if (message == null)
                throw new ArgumentNullException("message");
            try
            {
                IBaseMessage message1 = pipelineContext.GetMessageFactory().CreateMessage();

                message1.Context = GetMininalContext(pipelineContext, message);

                MessageHelper.CloneAndAddMessageParts(pipelineContext, message, message1);
                pipelineContext.ResourceTracker.AddResource((object)message1.BodyPart.Data);
                return message1;
            }
            catch (Exception ex)
            {
                DECore.TraceProvider.Logger.TraceError(ex);
                throw;
            }
        }

        public static IBaseMessageContext GetMininalContext(IPipelineContext pipelineContext, IBaseMessage message)
        {
            IBaseMessageContext outputContext = pipelineContext.GetMessageFactory().CreateMessageContext();
            foreach (var context in minMessageContext)
            {
                var value = message.Context.Read(context.Item1, context.Item2);
                if (value != null)
                {
                    var isPromoted = message.Context.IsPromoted(context.Item1, context.Item2);
                    if (isPromoted || context.Item3)
                    {
                        outputContext.Promote(context.Item1, context.Item2, value);
                    }
                    else
                    {
                        outputContext.Write(context.Item1, context.Item2, value);
                    }
                }
            }
            return outputContext;
        }

        public static IBaseMessageContext CloneAndExcludeESBProperties(IPipelineContext pipelineContext, IBaseMessage message)
        {
            IBaseMessageContext outputContext = pipelineContext.GetMessageFactory().CreateMessageContext();
            for (int i = 0; i < message.Context.CountProperties; i++)
            {
                var name = string.Empty;
                var nameSpace = string.Empty;
                var value = message.Context.ReadAt(i, out name, out nameSpace);
                if (!nameSpace.Equals(itineraryNamespace, StringComparison.InvariantCultureIgnoreCase) && value != null)
                {
                    var isPromoted = message.Context.IsPromoted(name, nameSpace);
                    if (isPromoted)
                    {
                        outputContext.Promote(name, nameSpace, value);
                    }
                    else
                    {
                        outputContext.Write(name, nameSpace, value);
                    }
                }
            }
            return outputContext;
        }

        public static IBaseMessage DeepCloneMessage(IPipelineContext pipelineContext, IBaseMessage message)
        {
            if (pipelineContext == null)
                throw new ArgumentNullException("pipelineContext");
            if (message == null)
                throw new ArgumentNullException("message");
            IBaseMessage message1 = pipelineContext.GetMessageFactory().CreateMessage();
            message1.Context = PipelineUtil.CloneMessageContext(message.Context);
            MessageHelper.CloneAndAddMessageParts(pipelineContext, message, message1);
            pipelineContext.ResourceTracker.AddResource((object)message1.BodyPart.Data);
            return message1;
        }

        public static IBaseMessage DeepCloneMessage(IPipelineContext pipelineContext, IBaseMessage message, bool cloneItinerary)
        {
            if (pipelineContext == null)
                throw new ArgumentNullException("pipelineContext");
            if (message == null)
                throw new ArgumentNullException("message");
            IBaseMessage message1 = pipelineContext.GetMessageFactory().CreateMessage();
            if (cloneItinerary)
            {
                message1.Context = PipelineUtil.CloneMessageContext(message.Context);
            }
            else
            {
                message1.Context = CloneAndExcludeESBProperties(pipelineContext, message);
            }
            MessageHelper.CloneAndAddMessageParts(pipelineContext, message, message1);
            pipelineContext.ResourceTracker.AddResource((object)message1.BodyPart.Data);
            return message1;
        }

       
        /// <summary>
        /// Copies all the message parts from the source message into the destination message.
        /// </summary>
        /// <param name="sourceMessage">Source message.</param>
        /// <param name="destinationMessage">Destination message.</param>
        public static void CopyMessageParts(IBaseMessage sourceMessage, IBaseMessage destinationMessage)
        {
            if (sourceMessage == null)
                throw new ArgumentNullException("sourceMessage");
            if (destinationMessage == null)
                throw new ArgumentNullException("destinationMessage");

            string bodyPartName = sourceMessage.BodyPartName;
            for (int index = 0; index < sourceMessage.PartCount; ++index)
            {
                string partName = null;
                IBaseMessagePart partByIndex = sourceMessage.GetPartByIndex(index, out partName);
                bool isBodyPart = string.Compare(partName, bodyPartName, true, CultureInfo.CurrentCulture) == 0;
                destinationMessage.AddPart(partName, partByIndex, isBodyPart);
            }
        }

        public static void CloneAndAddMessageParts(IPipelineContext pipelineContext, IBaseMessage sourceMessage, IBaseMessage destinationMessage)
        {
            if (pipelineContext == null)
                throw new ArgumentNullException("pipelineContext");
            if (sourceMessage == null)
                throw new ArgumentNullException("sourceMessage");
            if (destinationMessage == null)
                throw new ArgumentNullException("destinationMessage");
            try
            {
                string bodyPartName = sourceMessage.BodyPartName;
                for (int index = 0; index < sourceMessage.PartCount; ++index)
                {
                    string partName = (string)null;
                    IBaseMessagePart partByIndex = sourceMessage.GetPartByIndex(index, out partName);
                    IBaseMessagePart part = MessageHelper.CloneMessagePart(pipelineContext, partByIndex);
                    bool bBody = string.Compare(partName, bodyPartName, true, CultureInfo.CurrentCulture) == 0;
                    destinationMessage.AddPart(partName, part, bBody);
                }
            }
            catch (Exception ex)
            {
                DECore.TraceProvider.Logger.TraceError(ex);
                throw;
            }
        }

        private static IBaseMessagePart CloneMessagePart(IPipelineContext pipelineContext, IBaseMessagePart messagePart)
        {
            IBaseMessageFactory messageFactory = pipelineContext.GetMessageFactory();
            IBaseMessagePart messagePart1 = messageFactory.CreateMessagePart();
            messagePart1.Charset = messagePart.Charset;
            messagePart1.ContentType = messagePart.ContentType;
            messagePart1.PartProperties = PipelineUtil.CopyPropertyBag(messagePart.PartProperties, messageFactory);
            VirtualStream virtualStream = new VirtualStream();
            MessageHelper.CopyStream(messagePart.Data, (Stream)virtualStream);
            messagePart1.Data = (Stream)virtualStream;
            return messagePart1;
        }

     
        public static string GetContextValue(IBaseMessageContext context, string property, string propertyNamespace)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (string.IsNullOrEmpty(property))
                throw new ArgumentNullException("property");
            if (string.IsNullOrEmpty(propertyNamespace))
                throw new ArgumentNullException("propertyNamespace");
            string str = string.Empty;
            try
            {
                object obj = context.Read(property, propertyNamespace);
                if (obj != null)
                    str = obj.ToString();
                return str;
            }
            catch (Exception ex)
            {
                DECore.TraceProvider.Logger.TraceError(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// Sets the message schema strong name and message type.
        /// </summary>
        /// <param name="context">Pipeline context.</param>
        /// <param name="messageType">Message type.</param>
        /// <param name="message">Message instance.</param>
        /// <param name="docSpec">Document specification.</param>
      

        public static void SetDocProperties(IPipelineContext pContext, string messageType, IBaseMessage pInMsg, ref string docSpec)
        {
            try
            {
                pInMsg.Context.Promote(BtsProperties.MessageType.Name, BtsProperties.MessageType.Namespace, (object)messageType);
                IDocumentSpec documentSpecByType = pContext.GetDocumentSpecByType(messageType);
                if (documentSpecByType == null)
                    return;
                pInMsg.Context.Write(DasmProperties.DocumentSpecName.Name, DasmProperties.DocumentSpecName.Namespace, (object)null);
                pInMsg.Context.Write(BtsProperties.SchemaStrongName.Name, BtsProperties.SchemaStrongName.Namespace, (object)documentSpecByType.DocSpecStrongName);
                docSpec = docSpec == null ? (docSpec = documentSpecByType.DocSpecName) : docSpec;
            }
            catch (DocumentSpecException ex)
            {
                // Todo log this error
                DECore.TraceProvider.Logger.TraceInfo(ex.ToString());
            }
            catch (COMException ex)
            {
                // Todo log this errors
                DECore.TraceProvider.Logger.TraceInfo(ex.ToString());
            }
            catch (Exception ex)
            {
                DECore.TraceProvider.Logger.TraceError(ex);
                throw;
            }
        }
    

        public static string GetMessageType(IBaseMessage msg, IPipelineContext context)
        {
            MarkableForwardOnlyEventingReadStream stm = new MarkableForwardOnlyEventingReadStream(msg.BodyPart.GetOriginalDataStream());
            context.ResourceTracker.AddResource((object)stm);
            return Utils.GetDocType(stm);
        }

        public static void SetItineraryDescriptionProperties(IPipelineContext context, IBaseMessage msg)
        {
            string s = msg.Context.Read(ItineraryDescriptionProperties.Name, ItineraryDescriptionProperties.Namespace) as string;
            if (string.IsNullOrEmpty(s))
                return;
            XmlReader xmlReader = (XmlReader)new XmlTextReader((TextReader)new StringReader(s));
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Name")
                    msg.Context.Write(ItineraryDescriptionProperties.ItineraryDescriptionName.Name, ItineraryDescriptionProperties.ItineraryDescriptionName.Namespace, (object)xmlReader.ReadElementContentAsString());
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Version")
                    msg.Context.Write(ItineraryDescriptionProperties.ItineraryDescriptionVersion.Name, ItineraryDescriptionProperties.ItineraryDescriptionVersion.Namespace, (object)xmlReader.ReadElementContentAsString());
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Guid")
                    msg.Context.Write(ItineraryDescriptionProperties.ItineraryDescriptionGuid.Name, ItineraryDescriptionProperties.ItineraryDescriptionGuid.Namespace, (object)xmlReader.ReadElementContentAsString());
            }
        }

    

        public static void CopyStream(Stream input, Stream output)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (output == null)
                throw new ArgumentNullException("output");
            byte[] buffer = new byte[32768];
            int count;
            while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
                output.Write(buffer, 0, count);
            if (!output.CanSeek)
                return;
            output.Seek(0L, SeekOrigin.Begin);
            input.Seek(0L, SeekOrigin.Begin);
        }
   
        #region Constants

        public const string XML_ANY_TYPE_INFO = "Microsoft.XLANGs.BaseTypes.Any, Microsoft.XLANGs.BaseTypes, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
        private const int BUFFER_SIZE = 8192;

        #endregion Constants

        #region Methods

        public static void CopyMessageParts(IPipelineContext pc, IBaseMessage inmsg, IBaseMessage outmsg, IBaseMessagePart bodyPart)
        {
            CopyMessageParts(pc, inmsg, outmsg, bodyPart, false);
        }

        public static void CopyMessageParts(IPipelineContext pc, IBaseMessage inmsg, IBaseMessage outmsg, IBaseMessagePart bodyPart, bool allowUnrecognizeMessage)
        {
            string bodyPartName = inmsg.BodyPartName;
            for (int i = 0; i < inmsg.PartCount; i++)
            {
                string partName = null;
                IBaseMessagePart partByIndex = inmsg.GetPartByIndex(i, out partName);
                if ((partByIndex == null) && !allowUnrecognizeMessage)
                {
                    throw new ArgumentNullException("otherOutPart[" + i + "]");
                }
                if (bodyPartName != partName)
                {
                    outmsg.AddPart(partName, partByIndex, false);
                }
                else
                {
                    outmsg.AddPart(bodyPartName, bodyPart, true);
                }
            }
        }

        /// <summary>
        /// Creates a clone copy of the specified message.
        /// </summary>
        /// <param name="pipelineContext">Pipeline context.</param>
        /// <param name="message">Source message.</param>
        /// <returns>Clone message.</returns>
       


        public static IBaseMessage CreateCloneMessage(IPipelineContext pipelineContext, IBaseMessage message)
        {
            if (pipelineContext == null)
                throw new ArgumentNullException("pipelineContext");
            if (message == null)
                throw new ArgumentNullException("message");
            try
            {
                IBaseMessage message1 = pipelineContext.GetMessageFactory().CreateMessage();
                message1.Context = PipelineUtil.CloneMessageContext(message.Context);
                MessageHelper.CopyMessageParts(message, message1);
                pipelineContext.ResourceTracker.AddResource((object)message1.BodyPart.Data);
                return message1;
            }
            catch (Exception ex)
            {
                DECore.TraceProvider.Logger.TraceError(ex);
                throw;
            }
        }
        /// <summary>
        /// Creates a new message by cloning the source message, but using the specified output stream instead.
        /// </summary>
        /// <param name="pipelineContext">Pipeline context.</param>
        /// <param name="message">Source message.</param>
        /// <param name="streamOut">Output stream that will be associated to the new message.</param>
        /// <returns>New message.</returns>
       

        public static IBaseMessage CreateNewMessage(IPipelineContext pipelineContext, IBaseMessage message, Stream streamOut)
        {
            if (pipelineContext == null)
                throw new ArgumentNullException("pipelineContext");
            if (message == null)
                throw new ArgumentNullException("message");
            if (streamOut == null)
                throw new ArgumentNullException("streamOut");
            try
            {
                IBaseMessage baseMessage = (IBaseMessage)((System.ICloneable)message).Clone();
                streamOut.Position = 0L;
                baseMessage.BodyPart.Data = streamOut;
                pipelineContext.ResourceTracker.AddResource((object)baseMessage.BodyPart.Data);
                return baseMessage;
            }
            catch (Exception ex)
            {
                DECore.TraceProvider.Logger.TraceError(ex);
                throw;
            }
        }

        /// <summary>
        /// Sets the message schema strong name and message type.
        /// </summary>
        /// <param name="context">Pipeline context.</param>
        /// <param name="message">Message instance.</param>
       
        public static void SetDocProperties(IPipelineContext context, IBaseMessage msg)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (msg == null)
                throw new ArgumentNullException("msg");
            object obj = msg.Context.Read(BtsProperties.SchemaStrongName.Name, BtsProperties.SchemaStrongName.Namespace);
            if (obj != null && string.Compare(obj.ToString(), "Microsoft.XLANGs.BaseTypes.Any, Microsoft.XLANGs.BaseTypes, Version=3.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", true, CultureInfo.CurrentCulture) == 0)
                msg.Context.Write(BtsProperties.SchemaStrongName.Name, BtsProperties.SchemaStrongName.Namespace, (object)null);
            string DocSpecName = (string)msg.Context.Read(BtsProperties.SchemaStrongName.Name, BtsProperties.SchemaStrongName.Namespace) ?? string.Empty;
            DECore.TraceProvider.Logger.TraceInfo(string.Format("Schema strong name is {0} after first context read.", new object[1] { obj }));
            string str = (string)msg.Context.Read(BtsProperties.MessageType.Name, BtsProperties.MessageType.Namespace) ?? string.Empty;
            DECore.TraceProvider.Logger.TraceInfo(string.Format("Document message type is {0} after first context read.", new object[1] { (object)str }));
            if (!string.IsNullOrEmpty(str))
                return;
            if (!string.IsNullOrEmpty(DocSpecName))
            {
                IDocumentSpec documentSpec = (IDocumentSpec)null;
                try
                {
                    documentSpec = context.GetDocumentSpecByName(DocSpecName);
                }
                catch (DocumentSpecException ex)
                {
                    // Todo Log this Error
                    DECore.TraceProvider.Logger.TraceInfo(ex.ToString());
                }
                catch (COMException ex)
                {
                    // todo Log this erro
                    DECore.TraceProvider.Logger.TraceInfo(ex.ToString());
                }
                if (documentSpec != null)
                    str = documentSpec.DocType;
            }
            msg.Context.Write(BtsProperties.MessageType.Name, BtsProperties.MessageType.Namespace, (object)str);
            DECore.TraceProvider.Logger.TraceInfo(string.Format("Message type is {0}.", new object[1] { (object)str }));
        }
      
        /// <summary>
        /// Returns the message type.
        /// </summary>
        /// <param name="context">Pipeline context.</param>
        /// <param name="message">Message instance.</param>
        /// <returns>The message type.</returns>
        public static string GetMessageType(IPipelineContext context, IBaseMessage message)
        {
            MarkableForwardOnlyEventingReadStream stm = new MarkableForwardOnlyEventingReadStream(message.BodyPart.GetOriginalDataStream());
            context.ResourceTracker.AddResource((object)stm);
            return Utils.GetDocType(stm);
        }

        /// <summary>
        /// Returns the message size.
        /// </summary>
        /// <param name="message">Message instace.</param>
        /// <returns>The message size.</returns>
        public static ulong GetMessageSize(IBaseMessage message)
        {
            ulong msgSize;
            bool isImplemented;
            message.GetSize(out msgSize, out isImplemented);
            return msgSize;
        }

        /// <summary>
        /// Removes the byte order mask.
        /// </summary>
        /// <param name="inStream">Input stream.</param>
        /// <returns>New stream without the byte order mask.</returns>
        public static Stream RemoveByteOrderMark(Stream inStream)
        {
            if (inStream == null)
                throw new ArgumentNullException("inStream");

            int num = MatchByteOrderMarkSequence(inStream);
            if (num <= 0)
                return inStream;
            VirtualStream virtualStream = new VirtualStream((int)inStream.Length);
            byte[] buffer = new byte[BUFFER_SIZE];
            inStream.Position = (long)num;
            int count;
            while ((count = inStream.Read(buffer, 0, BUFFER_SIZE)) > 0)
                virtualStream.Write(buffer, 0, count);
            virtualStream.Flush();
            virtualStream.Position = 0L;
            return virtualStream;
        }

        private static int MatchByteOrderMarkSequence(Stream inStream)
        {
            if (inStream == null)
                throw new ArgumentNullException("inStream");
            byte[][] numArray = new byte[3][]
            {
                new byte[3]
                {
                    (byte) 239,
                    (byte) 187,
                    (byte) 191
                },
                new byte[2]
                {
                    byte.MaxValue,
                    (byte) 254
                },
                new byte[2]
                {
                    (byte) 254,
                    byte.MaxValue
                }
            };
            int num = 0;
            byte[] buffer = new byte[3];
            inStream.Read(buffer, 0, 3);
            for (int index1 = 0; index1 < numArray.Length; ++index1)
            {
                int index2 = 0;
                while (index2 < numArray[index1].Length && (int)buffer[index2] == (int)numArray[index1][index2])
                    ++index2;
                if (index2 == numArray[index1].Length)
                {
                    num = numArray[index1].Length;
                    break;
                }
            }
            return num;
        }

        #endregion Methods
    }
}