using GT.BizTalk.Framework.Core.Tracing;
using GT.BizTalk.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System.Collections.Generic;

namespace GT.BizTalk.Framework.PipelineComponents
{
    /// <summary>
    /// Pipeline component to enable request to be routed back to caller as response.
    ///
    /// The pipeline component can be placed into any receive or send
    /// pipeline stage.
    /// </summary>
    [System.Runtime.InteropServices.Guid(COMPONENT_GUID)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Any)]
    public class RequestResponse : BasePipelineComponent, IBaseComponent
    {
        #region Constants

        private const string COMPONENT_GUID = "04CA64A5-5BF9-44E5-83E9-37242AE61048";

        #endregion Constants

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public RequestResponse()
            : base(Resources.ResourceManager
            , Resources.RequestResponseName
            , Resources.RequestResponseDescription
            , Resources.RequestResponseVersion
            , Resources.RequestResponseIcon)
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
            if (inputMessage.BodyPart != null)
            {
                TraceProvider.Logger.TraceInfo("Routing inbound request message back to sender");
                BTS.RouteDirectToTP RouteDirectToTP = new BTS.RouteDirectToTP();
                BTS.IsRequestResponse IsRequestResponse = new BTS.IsRequestResponse();

                inputMessage.Context.Promote(RouteDirectToTP.Name.Name, RouteDirectToTP.Name.Namespace, true);
                inputMessage.Context.Promote(IsRequestResponse.Name.Name, IsRequestResponse.Name.Namespace, true);
            }
            else
            {
                TraceProvider.Logger.TraceInfo("Message has no body part, exiting");
            }
            // return original message
            return inputMessage;
        }

        #endregion BasePipelineComponent Members
    }
}