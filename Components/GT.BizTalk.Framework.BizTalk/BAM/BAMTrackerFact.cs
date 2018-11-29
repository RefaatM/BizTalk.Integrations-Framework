using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;

namespace DE.DAXFSA.Framework.BizTalk.BAM
{
    public class BAMTrackerFact
    {
        #region constants

        public const string RELATED_ACTIVITY_XPATH_ROOT_SOURCE = "{58CEBF6A-9001-417A-8786-D7087020CFFA}";

        #endregion constants

        #region private fields

        private List<BAMPropertyEventSource> propertyEventSources;
        private List<BAMPropertyEventSource> relatedActivityPropertyEventSources;
        private List<BAMPropertyEventSource> postprocessRelatedActivityXPathEventSources;
        private string _RelatedActivityTrackerHandlerAssemblyName = string.Empty;
        private IRelatedBAMActivitiesTracker _RelatedActivityTrackerHandler = null;
        private IPipelineContext _pContext;
        private IBaseMessage _iMsg;

        #endregion private fields

        #region constructor

        public BAMTrackerFact(IPipelineContext context, IBaseMessage msg)
        {
            propertyEventSources = new List<BAMPropertyEventSource>();
            relatedActivityPropertyEventSources = new List<BAMPropertyEventSource>();
            postprocessRelatedActivityXPathEventSources = new List<BAMPropertyEventSource>();
            this._iMsg = msg;
            this._pContext = context;
        }

        #endregion constructor

        public string ActivityName { get; set; }

        public string RelatedActivityName { get; set; }

        public bool UseContinuation { get; set; }

        public bool EnableContinuation { get; set; }

        public bool RelatedActivityUseContinuation { get; set; }

        public bool RelatedActivityEnableContinuation { get; set; }

        public bool UseInterchangeIdAsActivityId { get; set; }

        public bool UseRelatedActivityXPathRootSourceAsRelatedActivitySource { get; set; }

        ///// <summary>
        ///// Retrieves the current entry in the Message Context Value collection if present, otherwise it returns an empty string
        ///// </summary>
        ///// <param name="propName">The Name of the property to retrieve</param>
        ///// <param name="propNamespace">The namespace of the property name to retrieve</param>
        ///// <returns>String representation of the value</returns>
        //public object GetContextPropertyValue(string propName, string propNamespace)
        //{
        //    if (string.IsNullOrEmpty(propName) == true || string.IsNullOrEmpty(propNamespace) == true)
        //        return string.Empty;

        //    string key = string.Format("{0}#{1}", propNamespace, propName);
        //    object value = null;
        //    try
        //    {
        //        value = this._iMsg.Context.Read(propName, propNamespace);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceProvider.Logger.TraceInfo("Error retrieving context property {0}: {1}", key, ex.Message);
        //    }
        //    return value;
        //}

        public string RelatedActivityTrackerHandlerAssemblyName
        {
            get { return _RelatedActivityTrackerHandlerAssemblyName; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var type = Type.GetType(value);
                    _RelatedActivityTrackerHandler = (IRelatedBAMActivitiesTracker)Activator.CreateInstance(type);
                }
                _RelatedActivityTrackerHandlerAssemblyName = value;
            }
        }

        public IRelatedBAMActivitiesTracker RelatedActivityTrackerHandler
        {
            get
            {
                return _RelatedActivityTrackerHandler;
            }
        }

        public List<BAMPropertyEventSource> PropertyEventSources
        {
            get
            {
                return propertyEventSources;
            }
        }

        public List<BAMPropertyEventSource> RelatedActivityPropertyEventSources
        {
            get
            {
                return relatedActivityPropertyEventSources;
            }
        }

        public List<BAMPropertyEventSource> PostProcessRelatedActivityXPathEventSources
        {
            get
            {
                return postprocessRelatedActivityXPathEventSources;
            }
        }

        public void AddActivityEventSource(string ActivityItem, BAMPropertyEventSourceSource Source, string Value, bool IgnoreNullOrEmptyValue)
        {
            propertyEventSources.Add(new BAMPropertyEventSource
            {
                ActivityItem = ActivityItem,
                Source = Source,
                Value = Value,
                IgnoreNullOrEmptyValue = IgnoreNullOrEmptyValue,
            }
            );
        }

        public void AddRelatedActivityEventSource(string ActivityItem, BAMPropertyEventSourceSource Source, string Value, bool IgnoreNullOrEmptyValue)
        {
            relatedActivityPropertyEventSources.Add(new BAMPropertyEventSource
            {
                ActivityItem = ActivityItem,
                Source = Source,
                Value = Value,
                IgnoreNullOrEmptyValue = IgnoreNullOrEmptyValue,
            }
            );
        }

        public void AddPostProcessRelatedActivityXPathEventSource(string ActivityItem, string Value, bool IgnoreNullOrEmptyValue)
        {
            postprocessRelatedActivityXPathEventSources.Add(new BAMPropertyEventSource
            {
                ActivityItem = ActivityItem,
                Source = BAMPropertyEventSourceSource.XPath,
                Value = Value,
                IgnoreNullOrEmptyValue = IgnoreNullOrEmptyValue,
            }
            );
        }

        public string GetContextProperty(string propertyName, string propertyNamespace)
        {
            var value = this._iMsg.Context.Read(propertyName, propertyNamespace);
            if (value != null)
                return Convert.ToString(value);
            return string.Empty;
        }
    }
}