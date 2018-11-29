using System.Xml;

namespace GT.BizTalk.Framework.BizTalk
{
    /// <summary>
    /// BTS context properties.
    /// </summary>
    public static class BtsProperties
    {
        public static readonly XmlQualifiedName DocumentSpecName = new XMLNORM.DocumentSpecName().Name;
        public static readonly XmlQualifiedName SchemaStrongName = new BTS.SchemaStrongName().Name;
        public static readonly XmlQualifiedName MessageType = new BTS.MessageType().Name;
        public static readonly XmlQualifiedName InterchangeID = new BTS.InterchangeID().Name;
        public static readonly XmlQualifiedName InterchangeSequenceNumber = new BTS.InterchangeSequenceNumber().Name;
        public static readonly XmlQualifiedName MessageID = new BTS.MessageID().Name;
        public static readonly XmlQualifiedName InboundTransportType = new BTS.InboundTransportType().Name;
        public static readonly XmlQualifiedName OutboundTransportType = new BTS.OutboundTransportType().Name;
        public static readonly XmlQualifiedName OutboundTransportLocation = new BTS.OutboundTransportLocation().Name;
        public static readonly XmlQualifiedName ReceivePortName = new BTS.ReceivePortName().Name;
        public static readonly XmlQualifiedName RetryCount = new BTS.RetryCount().Name;
        public static readonly XmlQualifiedName RetryInterval = new BTS.RetryInterval().Name;
        public static readonly XmlQualifiedName ActualRetryCount = new XmlQualifiedName("ActualRetryCount", RetryCount.Namespace);
        public static readonly XmlQualifiedName SSOTicket = new BTS.SSOTicket().Name;
        public static readonly XmlQualifiedName ReceivedFileName = new FILE.ReceivedFileName().Name;
    }

    /// <summary>
    /// Dasm properties.
    /// </summary>
    public static class DasmProperties
    {
        public static readonly XmlQualifiedName DocumentSpecName = new XMLNORM.DocumentSpecName().Name;
    }

    /// <summary>
    /// Archiver pipeline component schema properties.
    /// </summary>
    public static class ArchiverProperties
    {
        public static readonly XmlQualifiedName Enabled = new GT.BizTalk.Framework.GlobalPropertySchemas.Pipelines.Archiver.Enabled().Name;
        public static readonly XmlQualifiedName ArchivePath = new GT.BizTalk.Framework.GlobalPropertySchemas.Pipelines.Archiver.ArchivePath().Name;
        public static readonly XmlQualifiedName ArchiveFileName = new GT.BizTalk.Framework.GlobalPropertySchemas.Pipelines.Archiver.ArchiveFileName().Name;
        public static readonly XmlQualifiedName Optimized = new GT.BizTalk.Framework.GlobalPropertySchemas.Pipelines.Archiver.Optimized().Name;
    }

    /// <summary>
    /// BAM Tracker pipeline component schema properties.
    /// </summary>
    public static class BAMTrackerProperties
    {
        public static readonly XmlQualifiedName Enabled = new GT.BizTalk.Framework.GlobalPropertySchemas.Pipelines.BAMTracker.Enabled().Name;
        public static readonly XmlQualifiedName ActivityName = new GT.BizTalk.Framework.GlobalPropertySchemas.Pipelines.BAMTracker.ActivityName().Name;
        public static readonly XmlQualifiedName EnableContinuation = new GT.BizTalk.Framework.GlobalPropertySchemas.Pipelines.BAMTracker.EnableContinuation().Name;
        public static readonly XmlQualifiedName UseContinuation = new GT.BizTalk.Framework.GlobalPropertySchemas.Pipelines.BAMTracker.UseContinuation().Name;
        public static readonly XmlQualifiedName EventSources = new GT.BizTalk.Framework.GlobalPropertySchemas.Pipelines.BAMTracker.EventSources().Name;
    }
}