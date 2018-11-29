using GT.BizTalk.Framework.Core.Mail;
using GT.BizTalk.Framework.SemanticLogging.Formatters;
using GT.BizTalk.Framework.SemanticLogging.Utility;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace GT.BizTalk.Framework.SemanticLogging.Sinks
{
    /// <summary>
    /// Semantic Logging sink for sending event information via email notifications.
    /// </summary>
    public sealed class EmailSink : IObserver<EventEntry>
    {
        #region Fields

        private const string DefaultSubject = "Email Sink Extension";
        private IEventTextFormatter formatter;
        private string from;
        private string to;
        private string subject;
        private string templateFilePath;

        #endregion Fields

        #region Constructor

        public EmailSink(string from, string to, string subject, string templateFilePath, IEventTextFormatter formatter)
        {
            Guard.ArgumentNotNullOrEmpty(from, "from");
            Guard.ArgumentNotNullOrEmpty(to, "to");
            Guard.ArgumentNotNullOrEmpty(templateFilePath, "templateFilePath");

            this.formatter = formatter ?? new HtmlEventTextFormatter();
            this.from = from;
            this.to = to;
            this.subject = subject ?? DefaultSubject;
            this.templateFilePath = templateFilePath;
        }

        #endregion Constructor

        #region Sink Implementation

        public void OnNext(EventEntry entry)
        {
            if (entry != null)
            {
                using (var writer = new StringWriter())
                {
                    this.formatter.WriteEvent(entry, writer);
                    Post(writer.ToString());
                }
            }
        }

        private async void Post(string body)
        {
            Dictionary<string, string> replacements = new Dictionary<string, string>();
            replacements.Add("<%body%>", body);

            using (SmtpClient client = new SmtpClient())
            {
                using (var message = MailUtility.CreateMessage(
                    this.from,
                    this.to,
                    this.subject,
                    this.templateFilePath,
                    replacements))
                {
                    try
                    {
                        await client.SendMailAsync(message).ConfigureAwait(false);
                    }
                    catch (SmtpException e)
                    {
                        SemanticLoggingEventSource.Log.CustomSinkUnhandledFault("SMTP error sending email: " + e.Message);
                    }
                    catch (InvalidOperationException e)
                    {
                        SemanticLoggingEventSource.Log.CustomSinkUnhandledFault("Configuration error sending email: " + e.Message);
                    }
                }
            }
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        #endregion Sink Implementation
    }
}