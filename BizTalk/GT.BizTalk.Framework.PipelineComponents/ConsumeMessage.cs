using DE.DAXFSA.Framework.Core.Tracing;
using DE.DAXFSA.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System.Collections.Generic;

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
    public class ConsumeMessage : BasePipelineComponent, IBaseComponent
    {
        #region Constants

        private const string COMPONENT_GUID = "DF4F067B-C107-4062-B72D-C9F5BA76F089";

        #endregion Constants

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public ConsumeMessage()
            : base(Resources.ResourceManager
            , Resources.ConsumeMessageName
            , Resources.ConsumeMessageDescription
            , Resources.ConsumeMessageVersion
            , Resources.ConsumeMessageIcon)
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
            TraceProvider.Logger.TraceInfo("Consuming message....Done");

            return null;
        }

        #endregion BasePipelineComponent Members
    }
}