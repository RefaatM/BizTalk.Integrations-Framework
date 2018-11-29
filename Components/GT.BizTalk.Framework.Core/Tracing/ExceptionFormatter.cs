using GT.BizTalk.Framework.Core.Properties;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Security.Principal;
using System.Text;

namespace GT.BizTalk.Framework.Core.Tracing
{
    /// <summary>
    /// Provides exception formatting capability to construct a detailed message about an exception for logging and tracing purposes.
    /// </summary>
    public sealed class ExceptionFormatter
    {
        #region Private members

        /// <summary>
        /// Name of the additional information entry that holds the header.
        /// </summary>
        public static string Header = Resources.ExceptionFormatterHeader;

        private const char NewLine = '\n';
        private const string LineSeparator = "======================================";
        private const string EventTimestampFormatString = "dd/MM/yyyy HH:mm:ss.fff";

        private readonly NameValueCollection additionalInfo;
        private readonly string applicationName;

        #endregion Private members

        #region Constructors

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="ExceptionFormatter"/> class.</para>
        /// </summary>
        public ExceptionFormatter()
            : this(AppDomain.CurrentDomain.SetupInformation.ApplicationName)
        {
        }

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="ExceptionFormatter"/> class with the application name.</para>
        /// </summary>
        /// <param name="applicationName">
        /// <para>The application name.</para>
        /// </param>
        public ExceptionFormatter(string applicationName)
            : this(applicationName, new NameValueCollection(10))
        {
        }

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="ExceptionFormatter"/> class with the additional information and the application name.</para>
        /// </summary>
        /// <param name="additionalInfo">
        /// <para>The additional information to log.</para>
        /// </param>
        /// <param name="applicationName">
        /// <para>The application name.</para>
        /// </param>
        public ExceptionFormatter(string applicationName, NameValueCollection additionalInfo)
        {
            this.applicationName = applicationName;
            this.additionalInfo = additionalInfo;
        }

        #endregion Constructors

        #region Public methods

        /// <summary>
        /// <para>Get the formatted message to be logged.</para>
        /// </summary>
        /// <param name="exception"><para>The exception object whose information should be written to log file.</para></param>
        /// <returns><para>The formatted message.</para></returns>
        public string FormatException(Exception exception)
        {
            return FormatException(exception, true);
        }

        /// <summary>
        /// <para>Get the formatted message to be logged.</para>
        /// </summary>
        /// <param name="exception"><para>The exception object whose information should be written to log file.</para></param>
        /// <param name="includeStackTrace">A flag indicating whether or not call stack details should be included.</param>
        /// <returns><para>The formatted message.</para></returns>
        public string FormatException(Exception exception, bool includeStackTrace)
        {
            StringBuilder eventInformation = new StringBuilder();
            CollectAdditionalInfo();

            // Record the contents of the AdditionalInfo collection.
            if (this.additionalInfo.Get(Header) != null)
            {
                eventInformation.AppendFormat("{0}{1}{1}", this.additionalInfo.Get(Header), NewLine);
            }

            eventInformation.AppendFormat("{1} {0}:{3}{2}", Resources.ExceptionSummary, this.applicationName, LineSeparator, NewLine);

            foreach (string info in this.additionalInfo)
            {
                if (info != Header)
                {
                    eventInformation.AppendFormat("{1}{0}", this.additionalInfo.Get(info), NewLine);
                }
            }

            if (exception != null)
            {
                Exception currException = exception;
                int exceptionCount = 1;
                do
                {
                    eventInformation.AppendFormat("{2}{2}{0}{2}{1}", Resources.ExceptionDetails, LineSeparator, NewLine);
                    eventInformation.AppendFormat("{2}{0}: {1}", Resources.ExceptionType, currException.GetType().FullName, NewLine);

                    ReflectException(currException, eventInformation);

                    // Record the StackTrace with separate label.
                    if (includeStackTrace && currException.StackTrace != null)
                    {
                        eventInformation.AppendFormat("{2}{2}{0} {2}{1}", Resources.ExceptionStackTraceDetails, LineSeparator, NewLine);
                        eventInformation.AppendFormat("{1}{0}", currException.StackTrace, NewLine);
                    }

                    // Reset the temp exception object and iterate the counter.
                    currException = currException.InnerException;
                    exceptionCount++;
                }
                while (currException != null);
            }

            return eventInformation.ToString();
        }

