using System;
using System.Collections.Generic;

using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Practices.ESB.Itinerary;

using GT.BizTalk.Framework.BizTalk;
using GT.BizTalk.Framework.Core.Tracing;
using System.Reflection;

namespace GT.BizTalk.Framework.ESBServices
{
    public class AsyncRouter : IMessagingService
    {
        #region IMessagingService implementation
        public string Name
        {
            get { return " GT.BizTalk.Framework.ESBServices.AsyncRouter"; }
        }

        public bool ShouldAdvanceStep(IItineraryStep step, IBaseMessage msg)
        {
            return true;
        }

        public bool SupportsDisassemble
        {
            get { return true; }
        }

        public Microsoft.BizTalk.Message.Interop.IBaseMessage Execute(IPipelineContext context, IBaseMessage msg, string resolverString, IItineraryStep step)
        {
            var callToken = TraceProvider.Logger.TraceIn(this.Name);
            try
            {
                bool IsDirectSynchronousACK = GetConfigValue<bool>(step.PropertyBag, "isDirectSynchronousACK", false);

                BTS.RouteDirectToTP rdttp = new BTS.RouteDirectToTP();

                if (IsDirectSynchronousACK)
                    msg.Context.Promote(rdttp.Name.Name, rdttp.Name.Namespace, true);
                else
                    msg.Context.Promote(rdttp.Name.Name, rdttp.Name.Namespace, false);

                return msg;

            }
            catch (Exception ex)
            {
                // put component name as a source information in this exception,
                // so the event log in message could reflect this
                ex.Source = this.Name;
                TraceProvider.Logger.TraceError(ex);
                throw ex;
            }
            finally
            {
                TraceProvider.Logger.TraceOut(callToken, this.Name);
            }
        }
        #endregion

        #region Helpers
        private static string GetConfigValue(Dictionary<string, string> propertyBag, string propertyName, string defaultValue)
        {
            try
            {
                string value = string.Empty;
                if (propertyBag.ContainsKey(propertyName) == true)
                {
                    value = propertyBag[propertyName];
                }
                return (value != null ? value : defaultValue);
            }
            catch (ArgumentException)
            {
                // return default value, if property cannot be read
                return defaultValue;
            }
        }

        private static T GetConfigValue<T>(Dictionary<string, string> propertyBag, string propertyName, T defaultValue)
        {
            try
            {
                string value = string.Empty;
                if (propertyBag.ContainsKey(propertyName) == true)
                {
                    value = propertyBag[propertyName];
                }
                if (value != null)
                {
                    if (typeof(T).IsEnum == true)
                    {
                        return (T)Enum.Parse(typeof(T), value.ToString());
                    }
                    else
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                }
                return defaultValue;
            }
            catch (FormatException)
            {
                // return default value, if property cannot be read
                return defaultValue;
            }
            catch (ArgumentException)
            {
                // return default value, if property cannot be read
                return defaultValue;
            }
        }
        #endregion
    }

}
