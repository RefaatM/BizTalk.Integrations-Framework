using Microsoft.BizTalk.Bam.EventObservation;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System.Collections.Generic;

namespace DE.DAXFSA.Framework.BizTalk.BAM
{
    public interface IRelatedBAMActivitiesTracker
    {
        void Execute(IPipelineContext context
                            , IBaseMessage msg
                            , ref EventStream eventStream
                            , string PrimaryActivityName
                            , string PrimaryActivityID
                            , string RelatedActivityName
                            , List<BAMPropertyEventSource> BAMPropertyEventSources
                            , Dictionary<string, object> BAMPropertyEventSourceValues
                            , List<BAMPropertyEventSource> PostprocessXPathEventSources
                            , bool EnableContinuation
                            , bool UseContinuation
            );

        //public List<BAMPropertyEventSource> GetB
    }
}