using GT.BizTalk.Framework.SemanticLogging.Sinks;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Configuration;
using System;
using System.Xml.Linq;

namespace GT.BizTalk.Framework.SemanticLogging.Configuration
{
    /// <summary>
    /// Represents a configuration element that can create an instance of an EventLog sink.
    /// </summary>
    public class EventLogSinkElement : ISinkElement
    {
        private readonly XName sinkName = XName.Get("eventLogSink", Constants.Namespace);

        /// <summary>
        /// Determines whether this instance [can create sink] for the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can create sink] for the specified element; otherwise, <c>false</c>.
        /// </returns>
        public bool CanCreateSink(XElement element)
        {
            return element.Name == this.sinkName;
        }

        /// <summary>
        /// Creates the <see cref="IObserver<EventEntry>" /> instance.
        /// </summary>
        /// <param name="element">The configuration element.</param>
        /// <returns>
        /// The formatter instance.
        /// </returns>
        public IObserver<EventEntry> CreateSink(XElement element)
        {
            string eventSource = (string)element.Attribute("eventSource");

            var formatter = FormatterElementFactory.Get(element);

            return new EventLogSink(eventSource, formatter);
        }
    }
}