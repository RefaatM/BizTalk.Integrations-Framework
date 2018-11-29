using GT.BizTalk.Framework.BizTalk;
using GT.BizTalk.Framework.BizTalk.Serialization;
using GT.BizTalk.Framework.Core.Tracing;
using GT.BizTalk.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System.Collections.Generic;
using System.ComponentModel;

namespace GT.BizTalk.Framework.PipelineComponents
{
    /// <summary>
    /// Pipeline component which can be placed into any receive or send
    /// pipeline stage and do an itinerary Configuration.
    /// </summary>
    [System.Runtime.InteropServices.Guid("244026D7-2FE4-48A5-9A8D-A79D83B17C67")]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Any)]
    public sealed class ItineraryConfigurator : BasePipelineComponent
    {
        #region Constants

        private const string PROPERTIES_PROP_NAME = "ItineraryProperties";
        private const string ITINERARY_PROP_NAME = "ItineraryHeader";
        private const string ITINERARY_PROP_NAMESPACE = "http://schemas.microsoft.biztalk.practices.esb.com/itinerary/system-properties";

        #endregion Constants

        #region Constructors

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public ItineraryConfigurator()
            : base(Resources.ResourceManager, Resources.ItineraryConfiguratorName, Resources.ItineraryConfiguratorDescription, Resources.ItineraryConfiguratorVersion, Resources.ItineraryConfiguratorIcon)
        {
            this.ItineraryProperties = new List<ItineraryProperty>();
        }

        #endregion Constructors

        #region Design-time Properties

        /// <summary>
        /// Gets o sets the Itinerary properties value collection.
        /// </summary>
        [BtsPropertyName("ItineraryConfiguratorPropertyName")]
        [BtsDescription("ItineraryConfiguratorPropertyDescription")]
        [Editor(typeof(System.ComponentModel.Design.CollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public List<ItineraryProperty> ItineraryProperties
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
            TraceProvider.Logger.TraceIn("Itinerary configurator Load Properties ..");
            string xml = this.ReadPropertyValue<string>(propertyBag, PROPERTIES_PROP_NAME, null);
            if (string.IsNullOrEmpty(xml) == false)
            {
                ItineraryPropertySerializer serializer = new ItineraryPropertySerializer();
                serializer.Deserialize(xml);
                this.ItineraryProperties = serializer.Properties;
            }
            TraceProvider.Logger.TraceOut("Itinerary configurator Load Properties ..");
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected override void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            TraceProvider.Logger.TraceIn("Itinerary configurator Save Properties ..");
     
            string xml = null;
            if (this.ItineraryProperties != null)
            {
                ItineraryPropertySerializer serializer = new ItineraryPropertySerializer(this.ItineraryProperties);
                xml = serializer.Serialize();
            }
            this.WritePropertyValue(propertyBag, PROPERTIES_PROP_NAME, xml);
            TraceProvider.Logger.TraceOut("Itinerary configurator Save Properties ..");
     
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
        /// Promotes/writes the specified set of properties into the message context.
        /// </summary>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="inputMessage">Input message</param>
        /// <returns>Original input message</returns>
        protected override IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            if (this.Enabled && this.ItineraryProperties != null && this.ItineraryProperties.Count > 0)
            {
                // gete the itenerary
                string itenrary = this.GetItinerary(inputMessage);
                if (!string.IsNullOrWhiteSpace(itenrary))
                {
                    TraceProvider.Logger.TraceInfo("Origianle Itinerary: {0} ", itenrary);
                    // iterate through all context properties promoting or writing
                    foreach (ItineraryProperty itineraryProperty in this.ItineraryProperties)
                    {
                        if (!string.IsNullOrWhiteSpace( itineraryProperty.Value))
                        {
                            TraceProvider.Logger.TraceInfo("Replacing Itinerary Holder: {0} with {1}", itineraryProperty.Name, itineraryProperty.Value);
                            itenrary = itenrary.Replace(itineraryProperty.Name, itineraryProperty.Value);
                        }
                    }
                    // update the itinerary in the context
                    TraceProvider.Logger.TraceInfo("Updated Itinerary: {0} ", itenrary);
                    UpdateItinerary(inputMessage, itenrary);
                }
                else
                {
                    TraceProvider.Logger.TraceInfo("Itinerary is empty");
                }
            }
            return inputMessage;
        }

        #endregion BasePipelineComponent Members

        #region Helpers

        /// <summary>
        ///
        /// </summary>
        /// <param name="inputMessage"></param>
        /// <param name="itenrary"></param>
        private void UpdateItinerary(IBaseMessage inputMessage, string itinrary)
        {
            inputMessage.Context.Write(ITINERARY_PROP_NAME, ITINERARY_PROP_NAMESPACE, itinrary);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="inputMessage"></param>
        /// <returns></returns>
        private string GetItinerary(IBaseMessage inputMessage)
        {
            return inputMessage.Context.Read<string>(ITINERARY_PROP_NAME, ITINERARY_PROP_NAMESPACE, null);
        }

        #endregion Helpers
    }
}