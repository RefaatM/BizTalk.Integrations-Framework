using DE.DAXFSA.Framework.BizTalk;
using DE.DAXFSA.Framework.Core.Tracing;
using DE.DAXFSA.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Bam.EventObservation;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DE.DAXFSA.Framework.PipelineComponents
{
    /// <summary>
    /// Pipeline component for BAM activity tracking.
    /// </summary>
    [System.Runtime.InteropServices.Guid("B357E9BD-9899-43AF-9E33-3D7954ADCE3D")]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Any)]
    public sealed class BAMTracker : BasePipelineComponent
    {
        #region Constants

        private const string CONTINUATION_PREFIX = "CONT_";
        private const string ACTIVITY_NAME_PROP = "ActivityName";
        private const string ENABLE_CONTINUATION_PROP = "EnableContinuation";
        private const string USE_CONTINUATION_PROP = "UseContinuation";
        private const string EVENT_SOURCES_PROP = "EventSources";
        private const string ACTIVITY_ID_CTX_PROP_NAME = "ContinuationID";
        private const string ACTIVITY_ID_CTX_PROP_NAMESPACE = "https://schemas.DirectEnergy.com/Unify/Common/bam/context/2015";

        #endregion Constants

        #region Constructors

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public BAMTracker()
            : base(Resources.ResourceManager, Resources.BAMTrackerName, Resources.BAMTrackerDescription, Resources.BAMTrackerVersion, Resources.BAMTrackerIcon)
        {
            this.EventSources = new List<BAMEventSource>();
        }

        #endregion Constructors

        #region Design-time Properties

        /// <summary>
        /// Gets/sets the BAM activity name.
        /// </summary>
        [BtsPropertyName("BAMActivityPropertyName")]
        [BtsDescription("BAMActivityPropertyDescription")]
        public string ActivityName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets a value that indicates if continuations should be enabled.
        /// </summary>
        [BtsPropertyName("BAMEnableContinuationPropertyName")]
        [BtsDescription("BAMEnableContinuationPropertyDescription")]
        public bool EnableContinuation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets a value that indicates if continuations should be used.
        /// </summary>
        [BtsPropertyName("BAMUseContinuationPropertyName")]
        [BtsDescription("BAMUseContinuationPropertyDescription")]
        public bool UseContinuation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets o sets the context value collection.
        /// </summary>
        [BtsPropertyName("BAMEventsPropertyName")]
        [BtsDescription("BAMEventsPropertyDescription")]
        [Editor(typeof(System.ComponentModel.Design.CollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public List<BAMEventSource> EventSources
        {
            get;
            set;
        }

        #endregion Design-time Properties

        #region BasePipelineComponent Members

        /// <summary>
        /// Load component properties from a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="errorLog">Error log level</param>
        protected override void LoadProperties(IPropertyBag propertyBag, int errorLog)
        {
            this.ActivityName = this.ReadPropertyValue<string>(propertyBag, ACTIVITY_NAME_PROP, this.ActivityName);
            this.EnableContinuation = this.ReadPropertyValue<bool>(propertyBag, ENABLE_CONTINUATION_PROP, this.EnableContinuation);
            this.UseContinuation = this.ReadPropertyValue<bool>(propertyBag, USE_CONTINUATION_PROP, this.UseContinuation);
            string xml = this.ReadPropertyValue<string>(propertyBag, EVENT_SOURCES_PROP, null);
            if (string.IsNullOrEmpty(xml) == false)
            {
                BAMEventSourceSerializer serializer = new BAMEventSourceSerializer();
                serializer.Deserialize(xml);
                this.EventSources = serializer.Properties;
            }
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected override void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            this.WritePropertyValue(propertyBag, ACTIVITY_NAME_PROP, this.ActivityName);
            this.WritePropertyValue(propertyBag, ENABLE_CONTINUATION_PROP, this.EnableContinuation);
            this.WritePropertyValue(propertyBag, USE_CONTINUATION_PROP, this.UseContinuation);

            string xml = null;
            if (this.EventSources != null)
            {
                BAMEventSourceSerializer serializer = new BAMEventSourceSerializer(this.EventSources);
                xml = serializer.Serialize();
            }
            this.WritePropertyValue(propertyBag, EVENT_SOURCES_PROP, xml);
        }

        /// <summary>
        /// Validates the component properties.
        /// </summary>
        /// <returns>
        /// A list of error and/or warning messages encounter during validation
        /// of this component.
        /// </returns>
        protected override List<string> Validate()
        {
            List<string> errors = new List<string>();

            if (String.IsNullOrWhiteSpace(this.ActivityName) == true)
                errors.Add(Resources.InvalidActivityName);

            return errors;
        }

        /// <summary>
        /// Returns a value indicating whether the pipeline component is enabled.
        /// </summary>
        /// <remarks>
        /// Checks the BAMTraker.Enabled context property; otherwise returns the Enabled pipeline
        /// configuration setting.
        /// </remarks>
        /// <param name="inputMessage">Input message</param>
        /// <returns><b>true</b> if the component is enabled; <b>false</b> otherwise.</returns>
        protected override bool IsEnabled(IBaseMessage inputMessage)
        {
            return inputMessage.Context.Read<bool>(BAMTrackerProperties.Enabled.Name, BAMTrackerProperties.Enabled.Namespace, this.Enabled);
        }

        /// <summary>
        /// Performs the BAM activity tracking using the current message context and instance.
        /// </summary>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="inputMessage">Input message</param>
        /// <returns>Original input message</returns>
        protected override IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            // Note: this component, as a general rule, first attemps to read a property from the context and if
            // it cannot be found there, it uses the design property value
            string activityName = inputMessage.Context.Read<string>(BAMTrackerProperties.ActivityName.Name, BAMTrackerProperties.ActivityName.Namespace, this.ActivityName);
            List<BAMEventSource> eventSources = inputMessage.Context.Read<List<BAMEventSource>>(BAMTrackerProperties.EnableContinuation.Name, BAMTrackerProperties.EnableContinuation.Namespace, this.EventSources);
            if (eventSources != null && eventSources.Count > 0)
            {
                TraceProvider.Logger.TraceInfo("Retrieving Messaging Event Stream from pipeline context...");
                // get a messaging BAM event stream instance that participates in the pipeline transaction context
                EventStream eventStream = pipelineContext.GetEventStream();
                if (eventStream != null)
                {
                    // set default BAM activity ID
                    string activityID = Guid.NewGuid().ToString();

                    // read message InterchangeID to use it as the continuation token
                    TraceProvider.Logger.TraceInfo("Attempting to read InterchangeID from context...");
                    // attempt to read the activity ID from the context
                    string interchangeID = inputMessage.Context.Read<string>(BtsProperties.InterchangeID.Name, BtsProperties.InterchangeID.Namespace, null);
                    if (String.IsNullOrEmpty(interchangeID) == true)
                        throw new InvalidOperationException("InterchangeID not found in context.");

                    // if using continuation, open activity with the continuation ID
                    bool useContinuation = inputMessage.Context.Read<bool>(BAMTrackerProperties.UseContinuation.Name, BAMTrackerProperties.UseContinuation.Namespace, this.UseContinuation);
                    if (useContinuation == true)
                        activityID = CONTINUATION_PREFIX + interchangeID;

                    // open activity
                    TraceProvider.Logger.TraceInfo("Opening activity {0} with ID: {1}...", activityName, activityID);
                    eventStream.BeginActivity(activityName, activityID);

                    // check if we need to enable continuation
                    bool enableContinuation = inputMessage.Context.Read<bool>(BAMTrackerProperties.EnableContinuation.Name, BAMTrackerProperties.EnableContinuation.Namespace, this.EnableContinuation);
                    if (this.EnableContinuation == true)
                    {
                        string continuationToken = CONTINUATION_PREFIX + interchangeID;
                        TraceProvider.Logger.TraceInfo("Enabling continuation on activity {0} with ID: {1} using continuation token: {2}...", activityName, activityID, continuationToken);
                        eventStream.EnableContinuation(activityName, activityID, continuationToken);
                    }

                    TraceProvider.Logger.TraceInfo("Writing event data to activity {0} with ID: {1}...", activityName, activityID);
                    // write BAM data
                    List<object> eventData = new List<object>();
                    foreach (BAMEventSource eventSource in eventSources)
                    {
                        // add activity item key (this the BAM activity item name in the BAM definition)
                        eventData.Add(eventSource.ActivityItem);
                        // read from the context the corresponding value
                        object itemValue = inputMessage.Context.Read(eventSource.ContextPropertyName, eventSource.ContextPropertyNamespace);
                        eventData.Add(itemValue);
                    }

                    // update activity
                    eventStream.UpdateActivity(activityName, activityID, eventData.ToArray());
                    // end updates to activity
                    eventStream.EndActivity(activityName, activityID);
                    TraceProvider.Logger.TraceInfo("Finished writing to activity {0} with ID: {1}...", activityName, activityID);
                }
            }

            return inputMessage;
        }

        #endregion BasePipelineComponent Members
    }
}