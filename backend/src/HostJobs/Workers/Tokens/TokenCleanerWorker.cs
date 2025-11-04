using FitHub.Application.Users;

namespace FitHub.HostJobs.Workers.Tokens;

public class TokenCleanerWorker : BackgroundService
{
    private readonly IServiceProvider provider;
    private readonly ILogger<TokenCleanerWorker> logger;

    public TokenCleanerWorker(IServiceProvider provider, ILogger<TokenCleanerWorker> logger)
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
                var repository = scope.ServiceProvider.GetRequiredService<ITokenRepository>();
                await repository.ClearInactiveTokens(stoppingToken);
                await Task.Delay(TimeSpan.FromHours(10), stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError($"Error throw from worker {GetType().Name}", e);
            }
        }
        logger.LogInformation("{WorkerName} stopping", GetType().Name);
    }
}
