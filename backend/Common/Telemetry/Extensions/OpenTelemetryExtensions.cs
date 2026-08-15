using FitHub.Common.Telemetry.Propagations;
using FitHub.Common.Utilities.System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sdk = OpenTelemetry.Sdk;
using FitHub.Common.Extensions.Configuration;

namespace FitHub.Common.Telemetry.Extensions;

public static class OpenTelemetryExtensions
{
    /// <summary>
    /// Регистрирует распределённый трейсинг: <see cref="AddBindedOptions{T}"/> для <see cref="TracingOptions"/>, затем чтение секции через <see cref="ConfigurationExtensions.GetRequiredOptions{T}"/> и опционально <paramref name="configureTracingOptions"/>.
    /// </summary>
    /// <remarks>Делегат <paramref name="configureTracingOptions"/> влияет только на регистрацию OpenTelemetry; <see cref="Microsoft.Extensions.Options.IOptions{TOptions}"/> остаётся привязанным к конфигурации без этого шага.</remarks>
    public static IServiceCollection AddCommonTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<TracingOptions>? configureTracingOptions = null,
        Action<MeterProviderBuilder>? configureMetrics = null,
        Action<ResourceBuilder>? configureResources = null)
    {
        services.AddBindedOptions<TracingOptions>();
        var options = configuration.GetRequiredOptions<TracingOptions>();
        configureTracingOptions?.Invoke(options);
        services.AddCommonHealthChecks();
        return AddTelemetry(services, configuration, options, configureMetrics, configureResources);
    }

    public static IServiceCollection AddCommonTelemetry<TDataContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<TracingOptions>? configureTracingOptions = null,
        Action<MeterProviderBuilder>? configureMetrics = null,
        Action<ResourceBuilder>? configureResources = null)
        where TDataContext : DbContext
    {
        services.AddBindedOptions<TracingOptions>();
        var options = configuration.GetRequiredOptions<TracingOptions>();
        configureTracingOptions?.Invoke(options);
        services.AddCommonHealthChecksWith<TDataContext>();
        return AddTelemetry(services, configuration, options, configureMetrics, configureResources);
    }

    private static IServiceCollection AddTelemetry(IServiceCollection services,
        IConfiguration configuration,
        TracingOptions options,
        Action<MeterProviderBuilder>? configureMetrics = null,
        Action<ResourceBuilder>? configureResources = null)
    {
        Sdk.SetDefaultTextMapPropagator(
            new CompositeTextMapPropagator([
                new TraceContextPropagator(), // https://w3c.github.io/trace-context/
                new BaggagePropagator() // https://w3c.github.io/baggage/
            ]));

        // У HostDefault и WebHostDefaults ключ одинаковый - "environment", справедливо как минимум для .net 9 и .net 10
        var environmentName = configuration[HostDefaults.EnvironmentKey].Required();

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(options.RequiredServiceName)
                .AddAttributes(new Dictionary<string, object>
                {
                    // These attributes will be automatically converted to labels in Prometheus:
                    // deployment.environment -> deployment_environment
                    // service.name -> service_name
                    ["environment"] = environmentName.ToLowerInvariant()
                }))
            .WithTracing(tracing =>
            {
                tracing.AddSource(TracingConst.ActivitySourceName);

                if (options.EnableAspNetCore)
                {
                    tracing.AddAspNetCoreInstrumentation(opts =>
                    {
                        opts.RecordException = true;
                        opts.Filter = (HttpContext httpContext) =>
                            !httpContext.Request.Path.StartsWithSegments("/health");
                    });
                }

                if (options.EnableHttpClient)
                {
                    tracing.AddHttpClientInstrumentation(opts =>
                    {
                        opts.RecordException = true;
                    });
                }

                if (options.EnableEntityFramework)
                {
                    tracing.AddEntityFrameworkCoreInstrumentation();
                }

                tracing.AddSqlClientInstrumentation();

                if (options.EnableRabbitMq)
                {
                    tracing.AddRabbitMQInstrumentation();
                }

                options.ConfigureExporters?.Invoke(tracing);
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddSqlClientInstrumentation();

                string[] diagnosticsMetrics =
                [
                    "System.Net.Http",
                    "System.Net.NameResolution",
                    "System.Threading",
                    "System.Runtime",
                    "Microsoft.EntityFrameworkCore"
                ];

                metrics.AddMeter(diagnosticsMetrics);

                // Configure Prometheus exporter
                metrics.AddPrometheusExporter(configure =>
                {
                    configure.ScrapeEndpointPath = "/metrics";
                    configure.ScrapeResponseCacheDurationMilliseconds = 1000;
                });

                configureMetrics?.Invoke(metrics);
            });


        services.AddSingleton<ITraceContextManager, TraceContextManager>();

        return services;
    }

    /// <summary>
    /// Использовать стандартную телеметрию
    /// </summary>
    public static void UseCommonTelemetry(this IEndpointRouteBuilder endpoints)
    {
        endpoints.UseCommonHealthChecks();
        endpoints.UseCommonMetrics();
    }

    /// <summary>
    /// Использовать стандартные метрики
    /// </summary>
    private static void UseCommonMetrics(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPrometheusScrapingEndpoint();
    }
}
