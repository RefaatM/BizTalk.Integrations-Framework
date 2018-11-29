using System;
using System.IO;

namespace GT.BizTalk.Framework.Core.Streaming
{
    /// <summary>
    /// Custom stream transformer exception.
    /// </summary>
    [Serializable]
    public class StreamTransformerException : IOException
    {
        public StreamTransformerException()
            : base()
        {
        }

        public StreamTransformerException(string message)
            : base(message)
        {
        }

        public StreamTransformerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}