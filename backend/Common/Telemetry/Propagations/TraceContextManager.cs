using System.Diagnostics;

namespace FitHub.Common.Telemetry.Propagations;

public class TraceContextManager : ITraceContextManager
{
    private static readonly ActivitySource ActivitySource =
        new ActivitySource(TracingConst.ActivitySourceName);

    public IDisposable CreateNewContext(string operationName, ActivityKind activityKind = ActivityKind.Internal)
    {
        var activity = (IDisposable?)ActivitySource.StartActivity(operationName, activityKind);
        return activity
               ?? NoOpDisposable.Instance;
    }

    public IDisposable CreateFromTraceParent(string traceParent, string operationName, ActivityKind activityKind = ActivityKind.Internal)
    {
        // isRemote - указание, что trace пришел извне (например: outbox)
        if (!ActivityContext.TryParse(traceParent, traceState: null, isRemote: true, out var parentContext))
        {
            return CreateNewContext(operationName, activityKind);
        }

        var activity = (IDisposable?)ActivitySource.StartActivity(operationName, activityKind, parentContext);
        return activity
               ?? NoOpDisposable.Instance;
    }

    public TraceContext? GetCurrentContext()
    {
        var activity = Activity.Current;
        if (activity == null)
        {
            return null;
        }

        return new TraceContext
        {
            TraceId = activity.TraceId.ToString(),
            SpanId = activity.SpanId.ToString(),
            TraceParent = activity.Id
        };
    }

    public string? GetCurrentTraceId() => Activity.Current?.TraceId.ToString();
}
