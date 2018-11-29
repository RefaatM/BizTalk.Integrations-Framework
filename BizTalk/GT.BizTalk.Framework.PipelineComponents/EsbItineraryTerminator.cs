using DE.DAXFSA.Framework.Core.Tracing;
using DE.DAXFSA.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Practices.ESB.Itinerary;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DE.DAXFSA.Framework.PipelineComponents
{
    /// <summary>
    /// Pipeline component that consumes the message.
    ///
    /// The pipeline component can be placed into any receive or send
    /// pipeline stage.
    /// </summary>
    [System.Runtime.InteropServices.Guid(COMPONENT_GUID)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Any)]
    public class EsbItineraryTerminator : BasePipelineComponent, IBaseComponent
    {
        #region Constants

        private const string COMPONENT_GUID = "DF4F067B-C107-4062-B72D-C9F5BA76F089";

        #endregion Constants

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public EsbItineraryTerminator()
            : base(Resources.ResourceManager
            , Resources.EsbItineraryTerminatorName
            , Resources.EsbItineraryTerminatorDescription
            , Resources.EsbItineraryTerminatorVersion
            , Resources.EsbItineraryTerminatorIcon)
        {
        }

        #endregion Constructor



        #region BasePipelineComponent Members

        /// <summary>
        /// Load component properties from a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="errorLog">Error log level</param>
        protected override void LoadProperties(IPropertyBag propertyBag, int errorLog)
        {
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected override void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
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
        /// Send request message back to caller as response
        /// </summary>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="inputMessage">Input message</param>
        /// <returns>Original input message</returns>
        protected override IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            var callToken = TraceProvider.Logger.TraceIn(this.Name);
            try
            {
                // delegate work to the BizTalk Xml Dissassembler
                TraceProvider.Logger.TraceInfo("Checking if itinerary should be terminated");
                var IsTerminateItinerary = inputMessage.Context.Read("IsTerminateItinerary", "https://DE.DAXFSA.Framework.Properties.ESBGlobalProperties");

                if (IsTerminateItinerary != null && (bool)IsTerminateItinerary)
                {
                    IItinerary itinerary = ItineraryOMFactory.Create(inputMessage);
                    if (itinerary != null)
                    {
                        MethodInfo CompleteItinerary_MI = itinerary.GetType().GetMethod("CompleteItinerary", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        if (CompleteItinerary_MI != null) CompleteItinerary_MI.Invoke(itinerary, new object[] { null });
                    }
                    return null;
                }
                return inputMessage;
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

        #endregion BasePipelineComponent Members
    }
}