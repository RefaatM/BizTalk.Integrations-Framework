using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;

using Microsoft.Practices.Modeling.Common;
using Microsoft.Practices.Modeling.Common.Design;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Modeling.Services.Design;
using Microsoft.Practices.Services.Extenders;
using Microsoft.Practices.Services.ItineraryDsl;
using Microsoft.VisualStudio.Modeling;
using System.Collections.Generic;

namespace GT.BizTalk.Framework.ESBExtenders
{
    [Serializable]
    [ObjectExtender(typeof(ItineraryService))]
    public class AsyncRouterExtender : ItineraryServiceExtenderBase, IMessagingItineraryServiceExtender, IItineraryServiceExtender
    {
        #region Fields
        private EsbContainer container;
        #endregion

        #region ItineraryServiceExtenderBase implementation
        [EditorOutputProperty("ServiceId", "ServiceId", new string[] { })]
        [Editor(typeof(BiztalkMessagingServiceNameEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(TypeConverter))]
        public override string ServiceName { get; set; }

        public override string ServiceType
        {
            get
            {
                return "Messaging";
            }
        }
        #endregion

        #region IMessagingItineraryServiceExtender implementation
        [Description("Specifies the ESB container name and handler for send or receive.")]
        [Browsable(true)]
        [Editor(typeof(EsbContainerEditor), typeof(UITypeEditor))]
        [Category("Extender Settings")]
        [DisplayName("Container")]
        [ReadOnly(false)]
        public EsbContainer Container
        {
            get
            {
                return this.container;
            }
            set
            {
                if (this.container != null && (value == null || string.IsNullOrEmpty(value.Name)))
                    ItineraryDslUtility.RemoveHandler(this.ModelElement);
                this.container = value;
                if (this.container == null || this.container.Service == null)
                    return;
                ItineraryDslUtility.AddHandler(this.ModelElement, this.GetHandlers());
            }
        }

        private LinkedElementCollection<ItineraryService> GetHandlers()
        {
            if (this.container.Handler != HandlerType.receiveTransmit && this.container.Handler != HandlerType.sendTransmit)
                return ((EsbService)this.container.Service).ReceiveHandlers;
            else
                return ((EsbService)this.container.Service).SendHandlers;
        }
        #endregion

        #region Custom properties

        [Category(AsyncRouterExtensionProvider.ExtensionProviderPropertyCategory),
            Description("Determine if the message should be a direct synchronous ACK"),
            DisplayName("IsDirectSynchronousACK"),
            ReadOnly(false),
            Browsable(true)]
        [XmlElement]
        [PersistInPropertyBag] // NOTE: This property will be persisted in the service property bag
        public bool IsDirectSynchronousACK { get; set; }

        #endregion
    }
}
