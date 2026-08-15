using System.Text.Json;
using FitHub.Common.Json;
using FitHub.Common.Utilities.System.Linux;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FitHub.Common.Telemetry.Extensions;

/// <summary>
/// Реестр хч
/// </summary>
public static class HealthCheckRegistry
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = CommonJsonSerializerOptions.Create();

    /// <summary>
    /// Добавить стандартные хч
    /// </summary>
    /// <remarks>Список хч:<br />
    /// 1) Потребление ресурсов пк</remarks>
    public static void AddCommonHealthChecks(this IServiceCollection services, Action<IHealthChecksBuilder>? configure = null)
    {
        // HACKish: На линуксе падает из-за https://github.com/dotnet/extensions/issues/6832
        // поэтому выбираем парсер вручную
        if (OperatingSystem.IsLinux() && LinuxOperatingSystem.GetCgroupVersion() == CgroupVersion.V2)
        {
            services.Add(ServiceDescriptor.Singleton(
                service: Type.GetType("Microsoft.Extensions.Diagnostics.ResourceMonitoring.Linux.ILinuxUtilizationParser, Microsoft.Extensions.Diagnostics.ResourceMonitoring")!,
                implementationType: Type.GetType("Microsoft.Extensions.Diagnostics.ResourceMonitoring.Linux.LinuxUtilizationParserCgroupV2, Microsoft.Extensions.Diagnostics.ResourceMonitoring")!
            ));
        }

        var builder = services.AddHealthChecks()
            .AddResourceUtilizationHealthCheck();

        configure?.Invoke(builder);
    }

    /// <summary>
    /// Добавить стандартные хч
    /// </summary>
    /// <remarks>Список хч:<br />
    /// 1) Потребление ресурсов пк<br />
    /// 2) Контекст EF'а</remarks>
    public static void AddCommonHealthChecksWith<TDataContext>(this IServiceCollection services, Action<IHealthChecksBuilder>? configure = null)
        where TDataContext : DbContext
        => AddCommonHealthChecks(services, builder =>
        {
            builder.AddDbContextCheck<TDataContext>();
            configure?.Invoke(builder);
        });

    /// <summary>
    /// Использовать стандартные хч
    /// </summary>
    public static void UseCommonHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = WriteHealthCheckResponse });
    }

    private static Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsJsonAsync(report, JsonSerializerOptions);
    }
}
