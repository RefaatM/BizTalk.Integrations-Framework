using DE.DAXFSA.Framework.Core.Tracing;
using Microsoft.BizTalk.Bam.EventObservation;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;

namespace DE.DAXFSA.Framework.BizTalk.BAM
{
    public class BAMRelatedActivityTracker : IRelatedBAMActivitiesTracker
    {
        #region Constants

        private const string CONTINUATION_PREFIX = "CONT_";

        #endregion Constants

        /// <summary>
        /// implements the IRelatedBAMActivitiesTracker Execute method
        /// </summary>
        /// <param name="context">pipeline context</param>
        /// <param name="msg">IBaseMessage</param>
        /// <param name="eventStream">the eventstream for writing BAM data</param>
        /// <param name="PrimaryActivityName">the primary BAM activity name</param>
        /// <param name="PrimaryActivityID">the primary BAM activity ID</param>
        /// <param name="RelatedActivityName">the related BAM activity name</param>
        /// <param name="BAMPropertyEventSources">a list of BAM activity event source</param>
        /// <param name="BAMPropertyEventSourceValues">a list of BAM activity event source values</param>
        /// <param name="PostprocessXPathEventSources">a list of xpath for post-processing</param>
        /// <param name="EnableContinuation">if related activity should be enabled</param>
        /// <param name="UseContinuation">if related activity should use continuation</param>
        public void Execute(IPipelineContext context
                            , IBaseMessage msg
                            , ref EventStream eventStream
                            , string PrimaryActivityName
                            , string PrimaryActivityID
                            , string RelatedActivityName
                            , List<BAMPropertyEventSource> BAMPropertyEventSources
                            , Dictionary<string, object> BAMPropertyEventSourceValues
                            , List<BAMPropertyEventSource> PostprocessXPathEventSources
                            , bool EnableContinuation
                            , bool UseContinuation
                        )
        {
            var callToken = TraceProvider.Logger.TraceIn(this.GetType().FullName);
            try
            {
                if (BAMPropertyEventSources != null && BAMPropertyEventSources.Count > 0)
                {
                    if (eventStream != null)
                    {
                        List<object> eventData = new List<object>();

                        var relatedXpathEventSources = from propertySource in BAMPropertyEventSources
                                                       where propertySource.Source.Equals(BAMPropertyEventSourceSource.XPath)
                                                       select propertySource;

                        if (relatedXpathEventSources.Count() == 1
                            && relatedXpathEventSources.First().ActivityItem.Equals(BAMTrackerFact.RELATED_ACTIVITY_XPATH_ROOT_SOURCE)
                            && BAMPropertyEventSourceValues.ContainsKey(BAMTrackerFact.RELATED_ACTIVITY_XPATH_ROOT_SOURCE))
                        {
                            #region --------------- handle multiple related activities creation ---------------

                            var activitySource = (XPathNavigator)BAMPropertyEventSourceValues[BAMTrackerFact.RELATED_ACTIVITY_XPATH_ROOT_SOURCE];
                            Dictionary<string, string> xpathQueries = new Dictionary<string, string>();

                            PostprocessXPathEventSources.ToList().ForEach(x => xpathQueries.Add(x.ActivityItem, x.Value));
                            Dictionary<string, object> xpathValues = GetXPathValues(activitySource, xpathQueries);

                            for (int valueIndex = 0; valueIndex < ((List<object>)xpathValues.First().Value).Count; valueIndex++)
                            {
                                var originalRelatedId = string.Concat(PrimaryActivityID, "/", valueIndex);
                                var relatedId = string.Concat(PrimaryActivityID, "/", valueIndex);
                                List<object> relatedEventData = new List<object>();

                                relatedEventData.Add("PrimaryActivityId");
                                relatedEventData.Add(PrimaryActivityID);

                                foreach (var xpathQuery in xpathQueries)
                                {
                                    var itemValue = xpathValues.ContainsKey(xpathQuery.Key) && ((List<object>)xpathValues[xpathQuery.Key]).Count > 0
                                                        ? ((List<object>)xpathValues[xpathQuery.Key])[valueIndex]
                                                        : null;
                                    if (itemValue != null)
                                    {
                                        relatedEventData.Add(xpathQuery.Key);
                                        relatedEventData.Add(itemValue);
                                    }
                                }

                                if (BAMPropertyEventSources != null && BAMPropertyEventSources.Count() > 0)
                                {
                                    foreach (var eventsource in BAMPropertyEventSources)
                                    {
                                        if (!eventsource.Source.Equals(BAMPropertyEventSourceSource.XPath))
                                        {
                                            if (BAMPropertyEventSourceValues.ContainsKey(eventsource.ActivityItem))
                                            {
                                                relatedEventData.Add(eventsource.ActivityItem);
                                                relatedEventData.Add(BAMPropertyEventSourceValues[eventsource.ActivityItem]);
                                            }
                                        }
                                    }
                                }

                                #region start BAM Activity

                                // if using continuation, open activity with the continuation ID
                                if (UseContinuation)
                                {
                                    relatedId = CONTINUATION_PREFIX + originalRelatedId;
                                }
                                else
                                {
                                    TraceProvider.Logger.TraceInfo("Opening activity [{0}] with ID: {1}...", RelatedActivityName, relatedId);
                                    eventStream.BeginActivity(RelatedActivityName, relatedId);
                                }

                                #endregion start BAM Activity

                                #region update BAM Activity

                                TraceProvider.Logger.TraceInfo("Writing event data to activity [{0}] with ID: {1}...", RelatedActivityName, relatedId);
                                // write BAM data
                                eventStream.UpdateActivity(RelatedActivityName, relatedId, relatedEventData.ToArray());

                                #endregion update BAM Activity

                                #region BAM Activity Continuation

                                // check if we need to enable continuation
                                if (EnableContinuation == true && UseContinuation != true)
                                {
                                    string continuationToken = CONTINUATION_PREFIX + originalRelatedId;
                                    TraceProvider.Logger.TraceInfo("Enabling continuation on activity [{0}] with ID: {1} using continuation token: {2}...", RelatedActivityName, relatedId, continuationToken);
                                    eventStream.EnableContinuation(RelatedActivityName, relatedId, continuationToken);
                                }

                                #endregion BAM Activity Continuation

                                #region end related BAM Activity

                                // end updates to activity
                                if (EnableContinuation == false || UseContinuation == false)
                                {
                                    eventStream.EndActivity(RelatedActivityName, relatedId);
                                }
                                eventStream.Flush();

                                #endregion end related BAM Activity

                                #region add related activity

                                eventStream.AddRelatedActivity(PrimaryActivityName, PrimaryActivityID, RelatedActivityName, relatedId);

                                #endregion add related activity

                                TraceProvider.Logger.TraceInfo("Finished writing to activity [{0}] with ID: {1}...", RelatedActivityName, relatedId);
                            }

                            #endregion --------------- handle multiple related activities creation ---------------
                        }
                        else
                        {
                            #region --------------- handle single related activity creation ---------------

                            var originalRelatedId = string.Concat(PrimaryActivityID, "/", 0);
                            var relatedId = string.Concat(PrimaryActivityID, "/", 0);
                            List<object> relatedEventData = new List<object>();

                            relatedEventData.Add("PrimaryActivityId");
                            relatedEventData.Add(PrimaryActivityID);

                            if (BAMPropertyEventSources != null && BAMPropertyEventSources.Count() > 0)
                            {
                                foreach (var eventsource in BAMPropertyEventSources)
                                {
                                    var itemValue = BAMPropertyEventSourceValues.ContainsKey(eventsource.ActivityItem)
                                                        ? BAMPropertyEventSourceValues[eventsource.ActivityItem]
                                                        : null;
                                    if (itemValue != null)
                                    {
                                        relatedEventData.Add(eventsource.ActivityItem);
                                        relatedEventData.Add(itemValue);
                                    }
                                }
                            }

                            #region start BAM Activity

                            // if using continuation, open activity with the continuation ID
                            if (UseContinuation)
                            {
                                relatedId = CONTINUATION_PREFIX + originalRelatedId;
                            }
                            else
                            {
                                TraceProvider.Logger.TraceInfo("Opening activity [{0}] with ID: {1}...", RelatedActivityName, relatedId);
                                eventStream.BeginActivity(RelatedActivityName, relatedId);
                            }

                            #endregion start BAM Activity

                            #region update BAM Activity

                            TraceProvider.Logger.TraceInfo("Writing event data to activity [{0}] with ID: {1}...", RelatedActivityName, relatedId);
                            // write BAM data
                            eventStream.UpdateActivity(RelatedActivityName, relatedId, relatedEventData.ToArray());

                            #endregion update BAM Activity

                            #region BAM Activity Continuation

                            // check if we need to enable continuation
                            if (EnableContinuation == true && UseContinuation != true)
                            {
                                string continuationToken = CONTINUATION_PREFIX + originalRelatedId;
                                TraceProvider.Logger.TraceInfo("Enabling continuation on activity [{0}] with ID: {1} using continuation token: {2}...", RelatedActivityName, relatedId, continuationToken);
                                eventStream.EnableContinuation(RelatedActivityName, relatedId, continuationToken);
                            }

                            #endregion BAM Activity Continuation

                            #region end related BAM Activity

                            // end updates to activity
                            if (EnableContinuation == false || UseContinuation == false)
                            {
                                eventStream.EndActivity(RelatedActivityName, relatedId);
                            }
                            eventStream.Flush();

                            #endregion end related BAM Activity

                            #region add related activity

                            eventStream.AddRelatedActivity(PrimaryActivityName, PrimaryActivityID, RelatedActivityName, relatedId);

                            #endregion add related activity

                            TraceProvider.Logger.TraceInfo("Finished writing to activity [{0}] with ID: {1}...", RelatedActivityName, relatedId);

                            #endregion --------------- handle single related activity creation ---------------
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // put component name as a source information in this exception,
                // so the event log in message could reflect this
                ex.Source = this.GetType().FullName;
                TraceProvider.Logger.TraceError(ex);
                throw ex;
            }
            finally
            {
                TraceProvider.Logger.TraceOut(callToken, this.GetType().FullName);
            }
        }

        #region helpers

        /// <summary>
        /// retrieve xpath value for a list of xpath query
        /// </summary>
        /// <param name="xpathNav">an xpath navigator</param>
        /// <param name="XPathLists">a list of xpath queries</param>
        /// <returns>list of xpath values</returns>
        private Dictionary<string, object> GetXPathValues(XPathNavigator xpathNav, Dictionary<string, string> XPathLists)
        {
            var result = new Dictionary<string, object>();

            if (XPathLists != null)
            {
                TraceProvider.Logger.TraceInfo("Using {0} XPath properties", XPathLists.Count);
                foreach (var xpathProperty in XPathLists)
                {
                    // evaluate xpath expression
                    List<object> propertyValue = EvaluateXPath(xpathProperty.Value, xpathNav);
                    // check if we found the value
                    if (propertyValue != null)
                    {
                        TraceProvider.Logger.TraceInfo("Xpath property [{0}] = {1}", xpathProperty.Value, propertyValue);
                        result.Add(xpathProperty.Key, propertyValue);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// evaluate xpath using provided XPath navigator and xpath property list
        /// </summary>
        /// <param name="xpathProperty">xpath property list</param>
        /// <param name="navigator">xpath navigator</param>
        /// <returns>List of values (i.e. string, number, boolean, or XPathNavigator</returns>
        private List<object> EvaluateXPath(string xpath, XPathNavigator navigator)
        {
            List<object> result = new List<object>();
            // create an xpath expression
            XPathExpression expression = XPathExpression.Compile(xpath);

            switch (expression.ReturnType)
            {
                case XPathResultType.String:
                case XPathResultType.Number:
                    result.Add(navigator.Evaluate(expression));
                    break;

                case XPathResultType.NodeSet:
                    XPathNodeIterator ni = navigator.Select(expression);
                    if (ni.Count == 1)
                    {
                        if (ni.MoveNext() == true)
                        {
                            if (ni.Current.NodeType.Equals(XPathNodeType.Text)
                                || ni.Current.NodeType.Equals(XPathNodeType.Attribute))
                            {
                                result.Add(ni.Current.ToString());
                            }
                            else
                            {
                                result.Add(ni.Current.Clone());
                            }
                        }
                    }
                    else if (ni.Count > 1)
                    {
                        if (ni.MoveNext() == true)
                        {
                            if (ni.Current.NodeType.Equals(XPathNodeType.Text)
                                || ni.Current.NodeType.Equals(XPathNodeType.Attribute))
                            {
                                do
                                {
                                    result.Add(ni.Current.ToString());
                                }
                                while (ni.MoveNext());
                            }
                            else if (ni.Current.NodeType.Equals(XPathNodeType.Element))
                            {
                                do
                                {
                                    result.Add(ni.Current.OuterXml);
                                }
                                while (ni.MoveNext());
                            }
                            else
                            {
                                result.Add(ni.Current.Clone());
                            }
                        }
                    }
                    break;

                case XPathResultType.Boolean:
                    result.Add(navigator.Evaluate(expression));
                    break;
            }
            return result;
        }

        #endregion helpers
    }
}