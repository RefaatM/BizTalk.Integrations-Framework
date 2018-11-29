using System.IO;

namespace GT.BizTalk.Framework.Core.Streaming
{
    /// <summary>
    /// This is a wrapper for a FileStream. It implements the
    /// basic Stream methods and will delete the wrapped FileStream's
    /// file when closed.
    /// </summary>
    public class SelfDeletingFileStream : Stream
    {
        #region Fields

        private FileStream innerStream;
        private string fileName;

        #endregion Fields

        #region Constructor

        public SelfDeletingFileStream(FileStream fileStream)
        {
            this.innerStream = fileStream;
            this.fileName = fileStream.Name;
        }

        #endregion Constructor

        #region Overrides

        // when biztalk closes this stream we will delete the underlying file.
        public override void Close()
        {
            if (this.innerStream != null)
            {
                this.innerStream.Close();
                Tracing.TraceProvider.Logger.TraceInfo("Self-deleting file: {0}", this.innerStream.Name);
                File.Delete(this.innerStream.Name);
                this.innerStream.Dispose();
                this.innerStream = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        public override bool CanRead
        {
            get { return this.innerStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return this.innerStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return this.innerStream.CanWrite; }
        }

        public override void Flush()
        {
            this.innerStream.Flush();
        }

        public override long Length
        {
            get { return this.innerStream.Length; }
        }

        public override long Position
        {
            get
            {
                return this.innerStream.Position;
            }
            set
            {
                this.innerStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.innerStream.Write(buffer, offset, count);
        }

        #endregion Overrides
    }
}