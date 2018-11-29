using GT.BizTalk.Framework.Core.Tracing;
using System;
using System.Diagnostics;

namespace GT.BizTalk.Framework.Core.Diagnostics
{
    /// <summary>
    /// Static helper for System.Diagnostics.EventLog to perform logging of standard messages into the Windows Event Log.
    /// </summary>
    public static class EventLogHelper
    {
        #region Constants

        private const string EXECPTION_FORMAT = "{0}\n\n{1}";
        private const string EVENT_SOURCE = "GT.BizTalk.Framework";
        private const int INFO_EVENT_ID = 101;
        private const int WARN_EVENT_ID = 102;
        private const int ERROR_EVENT_ID = 103;

        #endregion Constants

        #region Information

        /// <summary>
        /// Writes an information message to the log. This method is provided for optimal performance when
        /// tracing simple messages which don't require a format string.
        /// </summary>
        /// <param name="message">A string containing the message to be logged.</param>
        public static void LogInfo(string message)
        {
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Information, INFO_EVENT_ID);
        }

        /// <summary>
        /// Writes an information message to the trace.
        /// </summary>
        /// <param name="format">A string containing zero or more format items.</param>
        /// <param name="parameters">A list containing zero or more data items to format.</param>
        public static void LogInfo(string format, params object[] parameters)
        {
            string message = string.Format(format, parameters);
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Information, INFO_EVENT_ID);
        }

        /// <summary>
        /// Writes an information message to the log including the exception information.
        /// </summary>
        /// <param name="ex">Exception to be logged.</param>
        public static void LogInfo(Exception ex)
        {
            ExceptionFormatter exceptionFormatter = new ExceptionFormatter();
            string message = exceptionFormatter.FormatException(ex);
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Information, INFO_EVENT_ID);
        }

        /// <summary>
        /// Writes an information message to the log including the exception information.
        /// </summary>
        /// <param name="ex">Exception to be logged.</param>
        /// <param name="includeStackTrace">A flag indicating whether or not call stack details should be included.</param>
        public static void LogInfo(Exception ex, bool includeStackTrace)
        {
            ExceptionFormatter exceptionFormatter = new ExceptionFormatter();
            string message = exceptionFormatter.FormatException(ex, includeStackTrace);
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Information, INFO_EVENT_ID);
        }

        /// <summary>
        /// Writes an information message to the log including the exception information.
        /// </summary>
        /// <param name="message">A string containing the message to be logged.</param>
        /// <param name="ex">Exception to be logged.</param>
        public static void LogInfo(string message, Exception ex)
        {
            ExceptionFormatter exceptionFormatter = new ExceptionFormatter();
            message = string.Format(EXECPTION_FORMAT, message, exceptionFormatter.FormatException(ex));
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Information, INFO_EVENT_ID);
        }

        /// <summary>
        /// Writes an information message to the log including the exception information.
        /// </summary>
        /// <param name="message">A string containing the message to be logged.</param>
        /// <param name="ex">Exception to be logged.</param>
        /// <param name="includeStackTrace">A flag indicating whether or not call stack details should be included.</param>
        public static void LogInfo(string message, Exception ex, bool includeStackTrace)
        {
            ExceptionFormatter exceptionFormatter = new ExceptionFormatter();
            message = string.Format(EXECPTION_FORMAT, message, exceptionFormatter.FormatException(ex, includeStackTrace));
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Information, INFO_EVENT_ID);
        }

        #endregion Information

        #region Warning

        /// <summary>
        /// Writes a warning message to the log. This method is provided for optimal performance when
        /// tracing simple messages which don't require a format string.
        /// </summary>
        /// <param name="message">A string containing the message to be logged.</param>
        public static void LogWarning(string message)
        {
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Warning, WARN_EVENT_ID);
        }

        /// <summary>
        /// Writes a warning message to the trace.
        /// </summary>
        /// <param name="format">A string containing zero or more format items.</param>
        /// <param name="parameters">A list containing zero or more data items to format.</param>
        public static void LogWarning(string format, params object[] parameters)
        {
            string message = string.Format(format, parameters);
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Warning, WARN_EVENT_ID);
        }

        /// <summary>
        /// Writes a warning message to the log including the exception information.
        /// </summary>
        /// <param name="ex">Exception to be logged.</param>
        public static void LogWarning(Exception ex)
        {
            ExceptionFormatter exceptionFormatter = new ExceptionFormatter();
            string message = exceptionFormatter.FormatException(ex);
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Warning, WARN_EVENT_ID);
        }

        /// <summary>
        /// Writes a warning message to the log including the exception information.
        /// </summary>
        /// <param name="ex">Exception to be logged.</param>
        /// <param name="includeStackTrace">A flag indicating whether or not call stack details should be included.</param>
        public static void LogWarning(Exception ex, bool includeStackTrace)
        {
            ExceptionFormatter exceptionFormatter = new ExceptionFormatter();
            string message = exceptionFormatter.FormatException(ex, includeStackTrace);
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Warning, WARN_EVENT_ID);
        }

        /// <summary>
        /// Writes a warning message to the log including the exception information.
        /// </summary>
        /// <param name="message">A string containing the message to be logged.</param>
        /// <param name="ex">Exception to be logged.</param>
        public static void LogWarning(string message, Exception ex)
        {
            ExceptionFormatter exceptionFormatter = new ExceptionFormatter();
            message = string.Format(EXECPTION_FORMAT, message, exceptionFormatter.FormatException(ex));
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Warning, WARN_EVENT_ID);
        }

        /// <summary>
        /// Writes a warning message to the log including the exception information.
        /// </summary>
        /// <param name="message">A string containing the message to be logged.</param>
        /// <param name="ex">Exception to be logged.</param>
        /// <param name="includeStackTrace">A flag indicating whether or not call stack details should be included.</param>
        public static void LogWarning(string message, Exception ex, bool includeStackTrace)
        {
            ExceptionFormatter exceptionFormatter = new ExceptionFormatter();
            message = string.Format(EXECPTION_FORMAT, message, exceptionFormatter.FormatException(ex, includeStackTrace));
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Warning, WARN_EVENT_ID);
        }

        #endregion Warning

        #region Error

        /// <summary>
        /// Writes an error message to the log. This method is provided for optimal performance when
        /// tracing simple messages which don't require a format string.
        /// </summary>
        /// <param name="message">A string containing the message to be logged.</param>
        public static void LogError(string message)
        {
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Error, ERROR_EVENT_ID);
        }

        /// <summary>
        /// Writes an error message to the trace.
        /// </summary>
        /// <param name="format">A string containing zero or more format items.</param>
        /// <param name="parameters">A list containing zero or more data items to format.</param>
        public static void LogError(string format, params object[] parameters)
        {
            string message = string.Format(format, parameters);
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Error, ERROR_EVENT_ID);
        }

        /// <summary>
        /// Writes an error message to the log including the exception information.
        /// </summary>
        /// <param name="ex">Exception to be logged.</param>
        public static void LogError(Exception ex)
        {
            ExceptionFormatter exceptionFormatter = new ExceptionFormatter();
            string message = exceptionFormatter.FormatException(ex);
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Error, ERROR_EVENT_ID);
        }

        /// <summary>
        /// Writes an error message to the log including the exception information.
        /// </summary>
        /// <param name="ex">Exception to be logged.</param>
        /// <param name="includeStackTrace">A flag indicating whether or not call stack details should be included.</param>
        public static void LogError(Exception ex, bool includeStackTrace)
        {
            ExceptionFormatter exceptionFormatter = new ExceptionFormatter();
            string message = exceptionFormatter.FormatException(ex, includeStackTrace);
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Error, ERROR_EVENT_ID);
        }

        /// <summary>
        /// Writes an error message to the log including the exception information.
        /// </summary>
        /// <param name="message">A string containing the message to be logged.</param>
        /// <param name="ex">Exception to be logged.</param>
        public static void LogError(string message, Exception ex)
        {
            ExceptionFormatter exceptionFormatter = new ExceptionFormatter();
            message = string.Format(EXECPTION_FORMAT, message, exceptionFormatter.FormatException(ex));
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Error, ERROR_EVENT_ID);
        }

        /// <summary>
        /// Writes an error message to the log including the exception information.
        /// </summary>
        /// <param name="message">A string containing the message to be logged.</param>
        /// <param name="ex">Exception to be logged.</param>
        /// <param name="includeStackTrace">A flag indicating whether or not call stack details should be included.</param>
        public static void LogError(string message, Exception ex, bool includeStackTrace)
        {
            ExceptionFormatter exceptionFormatter = new ExceptionFormatter();
            message = string.Format(EXECPTION_FORMAT, message, exceptionFormatter.FormatException(ex, includeStackTrace));
            EventLog.WriteEntry(EVENT_SOURCE, message, EventLogEntryType.Error, ERROR_EVENT_ID);
        }

        #endregion Error
    }
}