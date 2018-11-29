using System;
using System.IO;

namespace GT.BizTalk.Framework.Core.Streaming
{
    /// <summary>
    /// An abstract base class that decorates a Stream with transformation code. Its implementation of the Stream
    /// "Read" method calls our transformation code. However, it doesn't do the whole transformation in one shot.
    /// Instead, it will check how many bytes were requested by the caller, and will only call the transformation
    /// code enough times to fill the buffer with that number of bytes. It will then return the buffer to this caller,
    /// which will be able to do its own incremental processing on this buffer.
    /// </summary>
    /// <see cref="http://matricis.com/en/integration-en/tools-to-simply-streaming-pipeline-components-in-biztalk/"/>
    public abstract class StreamTransformer : Stream, IDisposable
    {
        #region Fields

        private Stream inputStream;
        private PipeStream outputStream = null;
        private bool reachedEndOfInputStream = false;
        private bool reachedEndOfOutputStream = false;

        #endregion Fields

        #region Constructor

        public StreamTransformer(Stream inputStream)
        {
            this.inputStream = inputStream;
            this.outputStream = new PipeStream();
        }

        #endregion Constructor

        #region Protected interface for concrete implementations

        /// <returns>Should return true if there was input to read/transform, false otherwise (if the end of input stream is reached)</returns>
        protected abstract bool PartialReadAndTransformInput();

        protected Stream InputStream { get { return this.inputStream; } }
        protected Stream OutputStream { get { return this.outputStream; } }

        #endregion Protected interface for concrete implementations

        #region Read override

        /// <summary>
        /// Reads bytes from the specified buffer.
        /// </summary>
        /// <param name="buffer">Input buffer.</param>
        /// <param name="offset">Buffer offset.</param>
        /// <param name="count">Count of bytes.</param>
        /// <returns>Bytes read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            try
            {
                int totalBytesWritten = 0;

                while (totalBytesWritten < count && !reachedEndOfOutputStream)
                {
                    if (!reachedEndOfInputStream)
                    {
                        reachedEndOfInputStream = !PartialReadAndTransformInput();
                    }

                    int outputOffset = offset + totalBytesWritten;
                    int remainingBytesToWrite = count - totalBytesWritten;
                    int bytesWritten = outputStream.Read(buffer, outputOffset, remainingBytesToWrite);

                    totalBytesWritten += bytesWritten;

                    if (reachedEndOfInputStream && bytesWritten == 0)
                    {
                        reachedEndOfOutputStream = true;
                    }
                }

                return totalBytesWritten;
            }
            catch (Exception ex)
            {
                // ensure the error message clearly indicates that the
                // error occurred in StreamTransformer
                // (without having to look at the stack trace)
                //
                // Otherwise, since this transformer will be called by internal BizTalk
                // we may be mislead into thinking the error occurred in BizTalk
                // (for example in an adapter) instead of in the transformation.
                throw new StreamTransformerException("StreamTransformer.Read failed: " + ex.Message, ex);
            }
        }

        #endregion Read override

        #region Stream overrides

        public override bool CanSeek
        {
            get
            {
                //as the stream is bufferring, additional code would have been required to support seeking
                //(mostly getting read of the buffer on seek), but this is not needed for the BizTalk scenario
                //(BizTalk itself will never seek the stream), so I've avoided adding this code
                return false;
            }
        }

        public override long Position
        {
            get
            {
                return inputStream.Position;
            }
            set
            {
                throw new NotSupportedException("StreamTransformer is not seekable");
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false; //stream is non-writable due to the way it buffers data, but this is not a requirement usually in this context
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("StreamTransformer is not seekable");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("StreamTransformer is not writable");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("StreamTransformer is not writable");
        }

        #endregion Stream overrides

        #region Stream overrides - all of these are forwarded to the underlying stream

        public override bool CanRead
        {
            get { return inputStream.CanRead; }
        }

        public override void Flush()
        {
            inputStream.Flush();
        }

        public override long Length
        {
            get { return inputStream.Length; }
        }

        #endregion Stream overrides - all of these are forwarded to the underlying stream

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion IDisposable Members
    }
}