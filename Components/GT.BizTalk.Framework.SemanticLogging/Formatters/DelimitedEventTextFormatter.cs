using GT.BizTalk.Framework.SemanticLogging.Utility;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;
using System;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.IO;
using System.Text;

namespace GT.BizTalk.Framework.SemanticLogging.Formatters
{
    /// <summary>
    /// A <see cref="IEventTextFormatter"/> implementation that writes out delimited formatted text suitable for EventLog logging.
    /// </summary>
    /// <remarks>This class is not thread-safe.</remarks>
    public class DelimitedEventTextFormatter : IEventTextFormatter
    {
        #region Constants

        /// <summary>
        /// The default delimiter.
        /// </summary>
        public const string DefaultDelimiter = "\t";

        /// <summary>
        /// The default <see cref="VerbosityThreshold"/>.
        /// </summary>
        public const EventLevel DefaultVerbosityThreshold = EventLevel.Error;

        #endregion Constants

        #region Fields

        private string delimiter;
        private string dateTimeFormat;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Instance constructor.
        /// </summary>
        /// <param name="delimiter">Delimiter used to separate the trace line tokens.</param>
        /// <param name="verbosityThreshold">The lowest <see cref="System.Diagnostics.Tracing.EventLevel" /> value where the formatted output provides all the event entry information.</param>
        /// <param name="dateTimeFormat">The date time format used for timestamp value.</param>
        public DelimitedEventTextFormatter(string delimiter = DefaultDelimiter, EventLevel verbosityThreshold = DefaultVerbosityThreshold, string dateTimeFormat = null)
        {
            this.Delimiter = delimiter;
            this.VerbosityThreshold = verbosityThreshold;
            this.DateTimeFormat = dateTimeFormat;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets the delimiter used to separate the trace line tokens.
        /// </summary>
        public string Delimiter
        {
            get
            {
                return this.delimiter;
            }
            set
            {
                this.delimiter = (string.IsNullOrEmpty(value) == false ? value : DefaultDelimiter);
            }
        }

        /// <summary>
        /// Gets or sets the lowest <see cref="System.Diagnostics.Tracing.EventLevel" /> value where the formatted output provides all the event entry information.
        /// Otherwise a summarized content of the event entry will be written.
        /// </summary>
        /// <value>The EventLevel.</value>
        public EventLevel VerbosityThreshold { get; set; }

        /// <summary>
        /// Gets or sets the date time format used for timestamp value.
        /// </summary>
        /// <value>The date time format value.</value>
        public string DateTimeFormat
        {
            get
            {
                return this.dateTimeFormat;
            }
            set
            {
                Guard.ValidDateTimeFormat(value, "DateTimeFormat");
                this.dateTimeFormat = value;
            }
        }

        #endregion Properties

        #region IEventTextFormatter Implementation

        /// <summary>
        /// Writes a formatted event entry into the trace log.
        /// </summary>
        /// <param name="eventEntry">Event entry.</param>
        /// <param name="writer">TextWriter instance.</param>
        public void WriteEvent(EventEntry eventEntry, TextWriter writer)
        {
            if (eventEntry.Schema.Level <= this.VerbosityThreshold || this.VerbosityThreshold == EventLevel.LogAlways)
            {
                // write with verbosityThreshold format
                writer.WriteLine(
                    "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}",
                    this.Delimiter,
                    eventEntry.GetFormattedTimestamp(this.DateTimeFormat),
                    eventEntry.ProviderId,
                    eventEntry.EventId,
                    eventEntry.Schema.Level,
                    eventEntry.Schema.Keywords,
                    eventEntry.Schema.Opcode,
                    eventEntry.Schema.Task,
                    eventEntry.Schema.EventName,
                    FormatDetails(eventEntry));
            }
            else
            {
                // write with brief format
                writer.WriteLine(
                    "{1}{0}{2}{0}{3}{0}{4}{0}{5}",
                    this.Delimiter,
                    eventEntry.GetFormattedTimestamp(this.DateTimeFormat),
                    eventEntry.EventId,
                    eventEntry.Schema.Level,
                    eventEntry.Schema.EventName,
                    FormatDetails(eventEntry));
            }
        }

        #endregion IEventTextFormatter Implementation

        #region Private Helpers

        private static string FormatDetails(EventEntry entry)
        {
            if (string.IsNullOrEmpty(entry.FormattedMessage) == false)
            {
                return entry.FormattedMessage;
            }
            else
            {
                return FormatPayload(entry);
            }
        }

        private static string FormatPayload(EventEntry entry)
        {
            var eventSchema = entry.Schema;
            var sb = new StringBuilder();
            sb.Append("[ ");

            for (int i = 0; i < entry.Payload.Count; i++)
            {
                try
                {
                    sb.AppendFormat("[{0} : {1}] ", eventSchema.Payload[i], entry.Payload[i]);
                }
                catch (Exception e)
                {
                    SemanticLoggingEventSource.Log.CustomFormatterUnhandledFault(e.ToString());
                    sb.AppendFormat("[{0} : {1}] ", "Exception", string.Format(CultureInfo.CurrentCulture, Properties.Resources.TextSerializationError, e.Message));
                }
            }

            sb.Append("]");
            return sb.ToString();
        }

        #endregion Private Helpers
    }
}