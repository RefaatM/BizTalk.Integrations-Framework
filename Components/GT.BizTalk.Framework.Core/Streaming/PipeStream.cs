using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GT.BizTalk.Framework.Core.Streaming
{
    /// <summary>
    /// An intermediary buffer to which the concrete StreamTransformer will write, and which will be consumed as BizTalk
    /// (or the next pipeline component in the pipeline) reads from it. Here, "consumed" means that once data is read from
    /// this stream, it is removed from memory. This allows working with the same convenience as a MemoryStream to manage a
    /// variable size buffer, but without keeping everything in memory (and without swapping to disk as a VirtualStream would do).
    /// </summary>
    /// <see cref="http://matricis.com/en/integration-en/tools-to-simply-streaming-pipeline-components-in-biztalk/"/>
    public class PipeStream : Stream, IDisposable
    {
        private byte[] leftOverFromFirstBuffer = null;
        private Queue<byte[]> buffers;
        private bool isOpen;

        public PipeStream()
        {
            isOpen = true;
            buffers = new Queue<byte[]>();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateBufferArgs(buffer, offset, count);
            if (!isOpen)
            {
                throw new ObjectDisposedException(null, "PipeStream is closed");
            }

            int remainingBytesToRead = count;

            if (leftOverFromFirstBuffer != null)
            {
                int bytesRead = ReadFromBufferAndTrim(ref leftOverFromFirstBuffer, buffer, offset, remainingBytesToRead);
                offset += bytesRead;
                remainingBytesToRead -= bytesRead;
            }

            while ((remainingBytesToRead > 0) && (buffers.Count > 0))
            {
                byte[] nextBuffer = buffers.Dequeue();
                int bytesRead = ReadFromBufferAndTrim(ref nextBuffer, buffer, offset, remainingBytesToRead);
                offset += bytesRead;
                remainingBytesToRead -= bytesRead;

                if (nextBuffer != null)
                {
                    // The buffer already contains more bytes than requested.
                    // ReadFromBufferAndTrim therefore has removed the part of
                    // the buffer that was already read. The remaining part will
                    // be read at the next call (and possibly trimmed again)
                    // before going to the next buffer in the queue.
                    leftOverFromFirstBuffer = nextBuffer;
                }
            }

            int totalBytesRead = count - remainingBytesToRead;
            return totalBytesRead;
        }

        private static int ReadFromBufferAndTrim(ref byte[] srcBuffer, byte[] destBuffer, int destOffset, int remainingBytesToRead)
        {
            int bytesToRead = Math.Min(srcBuffer.Length, remainingBytesToRead);
            Buffer.BlockCopy(srcBuffer, 0, destBuffer, destOffset, bytesToRead);

            if (bytesToRead < srcBuffer.Length)
            {
                srcBuffer = TrimBufferStart(srcBuffer, bytesToRead);
            }
            else
            {
                srcBuffer = null;
            }

            return bytesToRead;
        }

        private static byte[] TrimBufferStart(byte[] buffer, int firstByteToKeep)
        {
            int trimmedLength = buffer.Length - firstByteToKeep;
            byte[] trimmedBuffer = new byte[trimmedLength];

            Buffer.BlockCopy(buffer, firstByteToKeep, trimmedBuffer, 0, trimmedLength);
            return trimmedBuffer;
        }

        private void ValidateBufferArgs(byte[] buffer, int offset, int count)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", "offset must be non-negative");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "count must be non-negative");
            }
            if ((buffer.Length - offset) < count)
            {
                throw new ArgumentException("requested count exceeds available size");
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateBufferArgs(buffer, offset, count);
            if (!isOpen)
            {
                throw new ObjectDisposedException(null, "PipeStream is closed");
            }

            byte[] newBuffer = new byte[count];
            Buffer.BlockCopy(buffer, offset, newBuffer, 0, count);
            buffers.Enqueue(newBuffer);
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException("PipeStream is not seekable");
            }
            set
            {
                throw new NotSupportedException("PipeStream is not seekable");
            }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("PipeStream is not seekable");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("PipeStream length can not be changed");
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override long Length
        {
            get
            {
                return
                    (from b in buffers
                     select b.Length).Sum()
                    + leftOverFromFirstBuffer.Length;
            }
        }

        public override void Flush()
        {
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.buffers.Clear();
            this.buffers = null;
            this.leftOverFromFirstBuffer = null;
            this.isOpen = false;
            Close();
        }

        #endregion IDisposable Members

        private static string ToASCIIDebugString(byte[] bytes)
        {
            return String.Join(",", (from b in bytes select ((char)b).ToString()).ToArray());
        }
    }
}