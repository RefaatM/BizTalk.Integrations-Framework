using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GT.BizTalk.Framework.Core.Streaming
{
    /// <summary>
    /// Stream extension methods.
    /// </summary>
    public static class StreamExentions
    {
        #region Constants

        private static readonly string DEFAULT_DELIMITER = Environment.NewLine;
        private const int DEFAULT_BUFFER_SIZE = 1024 * 4; // 4K
        private const StringSplitOptions DEFAULT_SPLIT_OPTIONS = StringSplitOptions.RemoveEmptyEntries;

        #endregion Constants

        #region Split Extensions

        /// <summary>
        /// Splits the source stream into lines (records) using the default settings.
        /// </summary>
        /// <remarks>
        /// The implementation uses "yield" for more efficiency to return an IEnumerable
        /// allowing the caller to process the lines as they are parsed.
        /// </remarks>
        /// <param name="stream">Stream containing the data to be split.</param>
        /// <returns>IEnumerable containing the split lines (records).</returns>
        public static IEnumerable<string> Split(this Stream stream)
        {
            return stream.Split(DEFAULT_DELIMITER, DEFAULT_SPLIT_OPTIONS, DEFAULT_BUFFER_SIZE);
        }

        /// <summary>
        /// Splits the source stream into lines (records) using the specified delimiter.
        /// </summary>
        /// <remarks>
        /// The implementation uses "yield" for more efficiency to return an IEnumerable
        /// allowing the caller to process the lines as they are parsed.
        /// </remarks>
        /// <param name="stream">Stream containing the data to be split.</param>
        /// <param name="delimiter">Delimiter used to split the source stream.</param>
        /// <returns>IEnumerable containing the split lines (records).</returns>
        public static IEnumerable<string> Split(this Stream stream, string delimiter)
        {
            return stream.Split(delimiter, DEFAULT_SPLIT_OPTIONS, DEFAULT_BUFFER_SIZE);
        }

        /// <summary>
        /// Splits the source stream into lines (records) using the specified delimiter and options.
        /// </summary>
        /// <remarks>
        /// The implementation uses "yield" for more efficiency to return an IEnumerable
        /// allowing the caller to process the lines as they are parsed.
        /// </remarks>
        /// <param name="stream">Stream containing the data to be split.</param>
        /// <param name="delimiter">Delimiter used to split the source stream.</param>
        /// <param name="options">Split options. Used to specified if empty lines are removed.</param>
        /// <returns>IEnumerable containing the split lines (records).</returns>
        public static IEnumerable<string> Split(this Stream stream, string delimiter, StringSplitOptions options)
        {
            return stream.Split(delimiter, options, DEFAULT_BUFFER_SIZE);
        }

        /// <summary>
        /// Splits the source stream into lines (records) using the specified delimiter and buffer size.
        /// </summary>
        /// <remarks>
        /// The implementation uses "yield" for more efficiency to return an IEnumerable
        /// allowing the caller to process the lines as they are parsed.
        /// </remarks>
        /// <param name="stream">Stream containing the data to be split.</param>
        /// <param name="delimiter">Delimiter used to split the source stream.</param>
        /// <param name="bufferSize">Buffer size.</param>
        /// <returns>IEnumerable containing the split lines (records).</returns>
        public static IEnumerable<string> Split(this Stream stream, string delimiter, int bufferSize)
        {
            return stream.Split(delimiter, DEFAULT_SPLIT_OPTIONS, bufferSize);
        }

        /// <summary>
        /// Splits the source stream into lines (records) using the specified buffer size.
        /// </summary>
        /// <remarks>
        /// The implementation uses "yield" for more efficiency to return an IEnumerable
        /// allowing the caller to process the lines as they are parsed.
        /// </remarks>
        /// <param name="stream">Stream containing the data to be split.</param>
        /// <param name="bufferSize">Buffer size.</param>
        /// <returns>IEnumerable containing the split lines (records).</returns>
        public static IEnumerable<string> Split(this Stream stream, int bufferSize)
        {
            return stream.Split(DEFAULT_DELIMITER, DEFAULT_SPLIT_OPTIONS, bufferSize);
        }

        /// <summary>
        /// Splits the source stream into lines (records) using the specified delimiter, options and buffer size.
        /// </summary>
        /// <remarks>
        /// The implementation uses "yield" for more efficiency to return an IEnumerable
        /// allowing the caller to process the lines as they are parsed.
        /// </remarks>
        /// <param name="stream">Stream containing the data to be split.</param>
        /// <param name="delimiter">Delimiter used to split the source stream.</param>
        /// <param name="options">Split options. Used to specified if empty lines are removed.</param>
        /// <param name="bufferSize">Buffer size.</param>
        /// <returns>IEnumerable containing the split lines (records).</returns>
        public static IEnumerable<string> Split(this Stream stream, string delimiter, StringSplitOptions options, int bufferSize)
        {
            // buffer used to process lines
            StringBuilder stringBuffer = new StringBuilder(bufferSize);
            // use a StreamReader to read block of chars from the source stream
            using (var reader = new StreamReader(stream, Encoding.UTF8, true, bufferSize, true))
            {
                char[] rawCharBuffer = new char[bufferSize]; // buffer of raw chars to read from the source stream
                int charsRead = 0; // chars read into the buffer from the source stream
                do
                {
                    // read a block of chars into the buffer
                    charsRead = reader.ReadBlock(rawCharBuffer, 0, rawCharBuffer.Length);
                    // append to the string buffer used process lines
                    stringBuffer.Append(rawCharBuffer, 0, charsRead);

                    string text = stringBuffer.ToString(); // convert to a text string
                    int index = 0; // index of the position where the delimiter is found
                    int totalChars = 0; // total number of chars copied to the output line

                    // iterate through the text searching extracting lines separated by the delimiter
                    while ((index = text.IndexOf(delimiter, index)) >= 0)
                    {
                        // extract the line up to the delimiter (exclude delimiter)
                        string line = text.Substring(totalChars, index - totalChars);
                        // advance the index to a position after the delimiter (skip it)
                        index += delimiter.Length;
                        // return the line (if not empty or if empty lines are not removed)
                        if (options != StringSplitOptions.RemoveEmptyEntries || line != string.Empty)
                        {
                            yield return line;
                        }
                        // update total number of chars retuned
                        totalChars = index;
                    }

                    // the string buffer may still have unprocessed chars;
                    // remove the chars already processed from the begining of the buffer;
                    // this will cause the remaining chars to be shifted to the begining,
                    // so the new block will be appended to the end again:
                    stringBuffer.Remove(0, totalChars);
                } while (charsRead == rawCharBuffer.Length);
            }

            // return the remaining chars left in the string buffer (if not empty or if empty lines are not removed)
            if (options != StringSplitOptions.RemoveEmptyEntries || stringBuffer.Length > 0)
            {
                yield return stringBuffer.ToString();
            }
        }

        #endregion Split Extensions
    }
}