using GT.BizTalk.Framework.BizTalk.Serialization;
using Microsoft.BizTalk.Message.Interop;
using System;

namespace GT.BizTalk.Framework.PipelineComponents
{
    public static class ContextExtensions
    {
        public static bool TryRead(this IBaseMessageContext ctx, ContextProperty property, out object val)
        {
            return ((val = ctx.Read(property.Name, property.Namespace)) != null);
        }

        public static void Promote(this IBaseMessageContext ctx, ContextProperty property, object val)
        {
            ctx.Promote(property.Name, property.Namespace, val);
        }

        public static void Write(this IBaseMessageContext ctx, ContextProperty property, object val)
        {
            ctx.Write(property.Name, property.Namespace, val);
        }

        public static void Copy(this IBaseMessageContext ctx, ContextProperty source, ContextProperty destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            object sourceValue;

            if (ctx.TryRead(source, out sourceValue))
            {
                throw new InvalidOperationException("Could not find the specified source property in BizTalk context.");
            }

            ctx.Promote(destination, sourceValue);
        }
    }
}