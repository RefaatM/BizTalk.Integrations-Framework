using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace GT.BizTalk.Framework.BizTalk.Pipeline
{
    public interface IIBaseMessageProcessor
    {
        IBaseMessage Execute(IPipelineContext pc, IBaseMessage inmsg, string objectArgument);

        string Description { get; }
        string Name { get; }
        string Version { get; }
    }
}