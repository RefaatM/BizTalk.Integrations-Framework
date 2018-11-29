using DE.DAXFSA.Framework.BizTalk.Pipeline;
using DE.DAXFSA.Framework.Core.Tracing;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DE.DAXFSA.Framework.PipelineComponents
{
    /// <summary>
    ///
    /// </summary>
    public class MessageDebugger : IIBaseMessageProcessor
    {
        #region private properties

        private IBaseMessage originalMessage;

        #endregion private properties

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public MessageDebugger()
        {
        }

        #endregion Constructor

        #region Implementations

        public string Name
        {
            get { return "MessageDebugger"; }
        }

        public string Description
        {
            get { return "MessageDebugger"; }
        }

        public string Version
        {
            get { return "1.0"; }
        }

        public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage, string objectArgument)
        {
            var callToken = TraceProvider.Logger.TraceIn(this.Name);
            try
            {
                byte CR = 13;
                byte LF = 10;
                byte EQ = 61;

                // get original stream
                Stream messageStream = new ReadOnlySeekableStream(inputMessage.BodyPart.GetOriginalDataStream());
                messageStream.Seek(0, SeekOrigin.Begin);
                pipelineContext.ResourceTracker.AddResource(messageStream);
                using (FileStream fs = new FileStream(string.Format("c:\\temp\\{0}.{1}.txt", Guid.NewGuid().ToString(), DateTime.Now.Ticks), FileMode.CreateNew))
                {
                    while (true)
                    {
                        byte[] buffer = new byte[1024];
                        int m = messageStream.Read(buffer, 0, 1024);
                        if (m > 0)
                        {
                            for (int j = 0; j < m; j++)
                            {
                                int i = Convert.ToInt32(buffer[j]);
                                List<byte> newBuffer = new List<byte>();
                                newBuffer.Add(buffer[j]);
                                newBuffer.Add(EQ);
                                newBuffer.AddRange(Encoding.UTF8.GetBytes("" + i).ToList<byte>());
                                newBuffer.Add(CR);
                                fs.Write(newBuffer.ToArray(), 0, newBuffer.Count);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                originalMessage = inputMessage;

                return inputMessage;
            }
            catch (Exception ex)
            {
                // put component name as a source information in this exception,
                // so the event log in message could reflect this
                ex.Source = this.Name;
                TraceProvider.Logger.TraceError(ex);
                throw ex;
            }
            finally
            {
                TraceProvider.Logger.TraceOut(callToken, this.Name);
            }
        }

        #endregion Implementations
    }
}