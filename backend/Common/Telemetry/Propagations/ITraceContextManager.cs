using System.Diagnostics;

namespace FitHub.Common.Telemetry.Propagations;

public interface ITraceContextManager
{
    /// <summary>
    /// Создание нового контекста
    /// </summary>
    /// <returns></returns>
    IDisposable CreateNewContext(string operationName, ActivityKind activityKind = ActivityKind.Internal);

    /// <summary>
    /// Создание контекста из traceParent
    /// </summary>
    IDisposable CreateFromTraceParent(string traceParent, string operationName, ActivityKind activityKind = ActivityKind.Internal);

    /// <summary>
    /// Получение текущего контекста
    /// </summary>
    TraceContext? GetCurrentContext();

    /// <summary>
    /// Получение текущего traceId
    /// </summary>
    string? GetCurrentTraceId();
}
