using FitHub.BankManager.Clients.Payment;

namespace FitHub.Jobs.Workers;

public sealed class PaymentIntentAutoCompleteWorker : BackgroundService
{
    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(2);

    private readonly IServiceProvider provider;
    private readonly ILogger<PaymentIntentAutoCompleteWorker> logger;

    public PaymentIntentAutoCompleteWorker(IServiceProvider provider, ILogger<PaymentIntentAutoCompleteWorker> logger)
    {
        this.provider = provider;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = provider.CreateScope();
                var paymentClient = scope.ServiceProvider.GetRequiredService<IBankManagerPaymentClient>();
                var result = await paymentClient.CompletePendingPaymentIntentsAsync(stoppingToken);

                if (result.CompletedCount > 0)
                {
                    logger.LogInformation("Auto-completed BankManager payment intents: {CompletedCount}", result.CompletedCount);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to request BankManager payment intent auto-complete.");
            }

            await Task.Delay(Delay, stoppingToken);
        }
    }
}
