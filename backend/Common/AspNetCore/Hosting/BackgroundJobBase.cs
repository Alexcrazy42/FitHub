using FitHub.Common.Telemetry.Propagations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FitHub.Common.AspNetCore.Hosting;

public abstract class BackgroundJobBase : BackgroundService
{
    private string JobName => GetType().Name;

    protected readonly ILogger<BackgroundJobBase> Logger;
    protected readonly IServiceProvider ServiceProvider;

    protected BackgroundJobBase(ILogger<BackgroundJobBase> logger,
        IServiceProvider serviceProvider)
    {
        Logger = logger;
        ServiceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("Starting background job {JobName}", JobName);

        while (!stoppingToken.IsCancellationRequested)
        {
            var traceContextManager = ServiceProvider.GetRequiredService<ITraceContextManager>();

            using (var context = traceContextManager.CreateNewContext(JobName))
            {
                await ExecuteOnceAsync(stoppingToken);
            }
        }

        Logger.LogInformation("Stopping background job {JobName}", JobName);
    }

    public abstract Task ExecuteOnceAsync(CancellationToken cancellationToken);
}
