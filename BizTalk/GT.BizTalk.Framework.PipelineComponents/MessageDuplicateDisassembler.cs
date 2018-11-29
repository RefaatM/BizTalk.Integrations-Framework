using DE.DAXFSA.Framework.BizTalk;
using DE.DAXFSA.Framework.BizTalk.BRE;
using DE.DAXFSA.Framework.BizTalk.Tracing;
using DE.DAXFSA.Framework.Core.Tracing;
using DE.DAXFSA.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.RuleEngine;
using System;
using System.Collections.Generic;
using ECOMMsg = DE.DAXFSA.Framework.BizTalk.Message;

namespace DE.DAXFSA.Framework.PipelineComponents
{
    /// <summary>
    /// Disassembler pipeline component to duplicate original message and alter context property
    /// </summary>
    [System.Runtime.InteropServices.Guid(COMPONENT_GUID)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    public sealed class MessageDuplicateDisassembler : BasePipelineComponent, IDisassemblerComponent, IProbeMessage
    {
        #region Constants

        private const string COMPONENT_GUID = "0D690A41-A116-4095-9EF2-1C7DA0E6DE52";
        private const string POLICY_NAME_PROP_NAME = "PolicyName";
        private const string MAJOR_REV_PROP_NAME = "MajorRevision";
        private const string MINOR_REV_PROP_NAME = "MinorRevision";
        private const string DEEP_CLONE_PROP_NAME = "DeepCloneMessage";
        private const string CLONE_ITINERARY_PROP_NAME = "CloneItinerary";
        private const string CLONE_MSG_COUNT_PROP_NAME = "CloneMessageCount";

        #endregion Constants

        #region Fields

        // BizTalk Xml dissassembler
        private XmlDasmComp disassembler = new XmlDasmComp();

        private Queue<IBaseMessage> outMsgQueue = new Queue<IBaseMessage>();

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public MessageDuplicateDisassembler()
            : base(Resources.ResourceManager,
                Resources.MessageDuplicateDisassemblerName,
                Resources.MessageDuplicateDisassemblerDescription,
                Resources.MessageDuplicateDisassemblerVersion,
                Resources.MessageDuplicateDisassemblerIcon)
        {
            this.disassembler.RecoverableInterchangeProcessing = true;
        }

        #endregion Constructor

        #region Design-time properties

        /// <summary>
        /// Gets/sets the policy name.
        /// </summary>
        [BtsPropertyName("PolicyNamePropertyName")]
        [BtsDescription("PolicyNamePropertyDescription")]
        public string PolicyName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the policy major revision number.
        /// </summary>
        [BtsPropertyName("PolicyMajorRevisionPropertyName")]
        [BtsDescription("PolicyMajorRevisionPropertyDescription")]
        public int MajorRevision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the policy minor revision number.
        /// </summary>
        [BtsPropertyName("PolicyMinorRevisionPropertyName")]
        [BtsDescription("PolicyMinorRevisionPropertyDescription")]
        public int MinorRevision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the policy minor revision number.
        /// </summary>
        [BtsPropertyName("CloneMessageCountPropertyName")]
        [BtsDescription("CloneMessageCountPropertyDescription")]
        public int CloneMessageCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the flag to indicate if the message shall be clone with all context property.
        /// </summary>
        [BtsPropertyName("DeepCloneMessagePropertyName")]
        [BtsDescription("DeepCloneMessagePropertyDescription")]
        public bool DeepCloneMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets the flag to indicate if the message shall be clone with all context property.
        /// </summary>
        [BtsPropertyName("CloneItineraryPropertyName")]
        [BtsDescription("CloneItineraryPropertyDescription")]
        public bool CloneItinerary
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
            this.PolicyName = this.ReadPropertyValue<string>(propertyBag, POLICY_NAME_PROP_NAME, this.PolicyName);
            this.MajorRevision = this.ReadPropertyValue<int>(propertyBag, MAJOR_REV_PROP_NAME, this.MajorRevision);
            this.MinorRevision = this.ReadPropertyValue<int>(propertyBag, MINOR_REV_PROP_NAME, this.MinorRevision);
            this.DeepCloneMessage = this.ReadPropertyValue<bool>(propertyBag, DEEP_CLONE_PROP_NAME, this.DeepCloneMessage);
            this.CloneItinerary = this.ReadPropertyValue<bool>(propertyBag, CLONE_ITINERARY_PROP_NAME, this.CloneItinerary);
            this.CloneMessageCount = this.ReadPropertyValue<int>(propertyBag, CLONE_MSG_COUNT_PROP_NAME, this.CloneMessageCount);
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected override void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            this.WritePropertyValue(propertyBag, POLICY_NAME_PROP_NAME, this.PolicyName);
            this.WritePropertyValue(propertyBag, MAJOR_REV_PROP_NAME, this.MajorRevision);
            this.WritePropertyValue(propertyBag, MINOR_REV_PROP_NAME, this.MinorRevision);
            this.WritePropertyValue(propertyBag, DEEP_CLONE_PROP_NAME, this.DeepCloneMessage);
            this.WritePropertyValue(propertyBag, CLONE_ITINERARY_PROP_NAME, this.CloneItinerary);
            this.WritePropertyValue(propertyBag, CLONE_MSG_COUNT_PROP_NAME, this.CloneMessageCount);
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

        #endregion BasePipelineComponent Members

        #region IDisassemblerComponent Members

        /// <summary>
        /// Performs the disassembling of an incoming document.
        /// </summary>
        /// <param name="pipelineContext">The IPipelineContext containing the current pipeline context.</param>
        /// <param name="inputMessage">The IBaseMessage containing the message to be disassembled.</param>
        public void Disassemble(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            var callToken = TraceProvider.Logger.TraceIn(this.Name);
            try
            {
                // delegate work to the BizTalk Xml Dissassembler
                TraceProvider.Logger.TraceInfo("Delegating call to Microsoft.BizTalk.Component.XmlDasmComp.Disassemble() method");
                this.disassembler.Disassemble(pipelineContext, inputMessage);

                var outMsg1 = this.disassembler.GetNext(pipelineContext);
                outMsg1.Context.Write("ReplicationIndex", "https://DE.DAXFSA.Framework.Properties.ESBGlobalProperties", 1);

                outMsgQueue.Enqueue(outMsg1);

                if (CloneMessageCount > 0)
                {
                    TraceProvider.Logger.TraceInfo(string.Format("Component [{0}] is Enabled - returning {1} messages", this.Name, CloneMessageCount + 1));
                    for (int i = 0; i < CloneMessageCount; i++)
                    {
                        var outMsgX = this.DeepCloneMessage
                                            ? ECOMMsg.MessageHelper.DeepCloneMessage(pipelineContext, outMsg1, this.CloneItinerary)
                                            : ECOMMsg.MessageHelper.ShallowCloneMessage(pipelineContext, outMsg1);
                        outMsgX.Context.Write("ReplicationIndex", "https://DE.DAXFSA.Framework.Properties.ESBGlobalProperties", (i + 2));

                        outMsgQueue.Enqueue(outMsgX);
                    }
                }
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
                TraceProvider.Logger.TraceOut(callToken);
            }
        }

        /// <summary>
        /// Gets the next message from the message set resulting from the disassembler execution.
        /// </summary>
        /// <param name="pipelineContext">The IPipelineContext containing the current pipeline context.</param>
        /// <returns>
        /// A pointer to the IBaseMessage containing the next message from the disassembled document.
        /// Returns NULL if there are no more messages left.
        /// </returns>
        public IBaseMessage GetNext(IPipelineContext pipelineContext)
        {
            var callToken = TraceProvider.Logger.TraceIn(this.Name);
            try
            {
                if (outMsgQueue.Count > 0)
                {
                    var outMsg = outMsgQueue.Dequeue();

                    TraceProvider.Logger.TraceInfo("Using BRE policy {0} {1}.{2} to resolve IBaseMessage.", this.PolicyName, this.MajorRevision, this.MinorRevision);
                    // resolve the context properties
                    MessageContextAccessorResolver resolver = this.Resolve(outMsg);
                    if (resolver.UpdateMessageContext == true)
                    {
                        // update message context
                        resolver.ApplyMessageContextUpdates(pipelineContext, outMsg, true);
                    }

                    pipelineContext.ResourceTracker.AddResource(outMsg);
                    return outMsg;
                }
                else
                    return null;
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
                TraceProvider.Logger.TraceOut(callToken);
            }
        }

        #endregion IDisassemblerComponent Members

        #region IProbeMessage Members

        /// <summary>
        /// Checks if the incoming message is in a recognizable format.
        /// </summary>
        /// <param name="pipelineContext">The IPipelineContext containing the current pipeline context.</param>
        /// <param name="inputMessage">The incoming message.</param>
        /// <returns>true if the format was recognized; otherwise, false.</returns>
        public bool Probe(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            // delegate call to Flat File disassembler
            TraceProvider.Logger.TraceInfo("Delegating call to FlatFileDissassembler.Probe method");
            return this.disassembler.Probe(pipelineContext, inputMessage);
        }

        #endregion IProbeMessage Members

        #region Helpers

        /// <summary>
        /// Executes the specified resolution policy and returns the results
        /// in an instance of a MessageContextAccessorResolver object.
        /// </summary>
        /// <param name="message">The IBaseMessage instace.</param>
        /// <returns>MessageContextAccessorResolver instance containing the resolution results.</returns>
        public MessageContextAccessorResolver Resolve(IBaseMessage message)
        {
            // extract context properties
            Dictionary<string, object> contextProperties = new Dictionary<string, object>();
            message.Context.ReadAll(contextProperties);
            for (int p = 0; p < message.PartCount; p++)
            {
                string partName = string.Empty;
                var part = message.GetPartByIndex(p, out partName);
                part.PartProperties.ReadAll(contextProperties);
            }

            // create a fact instance
            MessageContextAccessorResolver fact = new MessageContextAccessorResolver(contextProperties);

            if (string.IsNullOrEmpty(this.PolicyName))
                return fact;

            // execute the schema resolution policy
            using (Policy policy = new Policy(this.PolicyName, this.MajorRevision, this.MinorRevision))
            {
                object[] facts = { fact };
                BRETrackingInterceptor interceptor = new BRETrackingInterceptor();
                policy.Execute(facts, interceptor);
                return fact;
            }
        }

        #endregion Helpers
    }
}