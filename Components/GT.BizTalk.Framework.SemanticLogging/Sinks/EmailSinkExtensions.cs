using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;
using System;

namespace GT.BizTalk.Framework.SemanticLogging.Sinks
{
    public static class EmailSinkExtensions
    {
        public static SinkSubscription<EmailSink> LogToEmail(
          this IObservable<EventEntry> eventStream, string from, string to, string subject, string templateFilePath,
            IEventTextFormatter formatter = null)
        {
            var sink = new EmailSink(from, to, subject, templateFilePath, formatter);

            var subscription = eventStream.Subscribe(sink);

            return new SinkSubscription<EmailSink>(subscription, sink);
        }
    }
}