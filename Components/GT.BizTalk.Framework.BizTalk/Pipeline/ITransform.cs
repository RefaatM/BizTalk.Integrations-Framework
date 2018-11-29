using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System.Collections.Generic;

namespace DE.DAXFSA.Framework.BizTalk.Pipeline
{
    public interface ITransform
    {
        Dictionary<string, object> ResolveMapping(IPipelineContext pc, IBaseMessage inmsg, string input);

        //Dictionary<string, object> ResolveMapping(string input);
        string Transform(string input);

        string DestinationMessage { get; }
        string SourceMessage { get; }
        string Version { get; }
    }
}