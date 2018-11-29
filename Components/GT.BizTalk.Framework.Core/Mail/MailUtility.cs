using GT.BizTalk.Framework.Core.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace GT.BizTalk.Framework.Core.Mail
{
    /// <summary>
    /// Provides utility methods for common mail creation and sending operations.
    /// </summary>
    public static class MailUtility
    {
        #region SendMail Methods

        /// <summary>
        /// Sends an email using the specified set of parameters.
        /// </summary>
        /// <param name="from">Sender's e-mail address. Could be null, in which case the default sender e-mail specified via smtp configuration will be used.</param>
        /// <param name="to">Comma-separated list of e-mail addresses to send the message to.</param>
        /// <param name="cc">Comma-separated list of e-mail addresses to send a copy (CC) of the message to.</param>
        /// <param name="bcc">Comma-separated list of e-mail addresses to send a blind copy (BCC) of the message to.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="priority">Email priority.</param>
        /// <param name="htmlTemplateFileName">Html template file name.</param>
        /// <param name="plainTemplateFileName">Plain text template file name. Could be <b>null</b>.</param>
        /// <param name="styleSheetFileName">Name of a file containing styles to be added to the email.</param>
        /// <param name="replacements">Collection of replacement arguments.</param>
        /// <param name="linkedResources">Collection containing information about linked resources.</param>
        /// <param name="attachments">Collection containing information about attachments.</param>
        public static void SendMail(
            string from,
            string to,
            string cc,
            string bcc,
            string subject,
            MailPriority priority,
            string htmlTemplateFileName,
            string plainTemplateFileName,
            string styleSheetFileName,
            IDictionary<string, string> replacements,
            IEnumerable<ILinkedResourceInfo> linkedResources,
            IEnumerable<IAttachmentInfo> attachments)
        {
            // create mail message
            using (MailMessage message = MailUtility.CreateMessage(
                from,
                to,
                cc,
                bcc,
                subject,
                priority,
                htmlTemplateFileName,
                plainTemplateFileName,
                styleSheetFileName,
                replacements,
                linkedResources,
                attachments))
            {
                // create instance of the SMTP client using default settings
                // from the configuration file
                using (SmtpClient smtpClient = new SmtpClient())
                {
                    // send mail
                    smtpClient.Send(message);
                }
            }
        }

        /// <summary>
        /// Sends an email using the specified set of parameters.
        /// </summary>
        /// <param name="from">Sender's e-mail address. Could be null, in which case the default sender e-mail specified via smtp configuration will be used.</param>
        /// <param name="to">Comma-separated list of e-mail addresses to send the message to.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="htmlTemplateFileName">Html template file name.</param>
        /// <param name="replacements">Collection of replacement arguments.</param>
        public static void SendMail(
            string from,
            string to,
            string subject,
            string htmlTemplateFileName,
            IDictionary<string, string> replacements)
        {
            MailUtility.SendMail(
                from,
                to,
                null, //cc
                null, //bcc
                subject,
                MailPriority.Normal,
                htmlTemplateFileName,
                null, //plainTemplateFileName
                null, //styleSheetFileName
                replacements,
                null, //linkedResources
                null); //attachments
        }

        /// <summary>
        /// Sends an email using the specified set of parameters.
        /// </summary>
        /// <param name="from">Sender's e-mail address. Could be null, in which case the default sender e-mail specified via smtp configuration will be used.</param>
        /// <param name="to">Comma-separated list of e-mail addresses to send the message to.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="htmlTemplateFileName">Html template file name.</param>
        /// <param name="plainTemplateFileName">Plain text template file name. Could be <b>null</b>.</param>
        /// <param name="replacements">Collection of replacement arguments.</param>
        public static void SendMail(
            string from,
            string to,
            string subject,
            string htmlTemplateFileName,
            string plainTemplateFileName,
            IDictionary<string, string> replacements)
        {
            MailUtility.SendMail(
                from,
                to,
                null, //cc
                null, //bcc
                subject,
                MailPriority.Normal,
                htmlTemplateFileName,
                plainTemplateFileName,
                null, //styleSheetFileName
                replacements,
                null, //linkedResources
                null); //attachments
        }

        /// <summary>
        /// Sends an email using the specified set of parameters.
        /// </summary>
        /// <param name="from">Sender's e-mail address. Could be null, in which case the default sender e-mail specified via smtp configuration will be used.</param>
        /// <param name="to">Comma-separated list of e-mail addresses to send the message to.</param>
        /// <param name="cc">Comma-separated list of e-mail addresses to send a copy (CC) of the message to.</param>
        /// <param name="bcc">Comma-separated list of e-mail addresses to send a blind copy (BCC) of the message to.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="htmlTemplateFileName">Html template file name.</param>
        /// <param name="plainTemplateFileName">Plain text template file name. Could be <b>null</b>.</param>
        /// <param name="styleSheetFileName">Name of a file containing styles to be added to the email.</param>
        /// <param name="replacements">Collection of replacement arguments.</param>
        public static void SendMail(
            string from,
            string to,
            string cc,
            string bcc,
            string subject,
            string htmlTemplateFileName,
            string plainTemplateFileName,
            string styleSheetFileName,
            IDictionary<string, string> replacements)
        {
            MailUtility.SendMail(
                from,
                to,
                cc,
                bcc,
                subject,
                MailPriority.Normal,
                htmlTemplateFileName,
                plainTemplateFileName,
                styleSheetFileName,
                replacements,
                null, //linkedResources
                null); //attachments
        }

        #endregion SendMail Methods

        #region CreateMessage

        /// <summary>
        /// Creates a new instance of a MailMessage using the specified parameters.
        /// </summary>
        /// <param name="from">Sender's e-mail address. Could be null, in which case the default sender e-mail specified via smtp configuration will be used.</param>
        /// <param name="to">Comma-separated list of e-mail addresses to send the message to.</param>
        /// <param name="cc">Comma-separated list of e-mail addresses to send a copy (CC) of the message to.</param>
        /// <param name="bcc">Comma-separated list of e-mail addresses to send a blind copy (BCC) of the message to.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="priority">Email priority.</param>
        /// <param name="htmlTemplateFileName">Html template file name.</param>
        /// <param name="plainTemplateFileName">Plain text template file name. Could be <b>null</b>.</param>
        /// <param name="styleSheetFileName">Name of a file containing styles to be added to the email.</param>
        /// <param name="replacements">Collection of replacement arguments.</param>
        /// <param name="linkedResources">Collection containing information about linked resources.</param>
        /// <param name="attachments">Collection containing information about attachments.</param>
        /// <returns>New instance of a Mail Message.</returns>
        public static MailMessage CreateMessage(
            string from,
            string to,
            string cc,
            string bcc,
            string subject,
            MailPriority priority,
            string htmlTemplateFileName,
            string plainTemplateFileName,
            string styleSheetFileName,
            IDictionary<string, string> replacements,
            IEnumerable<ILinkedResourceInfo> linkedResources,
            IEnumerable<IAttachmentInfo> attachments)
        {
            // copy replacements to our internal dictionary
            Dictionary<string, string> replacementArgs = new Dictionary<string, string>(replacements);

            // load styles, they will be automatically added to the email
            string physicalStyleSheetFileName = VirtualPathResolver.MapPath(styleSheetFileName);
            if (string.IsNullOrEmpty(physicalStyleSheetFileName) == false && File.Exists(physicalStyleSheetFileName) == true)
            {
                string styles = File.ReadAllText(physicalStyleSheetFileName);
                replacementArgs.Add("<%Styles%>", styles);
            }

            // create an instance of the MailMessage
            MailMessage message = new MailMessage();
            message.To.Add(to);
            message.Priority = priority;

            if (string.IsNullOrEmpty(from) == false)
            {
                MailAddress fromAddress = new MailAddress(from);
                message.From = fromAddress;
                message.Sender = fromAddress;
            }
            if (string.IsNullOrEmpty(cc) == false)
                message.CC.Add(cc);
            if (string.IsNullOrEmpty(bcc) == false)
                message.Bcc.Add(bcc);
            if (string.IsNullOrEmpty(subject) == false)
                message.Subject = subject;

            // load html body
            string htmlBodyTemplate = MailUtility.LoadMessageBody(htmlTemplateFileName);
            message.Body = MailUtility.BuildMessageBody(htmlBodyTemplate, replacementArgs);
            message.IsBodyHtml = true;

            // add the alternate body to the message
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message.Body);
            htmlView.ContentType.MediaType = "text/html";
            message.AlternateViews.Add(htmlView);

            // add plain text alternate body if plain text template was provided
            if (string.IsNullOrEmpty(plainTemplateFileName) == false)
            {
                // add the alternate body to the message
                string plainBodyTemplate = MailUtility.LoadMessageBody(plainTemplateFileName);
                string plainBody = MailUtility.BuildMessageBody(plainBodyTemplate, replacementArgs);
                AlternateView plainView = AlternateView.CreateAlternateViewFromString(plainBody);
                plainView.ContentType.MediaType = "text/plain";
                message.AlternateViews.Add(plainView);
            }

            // add linked resources
            if (linkedResources != null)
            {
                foreach (ILinkedResourceInfo resourceInfo in linkedResources)
                {
                    // create the LinkedResource
                    string physicalResourceFileName = VirtualPathResolver.MapPath(resourceInfo.FileName);
                    LinkedResource linkedResource = new LinkedResource(physicalResourceFileName, resourceInfo.MediaType);
                    linkedResource.ContentId = resourceInfo.ContentId;
                    // add the LinkedResource to the appropriate view
                    htmlView.LinkedResources.Add(linkedResource);
                }
            }

            // add attachments
            if (attachments != null)
            {
                foreach (IAttachmentInfo attachmentInfo in attachments)
                {
                    // create the file attachment for this e-mail message
                    string physicalAttachmentFileName = VirtualPathResolver.MapPath(attachmentInfo.FileName);
                    Attachment attachment = new Attachment(physicalAttachmentFileName, attachmentInfo.MediaType);
                    // add file information
                    attachment.ContentDisposition.CreationDate = File.GetCreationTime(attachmentInfo.FileName);
                    attachment.ContentDisposition.ModificationDate = File.GetLastWriteTime(attachmentInfo.FileName);
                    attachment.ContentDisposition.ReadDate = File.GetLastAccessTime(attachmentInfo.FileName);
                    // add the file attachment to this e-mail message
                    message.Attachments.Add(attachment);
                }
            }

            return message;
        }

        /// <summary>
        /// Creates a new instance of a MailMessage using the specified information.
        /// </summary>
        /// <param name="from">Sender's e-mail address. Could be null, in which case the default sender e-mail specified via smtp configuration will be used.</param>
        /// <param name="to">Comma-separated list of e-mail addresses to send the message to.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="htmlTemplateFileName">Html template file name.</param>
        /// <param name="replacements">Collection of replacement arguments.</param>
        /// <returns>New instance of a Mail Message.</returns>
        public static MailMessage CreateMessage(
            string from,
            string to,
            string subject,
            string htmlTemplateFileName,
            IDictionary<string, string> replacements)
        {
            return MailUtility.CreateMessage(
                from,
                to,
                null, //cc
                null, //bcc
                subject,
                MailPriority.Normal,
                htmlTemplateFileName,
                null, //plainTemplateFileName
                null, //styleSheetFileName
                replacements,
                null, //linkedResources,
                null); //attachments
        }

        /// <summary>
        /// Creates a new instance of a MailMessage using the specified information.
        /// </summary>
        /// <param name="from">Sender's e-mail address. Could be null, in which case the default sender e-mail specified via smtp configuration will be used.</param>
        /// <param name="to">Comma-separated list of e-mail addresses to send the message to.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="htmlTemplateFileName">Html template file name.</param>
        /// <param name="plainTemplateFileName">Plain text template file name. Could be <b>null</b>.</param>
        /// <param name="replacements">Collection of replacement arguments.</param>
        /// <returns>New instance of a Mail Message.</returns>
        public static MailMessage CreateMessage(
            string from,
            string to,
            string subject,
            string htmlTemplateFileName,
            string plainTemplateFileName,
            IDictionary<string, string> replacements)
        {
            return MailUtility.CreateMessage(
                from,
                to,
                null, //cc
                null, //bcc
                subject,
                MailPriority.Normal,
                htmlTemplateFileName,
                plainTemplateFileName,
                null, //styleSheetFileName
                replacements,
                null, //linkedResources,
                null); //attachments
        }

        /// <summary>
        /// Creates a new instance of a MailMessage using the specified parameters.
        /// </summary>
        /// <param name="from">Sender's e-mail address. Could be null, in which case the default sender e-mail specified via smtp configuration will be used.</param>
        /// <param name="to">Comma-separated list of e-mail addresses to send the message to.</param>
        /// <param name="cc">Comma-separated list of e-mail addresses to send a copy (CC) of the message to.</param>
        /// <param name="bcc">Comma-separated list of e-mail addresses to send a blind copy (BCC) of the message to.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="htmlTemplateFileName">Html template file name.</param>
        /// <param name="plainTemplateFileName">Plain text template file name. Could be <b>null</b>.</param>
        /// <param name="styleSheetFileName">Name of a file containing styles to be added to the email.</param>
        /// <param name="replacements">Collection of replacement arguments.</param>
        /// <returns>New instance of a Mail Message.</returns>
        public static MailMessage CreateMessage(
            string from,
            string to,
            string cc,
            string bcc,
            string subject,
            string htmlTemplateFileName,
            string plainTemplateFileName,
            string styleSheetFileName,
            IDictionary<string, string> replacements)
        {
            return MailUtility.CreateMessage(
                from,
                to,
                cc,
                bcc,
                subject,
                MailPriority.Normal,
                htmlTemplateFileName,
                plainTemplateFileName,
                styleSheetFileName,
                replacements,
                null, //linkedResources,
                null); //attachments
        }

        #endregion CreateMessage

        #region Private Helpers

        private static string LoadMessageBody(string bodyTemplateFileName)
        {
            if (string.IsNullOrEmpty(bodyTemplateFileName) == true)
                throw new ArgumentNullException("bodyTemplateFileName");

            // load the body template
            string body = null;
            string physicalFileName = VirtualPathResolver.MapPath(bodyTemplateFileName);
            if (string.IsNullOrEmpty(physicalFileName) == false && File.Exists(physicalFileName) == true)
            {
                using (TextReader textReader = new StreamReader(physicalFileName))
                {
                    body = textReader.ReadToEnd();
                }
            }
            return body;
        }

        private static string BuildMessageBody(string body, IDictionary<string, string> replacements)
        {
            // resolve replacements
            if (replacements != null && string.IsNullOrEmpty(body) == false)
            {
                foreach (string key in replacements.Keys)
                {
                    string pattern = key;
                    string replacement = replacements[key];
                    if (replacement == null)
                        throw new ArgumentException("Invalid replacement argument");
                    replacement = replacement.Replace("$", "$$");
                    body = Regex.Replace(body, pattern, replacement, RegexOptions.IgnoreCase);
                }
            }
            return body;
        }

        #endregion Private Helpers
    }
}