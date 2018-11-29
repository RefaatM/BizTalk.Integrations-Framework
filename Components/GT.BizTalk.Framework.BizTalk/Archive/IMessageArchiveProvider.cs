using System;
using System.IO;

namespace GT.BizTalk.Framework.BizTalk.Archive
{
    /// <summary>
    /// Message Archive Provider
    /// </summary>
    public interface IMessageArchiveProvider
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="providerName"></param>
        /// <param name="storedProcName"></param>
        /// <param name="msgId"></param>
        /// <param name="msgStream"></param>
        /// <param name="xmlProperties"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        bool ArchiveToDb(string connectionString, string providerName, string storedProcName, Guid msgId, Stream msgStream, string xmlProperties, long size);

        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="backupPath"></param>
        /// <param name="overwrite"></param>
        /// <param name="msgStream"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <param name="isCompressed"></param>
        /// <param name="compressionPassword"></param>
        /// <returns></returns>
        bool ArchiveToFile(string fileName, string backupPath, bool overwrite, Stream msgStream, string userName, string password, string domain);
    }
}