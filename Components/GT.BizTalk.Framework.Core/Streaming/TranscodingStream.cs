using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GT.BizTalk.Framework.Core.Streaming
{
    /// <summary>
    /// Performs conversion of encoding on the fly.
    /// </summary>
    public class TranscodingStream : Stream
    {
        #region Fields

        private Encoding sourceEncoding;
        private Encoding targetEncoding;
        private Decoder decoder;
        private Encoder encoder;
        private System.IO.Stream stream;
        private int bufferSize = 4096;
        private List<byte> remainingBytes = new List<byte>();

        #endregion Fields

        #region Construction

        /// <summary>
        /// Instance constructor.
        /// </summary>
        /// <param name="stream">Source stream.</param>
        /// <param name="sourceEncoding">Source encoding.</param>
        /// <param name="targetEncoding">Target encoding.</param>
        /// <param name="bufferSize">Buffer size.</param>
        public TranscodingStream(Stream stream, Encoding sourceEncoding, Encoding targetEncoding, int bufferSize)
        {
            this.stream = stream;
            this.sourceEncoding = sourceEncoding;
            this.targetEncoding = targetEncoding;
            this.decoder = this.sourceEncoding.GetDecoder();
            this.encoder = this.targetEncoding.GetEncoder();
            this.bufferSize = bufferSize;
        }

        #endregion Construction

        #region Read Override

        /// <summary>
        /// Reads and transcodes the bytes from the source to the target encoding.
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="count">Count.</param>
        /// <returns>Number of bytes read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // prepends any remaining bytes to the encoded buffer
            List<byte> encodedBytes = new List<byte>(this.remainingBytes.Count);
            encodedBytes.AddRange(this.remainingBytes);
            this.remainingBytes.RemoveRange(0, this.remainingBytes.Count);

            while (encodedBytes.Count < count)
            {
                // read one chunk from the underlying stream

                byte[] rawBytes = new byte[this.bufferSize];
                int readCount = this.stream.Read(rawBytes, 0, rawBytes.Length);
                if (readCount == 0)
                    break;

                // decode the chunk based on the source encoding

                int charCount = this.decoder.GetCharCount(rawBytes, 0, readCount, false);
                char[] chars = new char[charCount];
                this.decoder.GetChars(rawBytes, 0, readCount, chars, 0, false);

                if (charCount > 0)
                {
                    // encode characters into an encoding buffer based on the target encoding

                    int encodedCount = this.encoder.GetByteCount(chars, 0, charCount, false);
                    byte[] encoding_bytes = new byte[encodedCount];
                    this.encoder.GetBytes(chars, 0, charCount, encoding_bytes, 0, false);

                    if (encodedCount > 0)
                        encodedBytes.AddRange(encoding_bytes);
                }
            }

            // copy the encoded buffer to the requested output buffer

            int outputCount = Math.Min(encodedBytes.Count, count);

            encodedBytes.CopyTo(0, buffer, offset, outputCount);
            encodedBytes.RemoveRange(0, outputCount);
            this.remainingBytes.AddRange(encodedBytes);
            return outputCount;
        }

        #endregion Read Override

        #region System.IO.Stream Overrides

        public override bool CanRead
        {
            get { return this.stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return this.stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return this.stream.Length; }
        }

        public override long Position
        {
            get
            {
                return this.stream.Position;
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public override void SetLength(long value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Flush()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion System.IO.Stream Overrides
    }
}