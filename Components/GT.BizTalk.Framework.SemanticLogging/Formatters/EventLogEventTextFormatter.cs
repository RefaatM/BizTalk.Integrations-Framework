using GT.BizTalk.Framework.SemanticLogging.Utility;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;
using System;
using System.Diagnostics.Tracing;
using System.IO;

namespace GT.BizTalk.Framework.SemanticLogging.Formatters
{
    /// <summary>
    /// A <see cref="IEventTextFormatter"/> implementation that writes out formatted text suitable for EventLog logging.
    /// </summary>
    /// <remarks>This class is not thread-safe.</remarks>
    public class EventLogEventTextFormatter : IEventTextFormatter
    {
        #region Constants

        /// <summary>
        /// The default <see cref="VerbosityThreshold"/>.
        /// </summary>
        public const EventLevel DefaultVerbosityThreshold = EventLevel.Error;

        /// <summary>
        /// The formatting string.
        /// </summary>
        private const string FormatString = "{0}: {1}";

        #endregion Constants

        #region Fields

        /// <summary>
        /// The datetime format.
        /// </summary>
        private string dateTimeFormat;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogEventTextFormatter" /> class.
        /// </summary>
        /// <param name="verbosityThreshold">The verbosity threshold.</param>
        /// <param name="dateTimeFormat">The date time format used for timestamp value.</param>
        public EventLogEventTextFormatter(EventLevel verbosityThreshold = DefaultVerbosityThreshold, string dateTimeFormat = null)
        {
            this.VerbosityThreshold = verbosityThreshold;
            this.DateTimeFormat = dateTimeFormat;
        }

        #endregion Constructor

        #region Properties

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
        /// Writes the event.
        /// </summary>
        /// <param name="eventEntry">The <see cref="EventEntry" /> instance containing the event data.</param>
        /// <param name="writer">The writer.</param>
        public void WriteEvent(EventEntry eventEntry, TextWriter writer)
        {
            Guard.ArgumentNotNull(eventEntry, "eventEntry");
            Guard.ArgumentNotNull(writer, "writer");

            if (eventEntry.Schema.Level <= this.VerbosityThreshold || this.VerbosityThreshold == EventLevel.LogAlways)
            {
                // Write with verbosityThreshold format
                writer.WriteLine(FormatString, PropertyNames.ProviderId, eventEntry.ProviderId);
                writer.WriteLine(FormatString, PropertyNames.EventId, eventEntry.EventId);
                writer.WriteLine(FormatString, PropertyNames.Keywords, eventEntry.Schema.Keywords);
                writer.WriteLine(FormatString, PropertyNames.Level, eventEntry.Schema.Level);
                writer.WriteLine(FormatString, PropertyNames.Opcode, eventEntry.Schema.Opcode);
                writer.WriteLine(FormatString, PropertyNames.Task, eventEntry.Schema.Task);
                writer.WriteLine(FormatString, PropertyNames.Timestamp, eventEntry.GetFormattedTimestamp(this.DateTimeFormat));
                writer.WriteLine(FormatString, PropertyNames.ProcessId, eventEntry.ProcessId);
                writer.WriteLine(FormatString, PropertyNames.ThreadId, eventEntry.ThreadId);

                if (eventEntry.ActivityId != Guid.Empty)
                {
                    writer.WriteLine(FormatString, PropertyNames.ActivityId, eventEntry.ActivityId);
                }

                if (eventEntry.RelatedActivityId != Guid.Empty)
                {
                    writer.WriteLine(FormatString, PropertyNames.RelatedActivityId, eventEntry.RelatedActivityId);
                }

                writer.WriteLine();
                writer.WriteLine(FormatString, PropertyNames.Message, eventEntry.FormattedMessage);
            }
            else
            {
                // Write with summary format
                writer.WriteLine(FormatString, PropertyNames.EventId, eventEntry.EventId);
                writer.WriteLine(FormatString, PropertyNames.Level, eventEntry.Schema.Level);
                writer.WriteLine(FormatString, PropertyNames.Timestamp, eventEntry.GetFormattedTimestamp(this.DateTimeFormat));
                writer.WriteLine(FormatString, PropertyNames.ProcessId, eventEntry.ProcessId);
                writer.WriteLine(FormatString, PropertyNames.ThreadId, eventEntry.ThreadId);

                if (eventEntry.ActivityId != Guid.Empty)
                {
                    writer.WriteLine(FormatString, PropertyNames.ActivityId, eventEntry.ActivityId);
                }

                if (eventEntry.RelatedActivityId != Guid.Empty)
                {
                    writer.WriteLine(FormatString, PropertyNames.RelatedActivityId, eventEntry.RelatedActivityId);
                }

                writer.WriteLine();
                writer.WriteLine(FormatString, PropertyNames.Message, eventEntry.FormattedMessage);
            }

            writer.WriteLine();
        }

        #endregion IEventTextFormatter Implementation
    }
}