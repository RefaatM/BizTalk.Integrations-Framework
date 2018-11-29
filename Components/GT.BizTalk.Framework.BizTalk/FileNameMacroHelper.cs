using Microsoft.BizTalk.Message.Interop;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace GT.BizTalk.Framework.BizTalk
{
    /// <summary>
    /// Helper class to generate file names using supported macros.
    /// It supports the BizTalk %SourceFileName% and %MessageID% macros and
    /// the custom macros:
    ///     - %datetime:format%
    ///     - %BodyPartName%
    ///     - %InterchangeID%
    ///     - %InterchangeSequenceNumber%
    ///     - %TransportType%
    ///     - %ReceivePortName%
    ///
    /// The custom date/time macro has the following syntax: %datetime:format%
    /// where: format is any valid .NET date formatting string.
    ///
    /// Pattern example: %datetime:yyyMMddHHmmss%_%MessageID%_%SourceFileName%.bak
    /// which will output something like this: 20130101120030_{d1a62e55-7924-476c-9da8-bc538573ae47}_Invoice.csv.bak
    /// </summary>
    public static class FileNameMacroHelper
    {
        #region Constants

        private const string DATETIME_MACRO_PATTERN = "(?:%datetime:(?<format>.+?)%)";
        private const string SOURCE_FILE_NAME_MACRO_PATTERN = "%SourceFileName%";
        private const string BODY_PART_NAME_MACRO_PATTERN = "%BodyPartName%";
        private const string INTERCHANGE_ID_MACRO_PATTERN = "%InterchangeID%";
        private const string INTERCHANGE_SEQ_NO_MACRO_PATTERN = "%InterchangeSequenceNumber%";
        private const string TRANSPORT_LOC_MACRO_PATTERN = "%TransportType%";
        private const string RECEIVE_PORT_NAME_MACRO_PATTERN = "%ReceivePortName%";
        private const string MESSAGE_ID_MACRO_PATTERN = "%MessageID%";
        private const RegexOptions REGEX_OPTIONS = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace;

        #endregion Constants

        #region Fields

        private static readonly Regex DateTimeMacroRegex = new Regex(DATETIME_MACRO_PATTERN, REGEX_OPTIONS);
        private static readonly Regex SourceFileNameMacroRegex = new Regex(SOURCE_FILE_NAME_MACRO_PATTERN, REGEX_OPTIONS);
        private static readonly Regex BodyPartNameMacroRegex = new Regex(BODY_PART_NAME_MACRO_PATTERN, REGEX_OPTIONS);
        private static readonly Regex InterchangeIDMacroRegex = new Regex(INTERCHANGE_ID_MACRO_PATTERN, REGEX_OPTIONS);
        private static readonly Regex InterchangeSequenceNumberMacroRegex = new Regex(INTERCHANGE_SEQ_NO_MACRO_PATTERN, REGEX_OPTIONS);
        private static readonly Regex TransportTypeMacroRegex = new Regex(TRANSPORT_LOC_MACRO_PATTERN, REGEX_OPTIONS);
        private static readonly Regex ReceivePortNameMacroRegex = new Regex(RECEIVE_PORT_NAME_MACRO_PATTERN, REGEX_OPTIONS);
        private static readonly Regex MessageIDMacroRegex = new Regex(MESSAGE_ID_MACRO_PATTERN, REGEX_OPTIONS);

        #endregion Fields

        #region Public Methods

        /// <summary>
        /// Resolves/evaluates the macros in the specified file pattern for the given BizTalk message instance to return
        /// a filename.
        /// </summary>
        /// <param name="btsMessage">Input BizTalk message instance.</param>
        /// <param name="fileNamePattern">Filename pattern.</param>
        /// <returns>Filename generated with the specified pattern.</returns>
        public static string ResolveMacros(IBaseMessage btsMessage, string fileNamePattern)
        {
            // generate the new filename using the specified filename pattern
            // start by assigning the pattern to the output filename
            string outputFileName = fileNamePattern;

            outputFileName = ResolveDateTimeMacros(btsMessage, outputFileName);
            outputFileName = ResolveFilePropertiesMacros(btsMessage, outputFileName);
            outputFileName = ResolveMessageMacros(btsMessage, outputFileName);
            outputFileName = ResolveSystemPropertiesMacros(btsMessage, outputFileName);

            // return generated filename
            return outputFileName;
        }

        #endregion Public Methods

        #region Resolver helpers

        private static string ResolveDateTimeMacros(IBaseMessage btsMessage, string fileNamePattern)
        {
            // generate the new filename using the specified filename pattern
            // start by assigning the pattern to the output filename
            string outputFileName = fileNamePattern;

            // replace the %datetime:format% macro with the formatted datetime string(s)
            // NOTE: the formatting string in each instance of the datetime macro could be
            // different, so we need to use a delegate to evaluate each formatting
            if (DateTimeMacroRegex.IsMatch(outputFileName) == true)
            {
                outputFileName = DateTimeMacroRegex.Replace(outputFileName, new MatchEvaluator(
                    delegate(Match match)
                    {
                        return DateTime.Now.ToString(match.Groups["format"].Value);
                    }
                ));
            }

            // return generated filename
            return outputFileName;
        }

        private static string ResolveMessageMacros(IBaseMessage btsMessage, string fileNamePattern)
        {
            // generate the new filename using the specified filename pattern
            // start by assigning the pattern to the output filename
            string outputFileName = fileNamePattern;

            // replace the %MessageID% macro
            if (MessageIDMacroRegex.IsMatch(outputFileName) == true)
            {
                outputFileName = MessageIDMacroRegex.Replace(outputFileName, btsMessage.MessageID.ToString());
            }

            // replace the %BodyPartName% macro
            if (BodyPartNameMacroRegex.IsMatch(outputFileName) == true)
            {
                outputFileName = BodyPartNameMacroRegex.Replace(outputFileName, btsMessage.BodyPartName.ToString());
            }

            // return generated filename
            return outputFileName;
        }

        private static string ResolveFilePropertiesMacros(IBaseMessage btsMessage, string fileNamePattern)
        {
            // generate the new filename using the specified filename pattern
            // start by assigning the pattern to the output filename
            string outputFileName = fileNamePattern;

            // replace the %SourceFileName% macro with the the "ReceivedFileName" context property
            if (SourceFileNameMacroRegex.IsMatch(outputFileName) == true)
            {
                // read the "ReceivedFileName" property from the message context
                string receivedFilePath = btsMessage.Context.Read<string>(BtsProperties.ReceivedFileName.Name, BtsProperties.ReceivedFileName.Namespace, null);
                if (string.IsNullOrEmpty(receivedFilePath) == false)
                {
                    // get just the filename from the full receivedFilePath (same behavior as the BizTalk %SourceFileName% macro)
                    string fileName = Path.GetFileName(receivedFilePath);
                    outputFileName = SourceFileNameMacroRegex.Replace(outputFileName, fileName);
                }
            }

            // return generated filename
            return outputFileName;
        }

        private static string ResolveSystemPropertiesMacros(IBaseMessage btsMessage, string fileNamePattern)
        {
            // generate the new filename using the specified filename pattern
            // start by assigning the pattern to the output filename
            string outputFileName = fileNamePattern;

            // replace the %InterchangeID% macro
            if (InterchangeIDMacroRegex.IsMatch(outputFileName) == true)
            {
                string value = btsMessage.Context.Read<string>(BtsProperties.InterchangeID.Name, BtsProperties.InterchangeID.Namespace, null);
                if (string.IsNullOrEmpty(value) == false)
                {
                    outputFileName = InterchangeIDMacroRegex.Replace(outputFileName, value);
                }
            }

            // replace the %InterchangeSequenceNumber% macro
            if (InterchangeSequenceNumberMacroRegex.IsMatch(outputFileName) == true)
            {
                string value = btsMessage.Context.Read<string>(BtsProperties.InterchangeSequenceNumber.Name, BtsProperties.InterchangeSequenceNumber.Namespace, null);
                if (string.IsNullOrEmpty(value) == false)
                {
                    outputFileName = InterchangeSequenceNumberMacroRegex.Replace(outputFileName, value);
                }
            }

            // replace the %TransportType% macro
            if (TransportTypeMacroRegex.IsMatch(outputFileName) == true)
            {
                string value = btsMessage.Context.Read<string>(BtsProperties.OutboundTransportType.Name, BtsProperties.OutboundTransportType.Namespace, null);
                if (string.IsNullOrEmpty(value) == false)
                {
                    value = btsMessage.Context.Read<string>(BtsProperties.InboundTransportType.Name, BtsProperties.InboundTransportType.Namespace, null);
                }
                if (string.IsNullOrEmpty(value) == false)
                {
                    outputFileName = TransportTypeMacroRegex.Replace(outputFileName, value);
                }
            }

            // replace the %ReceivePortName% macro
            if (ReceivePortNameMacroRegex.IsMatch(outputFileName) == true)
            {
                string value = btsMessage.Context.Read<string>(BtsProperties.ReceivePortName.Name, BtsProperties.ReceivePortName.Namespace, null);
                if (string.IsNullOrEmpty(value) == false)
                {
                    outputFileName = ReceivePortNameMacroRegex.Replace(outputFileName, value);
                }
            }

            // return generated filename
            return outputFileName;
        }

        #endregion Resolver helpers
    }
}