using FitHub.Application.EmailNotifications;

namespace FitHub.HostJobs.Workers.EmailNotifications;

/// <summary>
/// Worker по отправке email уведомлений
/// </summary>
public class EmailNotificationSenderWorker : BackgroundService
{
    private readonly IServiceProvider provider;
    private readonly ILogger<EmailNotificationSenderWorker> logger;

    public EmailNotificationSenderWorker(IServiceProvider provider, ILogger<EmailNotificationSenderWorker> logger)
    {
        this.provider = provider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{WorkerName} starting", GetType().Name);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = provider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IEmailNotificationService>();
                await service.HandleFromWorker(stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError($"Error throw from worker {GetType().Name}", e);
            }
        }
        logger.LogInformation("{WorkerName} stopping", GetType().Name);
    }
}
