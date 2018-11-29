using System.Drawing;

using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.Services.ItineraryDsl;

namespace GT.BizTalk.Framework.ESBExtenders
{
    [ExtensionProvider("6FEDCF98-EE5A-4B17-8BAA-187F014909D6", "AsyncRouter", "AsyncRouter Extender", typeof(ItineraryDslDomainModel))]
    [ItineraryExtensionProvider]
    public class AsyncRouterExtensionProvider : ExtensionProviderBase, IExtenderStyle
    {
        public AsyncRouterExtensionProvider()
            : base(typeof(AsyncRouterExtender))
        {
        }

        public Color FillColor
        {
            get
            {
                return Color.FromKnownColor(KnownColor.YellowGreen);
            }
        }
    }
}
