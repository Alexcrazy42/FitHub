using FitHub.Common.Extensions.Configuration;
using FitHub.Common.Utilities.System;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;

namespace FitHub.Common.Telemetry.Extensions;

/// <summary>
/// Настройки распределённого трейсинга (секция <see cref="SectionName"/> в конфигурации).
/// </summary>
public sealed class TracingOptions : IHaveConfigSection
{
    public static string SectionName => "Telemetry";

    public string? ServiceName { get; set; }
    public bool EnableAspNetCore { get; set; } = true;
    public bool EnableHttpClient { get; set; } = true;
    public bool EnableEntityFramework { get; set; } = true;
    public bool EnableSqlClient { get; set; } = false;
    public bool EnableRabbitMq { get; set; } = true;
    public Action<TracerProviderBuilder>? ConfigureCollectors { get; set; }

    public Uri? Endpoint { get; set; }

    public OtlpExportProtocol? Protocol { get; set; }

    public string RequiredServiceName => ServiceName.Required();
}
