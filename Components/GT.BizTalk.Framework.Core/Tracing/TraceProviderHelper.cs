using System;

namespace GT.BizTalk.Framework.Core.Tracing
{
    /// <summary>
    /// Trace provider static helper that allows to access the TraceProvider.Logger singleton instance
    /// via static methods. Could be used from the Business Rule Engine or a BizTalk Map.
    /// </summary>
    public static class TraceProviderHelper
    {
        #region TraceIn/TraceOut

        /// <summary>
        /// Writes a verbose event into the trace log indicating that a method is invoked. This can be useful for tracing method calls to help analyze the
        /// code execution flow. A call to the TraceIn method would typically be at the very beginning of an instrumented code.
        /// This method is provided to ensure optimal performance when no parameters are required to be traced.
        /// </summary>
        /// <returns>A unique value which can be used as a correlation token to correlate TraceIn and TraceOut calls.</returns>
        public static Guid TraceIn()
        {
            return TraceProvider.Logger.TraceIn();
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating that a method is invoked. This can be useful for tracing method calls to help analyze the
        /// code execution flow. A call to the TraceIn method would typically be at the very beginning of an instrumented code.
        /// </summary>
        /// <param name="parameters">The method parameters which will be included into the traced event (make sure you do not supply any sensitive data).</param>
        /// <returns>A unique value which can be used as a correlation token to correlate TraceIn and TraceOut calls.</returns>
        public static Guid TraceIn(params object[] parameters)
        {
            return TraceProvider.Logger.TraceIn(parameters);
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating that a method is about to complete. This can be useful for tracing method calls to help analyze the
        /// code execution flow. A call to the TraceOut method would typically be at the very end of an instrumented code, before the code returns its result (if any).
        /// </summary>
        /// <param name="callToken">A unique value which is used as a correlation token to correlate TraceIn and TraceOut calls.</param>
        /// <param name="parameters">The method parameters which will be included into the traced event (make sure you do not supply any sensitive data).</param>
        public static void TraceOut()
        {
            TraceProvider.Logger.TraceOut();
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating that a method is about to complete. This can be useful for tracing method calls to help analyze the
        /// code execution flow. A call to the TraceOut method would typically be at the very end of an instrumented code, before the code returns its result (if any).
        /// </summary>
        /// <param name="callToken">A unique value which is used as a correlation token to correlate TraceIn and TraceOut calls.</param>
        /// <param name="parameters">The method parameters which will be included into the traced event (make sure you do not supply any sensitive data).</param>
        public static void TraceOut(params object[] parameters)
        {
            TraceProvider.Logger.TraceOut(parameters);
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating that a method is about to complete. This can be useful for tracing method calls to help analyze the
        /// code execution flow. A call to the TraceOut method would typically be at the very end of an instrumented code, before the code returns its result (if any).
        /// </summary>
        /// <param name="callToken">A unique value which is used as a correlation token to correlate TraceIn and TraceOut calls.</param>
        /// <param name="parameters">The method parameters which will be included into the traced event (make sure you do not supply any sensitive data).</param>
        public static void TraceOut(Guid callToken, params object[] parameters)
        {
            TraceProvider.Logger.TraceOut(callToken, parameters);
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
        public static long TraceStartScope(string scope)
        {
            return TraceProvider.Logger.TraceStartScope(scope);
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating a start of a scope for which duration will be measured.
        /// </summary>
        /// <param name="scope">A textual identity of a scope for which duration will be traced.</param>
        /// <param name="parameters">A list containing zero or more data items to be included into scope details.</param>
        /// <returns>The number of ticks that represent the date and time when it was invoked. This date/time will be used later when tracing the end of the scope.</returns>
        public static long TraceStartScope(string scope, params object[] parameters)
        {
            return TraceProvider.Logger.TraceStartScope(scope, parameters);
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating the start of a scope for which duration will be measured.
        /// This method is provided in order to ensure optimal performance when only 1 parameter of type Guid is available for tracing.
        /// </summary>
        /// <param name="scope">A textual identity of a scope for which duration will be traced.</param>
        /// <param name="callToken">An unique value which is used as a correlation token to correlate TraceStartScope and TraceEndScope calls.</param>
        /// <returns>The number of ticks that represent the date and time when it was invoked. This date/time will be used later when tracing the end of the scope.</returns>
        public static long TraceStartScope(string scope, Guid callToken)
        {
            return TraceProvider.Logger.TraceStartScope(scope, callToken);
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating the end of a scope for which duration will be measured.
        /// </summary>
        /// <param name="scope">A textual identity of a scope for which duration will be traced.</param>
        /// <param name="startTicks">The number of ticks that represent the date and time when the code entered the scope.</param>
        /// <returns>The calculated duration.</returns>
        public static long TraceEndScope(string scope, long startTicks)
        {
            return TraceProvider.Logger.TraceEndScope(scope, startTicks);
        }

        /// <summary>
        /// Writes an informational event into the trace log indicating the end of a scope for which duration will be measured.
        /// </summary>
        /// <param name="scope">A textual identity of a scope for which duration will be traced.</param>
        /// <param name="startTicks">The number of ticks that represent the date and time when the code entered the scope.</param>
        /// <param name="callToken">An unique value which is used as a correlation token to correlate TraceStartScope and TraceEndScope calls.</param>
        /// <returns>The calculated duration.</returns>
        public static long TraceEndScope(string scope, long startTicks, Guid callToken)
        {
            return TraceProvider.Logger.TraceStartScope(scope, startTicks, callToken);
        }

        #endregion TraceStartScope/TraceEndScope

        #region TraceInfo

        /// <summary>
        /// Writes an information message to the trace. This method is provided for optimal performance when
        /// tracing simple messages which don't require a format string.
        /// </summary>
        /// <param name="message">A string containing the message to be traced.</param>
        public static void TraceInfo(string message)
        {
            TraceProvider.Logger.TraceInfo(message);
        }

        /// <summary>
        /// Writes an information message to the trace.
        /// </summary>
        /// <param name="format">A string containing zero or more format items.</param>
        /// <param name="parameters">A list containing zero or more data items to format.</param>
        public static void TraceInfo(string format, params object[] parameters)
        {
            TraceProvider.Logger.TraceInfo(format, parameters);
        }

        /// <summary>
        /// Writes an information message to the trace. This method is intended to be used when the data that needs to be
        /// written to the trace is expensive to be fetched. The method represented by the Func(T) delegate will only be invoked if
        /// tracing is enabled.
        /// </summary>
        /// <param name="expensiveDataProvider">A method that has no parameters and returns a value that needs to be traced.</param>
        public static void TraceInfo(Func<string> expensiveDataProvider)
        {
            TraceProvider.Logger.TraceInfo(expensiveDataProvider);
        }

        #endregion TraceInfo

        #region TraceWarning

        /// <summary>
        /// Writes a warning message to the trace. This method is provided for optimal performance when
        /// tracing simple messages which don't require a format string.
        /// </summary>
        /// <param name="message">A string containing the message to be traced.</param>
        public static void TraceWarning(string message)
        {
            TraceProvider.Logger.TraceWarning(message);
        }

        /// <summary>
        /// Writes a warning message to the trace.
        /// </summary>
        /// <param name="format">A string containing zero or more format items.</param>
        /// <param name="parameters">A list containing zero or more data items to format.</param>
        public static void TraceWarning(string format, params object[] parameters)
        {
            TraceProvider.Logger.TraceWarning(format, parameters);
        }

        #endregion TraceWarning

        #region TraceError

        /// <summary>
        /// Writes an error message to the trace. This method is provided for optimal performance when
        /// tracing simple messages which don't require a format string.
        /// </summary>
        /// <param name="message">A string containing the error message to be traced.</param>
        public static void TraceError(string message, string callToken)
        {
            TraceProvider.Logger.TraceError(message);
        }

        /// <summary>
        /// Writes an error message to the trace.
        /// </summary>
        /// <param name="format">A string containing zero or more format items.</param>
        /// <param name="parameters">A list containing zero or more data items to format.</param>
        public static void TraceError(string format, params object[] parameters)
        {
            TraceProvider.Logger.TraceError(format, parameters);
        }

        /// <summary>
        /// Writes an error message to the trace.
        /// </summary>
        /// <param name="callToken">An unique value which is used as a correlation token to correlate TraceIn and TraceError calls.</param>
        /// <param name="format">A string containing zero or more format items.</param>
        /// <param name="parameters">A list containing zero or more data items to format.</param>
        public static void TraceError(Guid callToken, string format, params object[] parameters)
        {
            TraceProvider.Logger.TraceError(callToken, format, parameters);
        }

        /// <summary>
        /// Writes the exception details to the trace.
        /// </summary>
        /// <param name="ex">An exception to be formatted and written to the trace.</param>
        public static void TraceError(Exception ex)
        {
            TraceProvider.Logger.TraceError(ex);
        }

        /// <summary>
        /// Writes the exception details to the trace.
        /// </summary>
        /// <param name="ex">An exception to be formatted and written to the trace.</param>
        /// <param name="includeStackTrace">A flag indicating whether or not call stack details should be included.</param>
        public static void TraceError(Exception ex, bool includeStackTrace)
        {
            TraceProvider.Logger.TraceError(ex, includeStackTrace);
        }

        /// <summary>
        /// Writes the exception details to the trace.
        /// </summary>
        /// <param name="ex">An exception to be formatted and written to the trace.</param>
        /// <param name="includeStackTrace">A flag indicating whether or not call stack details should be included.</param>
        /// <param name="callToken">An unique value which is used as a correlation token to correlate TraceIn and TraceError calls.</param>
        public static void TraceError(Exception ex, bool includeStackTrace, Guid callToken)
        {
            TraceProvider.Logger.TraceError(ex, includeStackTrace, callToken);
        }

        #endregion TraceError
    }
}