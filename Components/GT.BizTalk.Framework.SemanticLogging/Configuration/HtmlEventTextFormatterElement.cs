using GT.BizTalk.Framework.SemanticLogging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;
using System;
using System.Diagnostics.Tracing;
using System.Xml.Linq;

namespace GT.BizTalk.Framework.SemanticLogging.Configuration
{
    /// <summary>
    /// Represents a configuration element that can create an instance of <see cref="HtmlEventTextFormatter"/>.
    /// </summary>
    internal class HtmlEventTextFormatterElement : IFormatterElement
    {
        private readonly XName formatterName = XName.Get("htmlEventTextFormatter", Constants.Namespace);

        /// <summary>
        /// Determines whether this instance [can create sink] the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can create sink] the specified element; otherwise, <c>false</c>.
        /// </returns>
        public bool CanCreateFormatter(XElement element)
        {
            return this.GetFormatterElement(element) != null;
        }

        /// <summary>
        /// Creates the <see cref="IEventTextFormatter" /> instance.
        /// </summary>
        /// <param name="element">The configuration element.</param>
        /// <returns>
        /// The formatter instance.
        /// </returns>
        public IEventTextFormatter CreateFormatter(XElement element)
        {
            var formatter = this.GetFormatterElement(element);

            EventLevel verbosityThreshold = (EventLevel)Enum.Parse(typeof(EventLevel), (string)formatter.Attribute("verbosityThreshold") ?? EventTextFormatter.DefaultVerbosityThreshold.ToString());

            return new HtmlEventTextFormatter(
                verbosityThreshold,
                (string)formatter.Attribute("dateTimeFormat"));
        }

        private XElement GetFormatterElement(XElement element)
        {
            return element.Element(this.formatterName);
        }
    }
}