using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace GT.BizTalk.Framework.Core.Tracing
{
    /// <summary>
    /// Custom EventSource with generic logging/tracing support.
    /// </summary>
    [EventSource(Name = "DE-Unify")]
    public partial class TraceProvider : EventSource
    {
        #region EventCodes

        public partial class EventCodes
        {
            public const int TraceIn = 1;
            public const int TraceOut = 2;
            public const int TraceStartScope = 3;
            public const int TraceEndScope = 4;
            public const int TraceInfo = 5;
            public const int TraceWarning = 6;
            public const int TraceError = 7;
        }

        #endregion EventCodes

        #region Keywords

        public partial class Keywords
        {
            public const EventKeywords Diagnostic = (EventKeywords)1;
            public const EventKeywords Perf = (EventKeywords)2;
        }

        #endregion Keywords

        #region Tasks

        public partial class Tasks
        {
            public const EventTask Initialize = (EventTask)1;
        }

        #endregion Tasks

        #region Constants

        private const string NullParameterValue = "NULL";
        private const string UnknownMethodName = "UNKNOWN";
        private const string NoReturnValue = "<void>";
        private const string FormatStringTraceIn = "=> {0}({1}) - [{2}]";
        private const string FormatStringTraceOut = "<= {0}(...) = {1} - [{2}]";
        private const string FormatStringTraceScopeStart = "-> {0}({1}) - [{2}]";
        private const string FormatStringTraceScopeEnd = "<- {0}: {1}ms - [{2}]";
        private const string FormatStringTracInfo = "{0}";
        private const string FormatStringTracWarning = "{0}";
        private const string FormatStringTraceError = "{0}";

        #endregion Constants

        #region Fields

        private readonly HighResTimer HighResTimer = new HighResTimer();

        #endregion Fields

        #region Singleton Instance

        /// <summary>
        /// Instance instance declaration using a lazy loading operation.
        /// </summary>
        private static readonly Lazy<TraceProvider> Instance =
            new Lazy<TraceProvider>(() => new TraceProvider());

        /// <summary>
        /// Private default constructor to force using the Singleton instance.
        /// </summary>
        private TraceProvider()
        {
        }

        /// <summary>
        /// Gets the reference to the singleton instance.
        /// </summary>
        public static TraceProvider Logger { get { return Instance.Value; } }

        #endregion Singleton Instance

        #region Events

        #region TraceIn/TraceOut

        /// <summary>
        /// Writes an informational event into the trace log indicating that a method is invoked. This can be useful for tracing method calls to help analyze the
        /// code execution flow. A call to the TraceIn method would typically be at the very beginning of an instrumented code.
        /// </summary>
        /// <param name="methodName">A message that will be included into the traced event (make sure you do not supply any sensitive data).</param>
        /// <param name="inputParams">The method input parameters which will be included into the traced event (make sure you do not supply any sensitive data).</param>
        /// <param name="callToken">A unique value used as a correlation token to correlate TraceIn and TraceOut calls.</param>
        [Event(EventCodes.TraceIn, Message = FormatStringTraceIn, Level = EventLevel.Verbose, Keywords = Keywords.Perf)]
        internal void TraceIn(string methodName, string inputParams, string callToken)
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.Diagnostic))
            {
                this.WriteEvent(EventCodes.TraceIn, methodName, inputParams, callToken);
            }
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating that a method is about to complete. This can be useful for tracing method calls to help analyze the
        /// code execution flow. A call to the TraceOut method would typically be at the very end of an instrumented code, before the code returns its result (if any).
        /// </summary>
        /// <param name="methodName">A message that will be included into the traced event (make sure you do not supply any sensitive data).</param>
        /// <param name="outputParams">The method parameters which will be included into the traced event (make sure you do not supply any sensitive data).</param>
        /// <param name="callToken">A unique value which is used as a correlation token to correlate TraceIn and TraceOut calls.</param>
        [Event(EventCodes.TraceOut, Message = FormatStringTraceOut, Level = EventLevel.Verbose, Keywords = Keywords.Perf)]
        internal void TraceOut(string methodName, string outputParams, string callToken)
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.Diagnostic))
            {
                this.WriteEvent(EventCodes.TraceOut, methodName, outputParams, callToken);
            }
        }

        #endregion TraceIn/TraceOut

        #region TraceStartScope/TraceEndScope

        /// <summary>
        /// Writes an informational event into the trace log indicating a start of a scope for which duration will be measured.
        /// </summary>
        /// <param name="scope">A textual identity of a scope for which duration will be traced.</param>
        /// <param name="data"Data items to be included into scope details.</param>
        /// <param name="callToken">An unique value which is used as a correlation token to correlate TraceStartScope and TraceEndScope calls.</param>
        [Event(EventCodes.TraceStartScope, Message = FormatStringTraceScopeStart, Level = EventLevel.Verbose, Keywords = Keywords.Perf)]
        internal void TraceStartScope(string scope, string data, string callToken)
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.Diagnostic))
            {
                this.WriteEvent(EventCodes.TraceStartScope, scope, data, callToken);
            }
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating the end of a scope for which duration will be measured.
        /// </summary>
        /// <param name="scope">A textual identity of a scope for which duration will be traced.</param>
        /// <param name="duration">The duration of the scope execution.</param>
        /// <param name="callToken">An unique value which is used as a correlation token to correlate TraceStartScope and TraceEndScope calls.</param>
        [Event(EventCodes.TraceEndScope, Message = FormatStringTraceScopeEnd, Level = EventLevel.Verbose, Keywords = Keywords.Perf)]
        internal void TraceEndScope(string scope, long duration, string callToken)
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.Diagnostic))
            {
                this.WriteEvent(EventCodes.TraceEndScope, scope, duration, callToken);
            }
        }

        #endregion TraceStartScope/TraceEndScope

        #region TraceInfo

        /// <summary>
        /// Writes an information message to the trace. This method is provided for optimal performance when
        /// tracing simple messages which don't require a format string.
        /// </summary>
        /// <param name="message">A string containing the message to be traced.</param>
        [Event(EventCodes.TraceInfo, Message = FormatStringTracInfo, Level = EventLevel.Informational)]
        public void TraceInfo(string message)
        {
            this.WriteEvent(EventCodes.TraceInfo, message);
        }

        #endregion TraceInfo

        #region TraceWarning

        /// <summary>
        /// Writes a warning message to the trace. This method is provided for optimal performance when
        /// tracing simple messages which don't require a format string.
        /// </summary>
        /// <param name="message">A string containing the message to be traced.</param>
        [Event(EventCodes.TraceWarning, Message = FormatStringTracWarning, Level = EventLevel.Warning, Keywords = Keywords.Diagnostic)]
        public void TraceWarning(string message)
        {
            this.WriteEvent(EventCodes.TraceWarning, message);
        }

        #endregion TraceWarning

        #region TraceError

        /// <summary>
        /// Writes an error message to the trace. This method is provided for optimal performance when
        /// tracing simple messages which don't require a format string.
        /// </summary>
        /// <param name="message">A string containing the error message to be traced.</param>
        [Event(EventCodes.TraceError, Message = FormatStringTraceError, Level = EventLevel.Error, Keywords = Keywords.Diagnostic)]
        public void TraceError(string message)
        {
            this.WriteEvent(EventCodes.TraceError, message);
        }

        #endregion TraceError

        #endregion Events

        #region Non-Event Helpers

        #region WriteEvent Overloads

        /// <summary>
        ///  Writes an event by using the provided event identifier and arguments.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="arg1">A string argument.</param>
        /// <param name="arg2">A 64 bit integer argument.</param>
        /// <param name="arg3">A string argument.</param>
        [NonEvent]
        private unsafe void WriteEvent(int eventId, string arg1, long arg2, string arg3)
        {
            if (this.IsEnabled())
            {
                if (arg1 == null)
                    arg1 = "";
                if (arg3 == null)
                    arg3 = "";
                fixed (char* chPtr1 = arg1)
                fixed (char* chPtr3 = arg3)
                {
                    EventSource.EventData* data = stackalloc EventSource.EventData[3];
                    data->DataPointer = (IntPtr)((void*)chPtr1);
                    data->Size = (arg1.Length + 1) * 2;
                    data[1].DataPointer = (IntPtr)((void*)&arg2);
                    data[1].Size = 8;
                    data[2].DataPointer = (IntPtr)((void*)chPtr3);
                    data[2].Size = (arg3.Length + 1) * 2;
                    this.WriteEventCore(eventId, 3, data);
                }
            }
        }

        /// <summary>
        ///  Writes an event by using the provided event identifier and arguments.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="arg1">A string argument.</param>
        /// <param name="arg2">A string argument.</param>
        /// <param name="arg3">A string argument.</param>
        /// <param name="arg4">A string argument.</param>
        [NonEvent]
        private unsafe void WriteEvent(int eventId, string arg1, string arg2, string arg3, string arg4)
        {
            if (this.IsEnabled())
            {
                if (arg1 == null)
                    arg1 = "";
                if (arg2 == null)
                    arg2 = "";
                if (arg3 == null)
                    arg3 = "";
                if (arg4 == null)
                    arg4 = "";
                fixed (char* chPtr1 = arg1)
                fixed (char* chPtr2 = arg2)
                fixed (char* chPtr3 = arg3)
                fixed (char* chPtr4 = arg4)
                {
                    EventSource.EventData* data = stackalloc EventSource.EventData[4];
                    data->DataPointer = (IntPtr)((void*)chPtr1);
                    data->Size = (arg1.Length + 1) * 2;
                    data[1].DataPointer = (IntPtr)((void*)chPtr2);
                    data[1].Size = (arg2.Length + 1) * 2;
                    data[2].DataPointer = (IntPtr)((void*)chPtr3);
                    data[2].Size = (arg3.Length + 1) * 2;
                    data[3].DataPointer = (IntPtr)((void*)chPtr4);
                    data[3].Size = (arg4.Length + 1) * 2;
                    this.WriteEventCore(eventId, 4, data);
                }
            }
        }

        #endregion WriteEvent Overloads

        #region TraceIn/TraceOut

        /// <summary>
        /// Writes a verbose event into the trace log indicating that a method is invoked. This can be useful for tracing method calls to help analyze the
        /// code execution flow. A call to the TraceIn method would typically be at the very beginning of an instrumented code.
        /// This method is provided to ensure optimal performance when no parameters are required to be traced.
        /// </summary>
        /// <returns>A unique value which can be used as a correlation token to correlate TraceIn and TraceOut calls.</returns>
        [NonEvent]
        public Guid TraceIn()
        {
            Guid callToken = Guid.NewGuid();
            if (this.IsEnabled(EventLevel.Verbose, Keywords.Diagnostic))
            {
                this.TraceIn(GetMethodDetails(GetCallingMethod()), null, callToken.ToString());
            }
            return callToken;
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating that a method is invoked. This can be useful for tracing method calls to help analyze the
        /// code execution flow. A call to the TraceIn method would typically be at the very beginning of an instrumented code.
        /// </summary>
        /// <param name="parameters">The method parameters which will be included into the traced event (make sure you do not supply any sensitive data).</param>
        /// <returns>A unique value which can be used as a correlation token to correlate TraceIn and TraceOut calls.</returns>
        [NonEvent]
        public Guid TraceIn(params object[] parameters)
        {
            Guid callToken = Guid.NewGuid();
            if (this.IsEnabled(EventLevel.Verbose, Keywords.Diagnostic))
            {
                this.TraceIn(GetMethodDetails(GetCallingMethod()), GetParameterList(parameters), callToken.ToString());
            }
            return callToken;
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating that a method is about to complete. This can be useful for tracing method calls to help analyze the
        /// code execution flow. A call to the TraceOut method would typically be at the very end of an instrumented code, before the code returns its result (if any).
        /// </summary>
        /// <param name="callToken">A unique value which is used as a correlation token to correlate TraceIn and TraceOut calls.</param>
        /// <param name="parameters">The method parameters which will be included into the traced event (make sure you do not supply any sensitive data).</param>
        [NonEvent]
        public void TraceOut()
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.Diagnostic))
            {
                this.TraceOut(GetMethodDetails(GetCallingMethod()), NoReturnValue, null);
            }
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating that a method is about to complete. This can be useful for tracing method calls to help analyze the
        /// code execution flow. A call to the TraceOut method would typically be at the very end of an instrumented code, before the code returns its result (if any).
        /// </summary>
        /// <param name="callToken">A unique value which is used as a correlation token to correlate TraceIn and TraceOut calls.</param>
        /// <param name="parameters">The method parameters which will be included into the traced event (make sure you do not supply any sensitive data).</param>
        [NonEvent]
        public void TraceOut(params object[] parameters)
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.Diagnostic))
            {
                string outputParams = (parameters != null && parameters.Length > 0 ? GetParameterList(parameters) : NoReturnValue);
                this.TraceOut(GetMethodDetails(GetCallingMethod()), outputParams, null);
            }
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating that a method is about to complete. This can be useful for tracing method calls to help analyze the
        /// code execution flow. A call to the TraceOut method would typically be at the very end of an instrumented code, before the code returns its result (if any).
        /// </summary>
        /// <param name="callToken">A unique value which is used as a correlation token to correlate TraceIn and TraceOut calls.</param>
        /// <param name="parameters">The method parameters which will be included into the traced event (make sure you do not supply any sensitive data).</param>
        [NonEvent]
        public void TraceOut(Guid callToken, params object[] parameters)
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.Diagnostic))
            {
                string outputParams = (parameters != null && parameters.Length > 0 ? GetParameterList(parameters) : NoReturnValue);
                this.TraceOut(GetMethodDetails(GetCallingMethod()), outputParams, callToken.ToString());
            }
        }

        #endregion TraceIn/TraceOut

        #region TraceStartScope/TraceEndScope

        /// <summary>
        /// Writes an informational event into the trace log indicating the start of a scope for which duration will be measured.
        /// This method is provided in order to ensure optimal performance when no parameters are available for tracing.
        /// </summary>
        /// <param name="scope">A textual identity of a scope for which duration will be traced.</param>
        /// <param name="parameters">A list containing zero or more data items to be included into scope details.</param>
        /// <returns>The number of ticks that represent the date and time when it was invoked. This date/time will be used later when tracing the end of the scope.</returns>
        [NonEvent]
        public long TraceStartScope(string scope)
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.Diagnostic))
            {
                this.TraceStartScope(scope, null, null);
            }
            return this.HighResTimer.TickCount;
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating a start of a scope for which duration will be measured.
        /// </summary>
        /// <param name="scope">A textual identity of a scope for which duration will be traced.</param>
        /// <param name="parameters">A list containing zero or more data items to be included into scope details.</param>
        /// <returns>The number of ticks that represent the date and time when it was invoked. This date/time will be used later when tracing the end of the scope.</returns>
        [NonEvent]
        public long TraceStartScope(string scope, params object[] parameters)
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.Diagnostic))
            {
                this.TraceStartScope(scope, GetParameterList(parameters), null);
            }
            return this.HighResTimer.TickCount;
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating the start of a scope for which duration will be measured.
        /// This method is provided in order to ensure optimal performance when only 1 parameter of type Guid is available for tracing.
        /// </summary>
        /// <param name="scope">A textual identity of a scope for which duration will be traced.</param>
        /// <param name="callToken">An unique value which is used as a correlation token to correlate TraceStartScope and TraceEndScope calls.</param>
        /// <returns>The number of ticks that represent the date and time when it was invoked. This date/time will be used later when tracing the end of the scope.</returns>
        [NonEvent]
        public long TraceStartScope(string scope, Guid callToken)
        {
            if (this.IsEnabled(EventLevel.Verbose, Keywords.Diagnostic))
            {
                this.TraceStartScope(scope, null, callToken.ToString());
            }
            return this.HighResTimer.TickCount;
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating the end of a scope for which duration will be measured.
        /// </summary>
        /// <param name="scope">A textual identity of a scope for which duration will be traced.</param>
        /// <param name="startTicks">The number of ticks that represent the date and time when the code entered the scope.</param>
        /// <returns>The calculated duration.</returns>
        [NonEvent]
        public long TraceEndScope(string scope, long startTicks)
        {
            long duration = 0;
            if (this.IsEnabled(EventLevel.Verbose, Keywords.Diagnostic))
            {
                duration = this.HighResTimer.GetElapsedMilliseconds(startTicks);
                this.TraceEndScope(scope, duration, null);
            }
            return duration;
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating the end of a scope for which duration will be measured.
        /// </summary>
        /// <param name="scope">A textual identity of a scope for which duration will be traced.</param>
        /// <param name="startTicks">The number of ticks that represent the date and time when the code entered the scope.</param>
        /// <param name="callToken">An unique value which is used as a correlation token to correlate TraceStartScope and TraceEndScope calls.</param>
        /// <returns>The calculated duration.</returns>
        [NonEvent]
        public long TraceEndScope(string scope, long startTicks, Guid callToken)
        {
            long duration = 0;
            if (this.IsEnabled(EventLevel.Verbose, Keywords.Diagnostic))
            {
                duration = this.HighResTimer.GetElapsedMilliseconds(startTicks);
                this.TraceEndScope(scope, duration, callToken.ToString());
            }
            return duration;
        }

        #endregion TraceStartScope/TraceEndScope

        #region TraceInfo

        /// <summary>
        /// Writes an information message to the trace.
        /// </summary>
        /// <param name="format">A string containing zero or more format items.</param>
        /// <param name="parameters">A list containing zero or more data items to format.</param>
        [NonEvent]
        public void TraceInfo(string format, params object[] parameters)
        {
            if (this.IsEnabled())
            {
                string message = string.Format(format, parameters);
                this.TraceInfo(message);
            }
        }

        /// <summary>
        /// Writes an information message to the trace. This method is intended to be used when the data that needs to be
        /// written to the trace is expensive to be fetched. The method represented by the Func(T) delegate will only be invoked if
        /// tracing is enabled.
        /// </summary>
        /// <param name="expensiveDataProvider">A method that has no parameters and returns a value that needs to be traced.</param>
        [NonEvent]
        public void TraceInfo(Func<string> expensiveDataProvider)
        {
            if (this.IsEnabled())
            {
                this.TraceInfo(expensiveDataProvider());
            }
        }

        /// <summary>
        /// Writes the contents of the specified IEnumerable collection to the trace.
        /// </summary>
        /// <param name="list">Instance of the IEnumerable collection.</param>
        [NonEvent]
        public void TraceInfo(IEnumerable list)
        {
            this.TraceInfo(() => { return ListToString(list); });
        }

        #endregion TraceInfo

        #region TraceWarning

        /// <summary>
        /// Writes a warning message to the trace.
        /// </summary>
        /// <param name="format">A string containing zero or more format items.</param>
        /// <param name="parameters">A list containing zero or more data items to format.</param>
        [NonEvent]
        public void TraceWarning(string format, params object[] parameters)
        {
            if (this.IsEnabled())
            {
                string message = string.Format(format, parameters);
                this.TraceWarning(message);
            }
        }

        #endregion TraceWarning

        #region TraceError

        /// <summary>
        /// Writes an error message to the trace.
        /// </summary>
        /// <param name="format">A string containing zero or more format items.</param>
        /// <param name="parameters">A list containing zero or more data items to format.</param>
        [NonEvent]
        public void TraceError(string format, params object[] parameters)
        {
            if (this.IsEnabled())
            {
                string message = string.Format(format, parameters);
                this.TraceError(message);
            }
        }

        /// <summary>
        /// Writes an error message to the trace.
        /// </summary>
        /// <param name="callToken">An unique value which is used as a correlation token to correlate TraceIn and TraceError calls.</param>
        /// <param name="format">A string containing zero or more format items.</param>
        /// <param name="parameters">A list containing zero or more data items to format.</param>
        [NonEvent]
        public void TraceError(Guid callToken, string format, params object[] parameters)
        {
            if (this.IsEnabled())
            {
                string message = string.Format(format, parameters);
                message = string.Format("{0} - [{1}]", message, callToken);
                this.TraceError(message);
            }
        }

        /// <summary>
        /// Writes the exception details to the trace.
        /// </summary>
        /// <param name="ex">An exception to be formatted and written to the trace.</param>
        [NonEvent]
        public void TraceError(Exception ex)
        {
            this.TraceError(ex, true);
        }

        /// <summary>
        /// Writes the exception details to the trace.
        /// </summary>
        /// <param name="ex">An exception to be formatted and written to the trace.</param>
        /// <param name="includeStackTrace">A flag indicating whether or not call stack details should be included.</param>
        [NonEvent]
        public void TraceError(Exception ex, bool includeStackTrace)
        {
            if (this.IsEnabled())
            {
                ExceptionFormatter exceptionFormatter = new ExceptionFormatter();
                string message = exceptionFormatter.FormatException(ex, includeStackTrace);
                this.TraceError(message);
            }
        }

        /// <summary>
        /// Writes the exception details to the trace.
        /// </summary>
        /// <param name="ex">An exception to be formatted and written to the trace.</param>
        /// <param name="includeStackTrace">A flag indicating whether or not call stack details should be included.</param>
        /// <param name="callToken">An unique value which is used as a correlation token to correlate TraceIn and TraceError calls.</param>
        [NonEvent]
        public void TraceError(Exception ex, bool includeStackTrace, Guid callToken)
        {
            if (this.IsEnabled())
            {
                ExceptionFormatter exceptionFormatter = new ExceptionFormatter();
                string message = exceptionFormatter.FormatException(ex, includeStackTrace);
                message = string.Format("{0} - [{1}]", message, callToken);
                this.TraceError(message);
            }
        }

        #endregion TraceError

        #endregion Non-Event Helpers

        #region Private Helpers

        /// <summary>
        /// Returns a string representing a list of parameters to be written into trace log.
        /// </summary>
        /// <param name="parameters">Parameters to be included in the trace log entry.</param>
        /// <returns>A comma-separated list of formatted parameters.</returns>
        private static string GetParameterList(object[] parameters)
        {
            // Make sure we have a parameter array which is safe to pass to Array.ConvertAll.
            if (null == parameters)
            {
                parameters = new object[] { null };
            }

            // Get a string representation of each parameter that we have been passed.
            string[] paramStrings = Array.ConvertAll<object, string>(parameters, ParameterObjectToString);

            // Create a string containing details of each parameter.
            return String.Join(", ", paramStrings);
        }

        private static string ParameterObjectToString(object parameter)
        {
            if (null == parameter)
            {
                return NullParameterValue;
            }

            // Surround string values with quotes.
            if (parameter.GetType() == typeof(string))
            {
                return "\"" + (string)parameter + "\"";
            }

            return parameter.ToString();
        }

        private static string GetMethodDetails(MethodBase callingMethod)
        {
            // Compose and return fully qualified method name.
            return callingMethod != null ? String.Format(CultureInfo.InvariantCulture, "{0}.{1}", callingMethod.DeclaringType.FullName, callingMethod.Name) : UnknownMethodName;
        }

        /// <summary>
        /// Determines if the specified method is part of the tracing library.
        /// </summary>
        /// <param name="methodToCheck">MethodBase describing the method to check.</param>
        /// <returns>True if the method is a member of the tracing library, false if not.</returns>
        private static bool IsTracingMethod(MethodBase methodToCheck)
        {
            return methodToCheck.DeclaringType.IsSubclassOf(typeof(EventSource));
        }

        /// <summary>
        /// Walks the call stack to find the name of the method which invoked this class.
        /// </summary>
        /// <returns>MethodBase representing the most recent method in the stack which is not a member of this class.</returns>
        private static MethodBase GetCallingMethod()
        {
            StackTrace stack = new StackTrace();

            foreach (StackFrame frame in stack.GetFrames())
            {
                MethodBase frameMethod = frame.GetMethod();
                if (!IsTracingMethod(frameMethod))
                {
                    return frameMethod;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a string representing a list of items from the IEnumerable collection to be written into trace log.
        /// </summary>
        /// <param name="list">IEnumerable collection to be included in the trace log entry.</param>
        /// <returns>Formatted string.</returns>
        private static string ListToString(IEnumerable list)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Writing contents of {0}", list.GetType().Name));
            foreach (object item in list)
            {
                sb.AppendLine(item.ToString());
            }
            return sb.ToString();
        }

        #endregion Private Helpers
    }
}