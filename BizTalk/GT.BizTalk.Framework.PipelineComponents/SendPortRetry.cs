using DE.DAXFSA.Framework.BizTalk;
using DE.DAXFSA.Framework.Core.Tracing;
using DE.DAXFSA.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;

namespace DE.DAXFSA.Framework.PipelineComponents
{
    /// <summary>
    /// Implements a pipeline component that sets the RetryCount and RetryInterval
    /// message context properties to enable the BizTalk retry functionality.
    /// </summary>
    [System.Runtime.InteropServices.Guid("00FF5CE5-B095-48E9-83CF-EBB17CC281E4")]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Transmitter)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Encoder)]
    public class SendPortRetry : BasePipelineComponent
    {
        #region Constants

        private const string RETRY_COUNT_PROP_NAME = "RetryCount";
        private const string RETRY_INTERVAL_PROP_NAME = "RetryInterval";

        #endregion Constants

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public SendPortRetry()
            : base(Resources.ResourceManager, Resources.SendPortRetryName, Resources.SendPortRetryDescription, Resources.SendPortRetryVersion, Resources.SendPortRetryIcon)
        {
        }

        #endregion Constructor

        #region Design-time properties

        /// <summary>
        /// Gets/sets the number of attempts the engine resends if transmission fails.
        /// </summary>
        [BtsPropertyName("RetryCountPropertyName")]
        [BtsDescription("RetryCountPropertyDescription")]
        public int RetryCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the time interval in minutes the engine waits between retries.
        /// </summary>
        [BtsPropertyName("RetryIntervalPropertyName")]
        [BtsDescription("RetryIntervalPropertyDescription")]
        public int RetryInterval
        {
            get;
            set;
        }

        #endregion Design-time properties

        #region BasePipelineComponent Members

        /// <summary>
        /// Load component properties from a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="errorLog">Error log level</param>
        protected override void LoadProperties(IPropertyBag propertyBag, int errorLog)
        {
            this.RetryCount = this.ReadPropertyValue<int>(propertyBag, RETRY_COUNT_PROP_NAME, this.RetryCount);
            this.RetryInterval = this.ReadPropertyValue<int>(propertyBag, RETRY_INTERVAL_PROP_NAME, this.RetryInterval);
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected override void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            this.WritePropertyValue(propertyBag, RETRY_COUNT_PROP_NAME, this.RetryCount);
            this.WritePropertyValue(propertyBag, RETRY_INTERVAL_PROP_NAME, this.RetryInterval);
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
            return null;
        }

        /// <summary>
        /// Archives the input message into the specified archiving location.
        /// </summary>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="inputMessage">Input message</param>
        /// <returns>Original input message</returns>
        protected override IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            // check if the message already has the RetryCount and RetryInterval properties
            // to skip the processing
            int retryCount = inputMessage.Context.Read<int>(BtsProperties.RetryCount.Name, BtsProperties.RetryCount.Namespace, 0);
            int retryInterval = inputMessage.Context.Read<int>(BtsProperties.RetryInterval.Name, BtsProperties.RetryInterval.Namespace, 0);
            if (retryCount <= 0 && retryInterval <= 0)
            {
                // get the actual number of transmissions the send handler has attempted
                int actualRetryCount = inputMessage.Context.Read<int>(BtsProperties.ActualRetryCount.Name, BtsProperties.ActualRetryCount.Namespace, 0);
                TraceProvider.Logger.TraceInfo("Message context ActualRetryCount = {0}", actualRetryCount);

                // get configured retry settings
                retryCount = this.RetryCount;
                retryInterval = this.RetryInterval;
                TraceProvider.Logger.TraceInfo("SendPortRetryService configuration settings: RetryCount = {0}, RetryInterval = {1}", retryCount, retryInterval);
                if (retryCount > 0 && retryInterval > 0)
                {
                    // calculate how many retries are left
                    retryCount = Math.Max(retryCount - actualRetryCount, 0);

                    // update message context
                    inputMessage.Context.Write(BtsProperties.RetryCount.Name, BtsProperties.RetryCount.Namespace, retryCount);
                    inputMessage.Context.Write(BtsProperties.RetryInterval.Name, BtsProperties.RetryInterval.Namespace, retryInterval);
                    TraceProvider.Logger.TraceInfo("Updated Message Retry context properties to: RetryCount = {0}, RetryInterval = {1}", retryCount, retryInterval);
                }
                else
                {
                    TraceProvider.Logger.TraceInfo("Skipping SendPortRetry, RetryCount and/or RetryInterval not specified: RetryCount = {0}, RetryInterval = {1}", retryCount, retryInterval);
                }
            }
            else
            {
                TraceProvider.Logger.TraceInfo("Skipping SendPortRetry, message already has context properties: RetryCount = {0}, RetryInterval = {1}", retryCount, retryInterval);
            }

            // return original message
            return inputMessage;
        }

        #endregion BasePipelineComponent Members
    }
}