namespace FitHub.Common.Telemetry.Propagations;

public class TraceContext
{
    public string? TraceId { get; set; }
    public string? SpanId { get; set; }
    public string? TraceParent { get; set; }
}
