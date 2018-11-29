using GT.BizTalk.Framework.SemanticLogging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility;
using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace GT.BizTalk.Framework.SemanticLogging.Sinks
{
    /// <summary>
    /// Semantic Logging sink for writing event information into the Windows Event Log.
    /// </summary>
    public class EventLogSink : IObserver<EventEntry>
    {
        #region Constants

        private const string EVENT_SOURCE = "DE.Unify";
        private const int INFO_EVENT_ID = 101;
        private const int WARN_EVENT_ID = 102;
        private const int ERROR_EVENT_ID = 103;

        #endregion Constants

        #region Fields

        private string eventSource;
        private IEventTextFormatter formatter;

        #endregion Fields

        #region Constructor

        public EventLogSink(string eventSource, IEventTextFormatter formatter)
        {
            this.eventSource = eventSource ?? EVENT_SOURCE;
            this.formatter = formatter ?? new EventLogEventTextFormatter();
        }

        #endregion Constructor

        #region Sink Implementation

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(EventEntry value)
        {
            EventLog.WriteEntry(
                this.eventSource,
                this.formatter.WriteEvent(value),
                this.ToEventLogEntryType(value.Schema.Level),
                this.ToEventID(value.Schema.Level));
        }

        #endregion Sink Implementation

        #region Private Helpers

        private EventLogEntryType ToEventLogEntryType(EventLevel level)
        {
            switch (level)
            {
                case EventLevel.Critical:
                case EventLevel.Error:
                    return EventLogEntryType.Error;

                case EventLevel.Warning:
                    return EventLogEntryType.Warning;

                default:
                    return EventLogEntryType.Information;
            }
        }

        private int ToEventID(EventLevel level)
        {
            switch (level)
            {
                case EventLevel.Critical:
                case EventLevel.Error:
                    return ERROR_EVENT_ID;

                case EventLevel.Warning:
                    return WARN_EVENT_ID;

                default:
                    return INFO_EVENT_ID;
            }
        }

        #endregion Private Helpers
    }
}