using FitHub.Common.Logging;
using FitHub.HostJobs;
using HostOptions = FitHub.HostJobs.HostOptions;

var host = Host.CreateDefaultBuilder(args)
    .UseCommonLogger<HostOptions>()
    .ConfigureServices((ctx, services) =>
    {
        services.AddServicesForBackground(ctx.Configuration);
    })
    .Build();

host.Run();
