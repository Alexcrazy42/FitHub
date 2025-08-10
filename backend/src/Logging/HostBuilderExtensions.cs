using FitHub.Common.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Http.BatchFormatters;

namespace FitHub.Common.Logging;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseCommonLogger<THostOptions>(this IHostBuilder builder,
        Action<HostBuilderContext, LoggerConfiguration>? configure = null)
        where THostOptions : class, IHostOptions
        => builder.UseSerilog((context, configuration) =>
        {
            var hostOptions = context.Configuration.GetRequiredOptions<THostOptions>();
            var logstashOptions = context.Configuration.GetRequiredOptions<LogstashOptions>();
            var isDevelopment = context.HostingEnvironment.IsDevelopment();

            configuration
                .ConfigureEnrichers()
                .ConfigureSinks(hostOptions, logstashOptions, isDevelopment)
                // Последним делом применяем параметры из конфига,
                // чтобы потребители могли переопределять поведение
                .ReadFrom.Configuration(context.Configuration);
            configure?.Invoke(context, configuration);
        });


    public static void UseCommonRequestLogging(this IApplicationBuilder builder)
    {
        builder.UseSerilogRequestLogging();
    }

    private static LoggerConfiguration ConfigureEnrichers(this LoggerConfiguration configuration)
        => configuration
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName();

    private static LoggerConfiguration ConfigureSinks(
        this LoggerConfiguration configuration,
        IHostOptions hostOptions,
        LogstashOptions logstashOptions,
        bool isDevelopment)
        => isDevelopment
            ? configuration
                .WriteTo.Async(configure => configure.Console())
            : configuration
                .WriteTo.Async(configure => configure.Console())
                .WriteTo.Async(configure => configure
                    .DurableHttpUsingFileSizeRolledBuffers(
                        requestUri: logstashOptions.RequiredHttpAddress,
                        batchFormatter: new ArrayBatchFormatter(),
                        textFormatter: new JsonLogFormatter(hostOptions.RequiredName)));
}
