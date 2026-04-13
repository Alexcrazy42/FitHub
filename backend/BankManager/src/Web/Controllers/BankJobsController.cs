using FitHub.BankManager.Application.Payments;
using FitHub.BankManager.Web.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.BankManager.Web.Controllers;

[ApiController]
[Route("api/v1/bank/jobs")]
public class BankJobsController : ControllerBase
{
    private const int OutboxBatchSize = 50;
    private const int PaymentIntentBatchSize = 50;
    private static readonly TimeSpan AutoCompleteDelay = TimeSpan.FromSeconds(20);

    private readonly IBankManagerOutboxPublisherService outboxPublisherService;
    private readonly IPaymentIntentService paymentIntentService;

    public BankJobsController(
        IBankManagerOutboxPublisherService outboxPublisherService,
        IPaymentIntentService paymentIntentService)
    {
        this.outboxPublisherService = outboxPublisherService;
        this.paymentIntentService = paymentIntentService;
    }

    [HttpPost("outbox/publish")]
    public async Task<PublishOutboxMessagesResponse> PublishOutboxAsync(CancellationToken ct)
    {
        var result = await outboxPublisherService.PublishPendingAsync(OutboxBatchSize, ct);
        return new PublishOutboxMessagesResponse(result.PublishedCount, result.FailedCount);
    }

    [HttpPost("payment-intents/complete-pending")]
    public async Task<CompletePendingPaymentIntentsResponse> CompletePendingPaymentIntentsAsync(CancellationToken ct)
    {
        var result = await paymentIntentService.CompletePendingAsync(
            DateTimeOffset.UtcNow.Subtract(AutoCompleteDelay),
            PaymentIntentBatchSize,
            ct);

        return new CompletePendingPaymentIntentsResponse(result.CompletedCount);
    }
}
