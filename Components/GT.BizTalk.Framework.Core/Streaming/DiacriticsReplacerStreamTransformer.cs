using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GT.BizTalk.Framework.Core.Streaming
{
    /// <summary>
    /// Stream transformer implementation that replaces diacritic characters with their standard equivalents.
    /// </summary>
    public class DiacriticsReplacerStreamTransformer : StreamTransformer
    {
        #region Fields

        private StreamReader inputReader;
        private StreamWriter outputWriter;
        private int bufferSize;

        // \p{Mn} or \p{Non_Spacing_Mark}:
        //   a character intended to be combined with another
        //   character without taking up extra space
        //   (e.g. accents, umlauts, etc.).
        private readonly static Regex nonSpacingMarkRegex = new Regex(@"\p{Mn}", RegexOptions.Compiled);

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Instance constructor.
        /// </summary>
        /// <param name="inputStream">Input stream instance.</param>
        /// <param name="encoding">Encoding used for reading and writing.</param>
        /// <param name="bufferSize">Buffer size in bytes.</param>
        public DiacriticsReplacerStreamTransformer(Stream inputStream, int bufferSize)
            : base(inputStream)
        {
            this.bufferSize = bufferSize;
            // read original stream and detect encoding
            this.inputReader = new StreamReader(base.InputStream, true);
            this.outputWriter = new StreamWriter(base.OutputStream);
        }

        /// <summary>
        /// Instance constructor.
        /// </summary>
        /// <param name="inputStream">Input stream instance.</param>
        /// <param name="encoding">Encoding used for reading and writing.</param>
        /// <param name="bufferSize">Block size in bytes.</param>
        public DiacriticsReplacerStreamTransformer(Stream inputStream, Encoding encoding, int bufferSize)
            : base(inputStream)
        {
            this.bufferSize = bufferSize;
            // read original stream and detect encoding
            this.inputReader = new StreamReader(base.InputStream, encoding);
            this.outputWriter = new StreamWriter(base.OutputStream, encoding);
        }

        #endregion Constructor

        #region Implementation

        /// <summary>
        /// Transformer method.
        /// </summary>
        /// <returns>true if it can read, false otherwise.</returns>
        protected override bool PartialReadAndTransformInput()
        {
            int bytesToRead = this.bufferSize;

            // read from stream into buffer
            char[] inputBuffer = new char[bytesToRead];
            int charsRead = this.inputReader.Read(inputBuffer, 0, bytesToRead);
            // if there are characters in the buffer
            if (charsRead > 0)
            {
                // convert the char array into a string
                string stringBuffer = new string(inputBuffer, 0, charsRead);
                // replace diacritics
                stringBuffer = this.ReplaceDiacritics(stringBuffer);
                // write to the output stream
                this.outputWriter.Write(stringBuffer.ToArray(), 0, stringBuffer.Length);
                return true;
            }
            else
            {
                // make sure the remaining data in the buffer is written
                this.outputWriter.Flush();
                // no more data to transform in the input stream
                return false;
            }
        }

        #endregion Implementation

        #region Helpers

        private string ReplaceDiacritics(string text)
        {
            if (text == null)
                return string.Empty;

            // remove <SUB> characters
            string normalizedText = text.Replace("\u001A", " ");
            // normalize accented characters
            normalizedText = normalizedText.Normalize(NormalizationForm.FormD);
            // remove extra characters added after normalization
            return nonSpacingMarkRegex.Replace(normalizedText, string.Empty);
        }

        #endregion Helpers
    }
}