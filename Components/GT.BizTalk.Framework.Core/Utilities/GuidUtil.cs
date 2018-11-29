using System;

namespace GT.BizTalk.Framework.Utilities
{
    public class GuidUtil
    {
        public static Guid? ParseGuid(string input)
        {
            Guid output;

            var parseSuccessful = Guid.TryParse(input, out output);

            return (parseSuccessful) ?
                new Nullable<Guid>(output)
                : null;
        }

        public static Guid ParseGuid2(string input)
        {
            Guid output;

            var parseSuccessful = Guid.TryParse(input, out output);

            return (parseSuccessful) ?
                output
                : Guid.Empty;
        }

        /// <summary>
        /// Decodes a base64 encode string into Guid
        /// </summary>
        /// <param name="encodedGuid"></param>
        /// <returns></returns>
        public static Guid Base64Decode(string encodedGuid)
        {
            var sGuid = Convert.FromBase64String(encodedGuid);
            return new Guid(sGuid);
        }

        /// <summary>
        /// Base64 Encodes a GUID in order to convert into string of length 22
        /// </summary>
        /// <param name="sGuid"></param>
        /// <returns></returns>
        public static string Base64EncodeGuidString(string sGuid)
        {
            return Convert.ToBase64String(new Guid(sGuid).ToByteArray());
        }

        /// <summary>
        /// Base64 Encodes a GUID in order to convert into string of length 22
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string Base64EncodeGuid(Guid guid)
        {
            return Convert.ToBase64String(guid.ToByteArray());
        }

        /// <summary>
        /// ensure the input is a well-form GUID, otherwise, throw exception with provided message
        /// </summary>
        /// <param name="input"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static string EnsureGuidType(string input, string errorMessage)
        {
            var guid = Guid.Empty;
            var parseResult = Guid.TryParse(input, out guid);
            if (!parseResult)
                throw new Exception(errorMessage);
            return input;
        }
    }
}