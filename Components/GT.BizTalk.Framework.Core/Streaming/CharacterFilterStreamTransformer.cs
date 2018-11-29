using System.Collections.Generic;
using System.IO;

namespace GT.BizTalk.Framework.Core.Streaming
{
    /// <summary>
    /// Stream transformer implementation that filters out (removes) characters.
    /// </summary>
    public class CharacterFilterStreamTransformer : StreamTransformer
    {
        #region Fields

        private int blockSize;
        private char[] charsToRemove;
        private StreamReader inputReader;
        private StreamWriter outputWriter;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Instance constructor.
        /// </summary>
        /// <param name="inputStream">Input stream instance.</param>
        /// <param name="blockSize">Block size in bytes.</param>
        /// <param name="charsToRemove">Characters to remove from the input stream.</param>
        public CharacterFilterStreamTransformer(Stream inputStream, int blockSize, char[] charsToRemove)
            : base(inputStream)
        {
            this.blockSize = blockSize;
            this.charsToRemove = charsToRemove;
            // read original stream and detect encoding
            this.inputReader = new StreamReader(InputStream, true);
            this.outputWriter = new StreamWriter(base.OutputStream);
        }

        #endregion Constructor

        #region Implementation

        /// <summary>
        /// Transformer method.
        /// </summary>
        /// <returns>true if it can read, false otherwise.</returns>
        protected override bool PartialReadAndTransformInput()
        {
            int bytesToRead = blockSize;

            // read from stream into buffer
            char[] inputBuffer = new char[bytesToRead];
            int charsRead = inputReader.Read(inputBuffer, 0, bytesToRead);
            // if there are characters in the buffer
            if (charsRead > 0)
            {
                // create a queue to store interim results
                Queue<char> output = new Queue<char>();
                // loop through buffer characters
                for (int i = 0; i < charsRead; i++)
                {
                    // if character is in the filter list, do not add it
                    // to the result queue
                    bool addItem = true;
                    char current = inputBuffer[i];
                    for (int j = 0; j < charsToRemove.Length; j++)
                    {
                        if (current == charsToRemove[j])
                        {
                            addItem = false;
                            break;
                        }
                    }
                    if (addItem == true)
                        output.Enqueue(current);
                }
                // write to the output stream
                this.outputWriter.Write(output.ToArray(), 0, output.Count);
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
    }
}