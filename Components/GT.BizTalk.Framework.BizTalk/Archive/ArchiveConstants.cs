using System.Collections.Generic;

namespace GT.BizTalk.Framework.BizTalk.Archive
{
    public class ArchiveConstants
    {
        private static readonly string IGNORED_ERROR_CODES = !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("NETUSE_ERRORCODE_IGNORED")) ? System.Environment.GetEnvironmentVariable("NETUSE_ERRORCODE_IGNORED") : "1394,1326,1219,64";
        private static readonly char DELIMITER = ',';

        /// <summary>
        /// List of error codes that will be ignored
        /// </summary>
        /// <returns></returns>
        public static List<string> GetIgnoredErrorCodes()
        {
            string[] codes = IGNORED_ERROR_CODES.Split(DELIMITER);
            return new List<string>(codes);
        }
    }
}