        #endregion Public methods

        #region Private methods

        private static void ReflectException(Exception currException, StringBuilder strEventInfo)
        {
            PropertyInfo[] arrPublicProperties = currException.GetType().GetProperties();

            foreach (PropertyInfo propinfo in arrPublicProperties)
            {
                // Do not log information for the InnerException or StackTrace. This information is
                // captured later in the process.
                if (propinfo.Name != "InnerException" && propinfo.Name != "StackTrace")
                {
                    if (propinfo.GetValue(currException, null) != null)
                    {
                        ProcessAdditionalInfo(propinfo, currException, strEventInfo);
                    }
                }
            }
        }

        private static void ProcessAdditionalInfo(PropertyInfo propInfo, Exception currException, StringBuilder stringBuilder)
        {
            NameValueCollection currAdditionalInfo;

            // Loop through the collection of AdditionalInformation if the exception type is a BaseApplicationException.
            if (propInfo.Name == "AdditionalInformation")
            {
                if (propInfo.GetValue(currException, null) != null)
                {
                    // Cast the collection into a local variable.
                    currAdditionalInfo = (NameValueCollection)propInfo.GetValue(currException, null);

                    // Check if the collection contains values.
                    if (currAdditionalInfo.Count > 0)
                    {
                        stringBuilder.AppendFormat("{0}Additional Information:", NewLine);

                        // Loop through the collection adding the information to the string builder.
                        for (int i = 0; i < currAdditionalInfo.Count; i++)
                        {
                            stringBuilder.AppendFormat("{2}--> {0}: {1}", currAdditionalInfo.GetKey(i), currAdditionalInfo[i], NewLine);
                        }
                    }
                }
            }
            if (propInfo.Name == "Data")
            {
                object propValue = propInfo.GetValue(currException, null);
                if (propValue != null)
                {
                    IDictionary additionalData = propValue as IDictionary;

                    // Check if the collection contains values.
                    if (additionalData != null && additionalData.Count > 0)
                    {
                        stringBuilder.AppendFormat("{0}Additional Data:", NewLine);

                        // Loop through the collection adding the information to the string builder.
                        IDictionaryEnumerator dataEnumerator = additionalData.GetEnumerator();
                        while (dataEnumerator.MoveNext())
                        {
                            stringBuilder.AppendFormat("{2}--> {0}: {1}", dataEnumerator.Key, dataEnumerator.Value, NewLine);
                        }
                    }
                }
            }
            else
            {
                // Otherwise just write the ToString() value of the property.
                stringBuilder.AppendFormat("{2}{0}: {1}", propInfo.Name, propInfo.GetValue(currException, null), NewLine);
            }
        }

        /// <devdoc>
        /// Add additional environmental information.
        /// </devdoc>
        private void CollectAdditionalInfo()
        {
            if (this.additionalInfo["MachineName:"] != null)
            {
                return;
            }

            this.additionalInfo.Add("Timestamp:", "Timestamp: " + DateTime.Now.ToString(EventTimestampFormatString, CultureInfo.CurrentCulture));
            this.additionalInfo.Add("MachineName:", "Machine Name: " + Environment.MachineName);
            this.additionalInfo.Add("AssemblyFullName:", "Assembly Full Name: " + Assembly.GetExecutingAssembly().FullName);
            this.additionalInfo.Add("AssemblyVersion:", "Assembly Version: " + Assembly.GetExecutingAssembly().GetName().Version);
            this.additionalInfo.Add("AppDomainName:", "App Domain Name: " + AppDomain.CurrentDomain.FriendlyName);
            this.additionalInfo.Add("ApplicationBase:", "Application Base Path: " + AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            this.additionalInfo.Add("WindowsIdentity:", "Windows Identity: " + WindowsIdentity.GetCurrent().Name);
        }

        #endregion Private methods
    }
}