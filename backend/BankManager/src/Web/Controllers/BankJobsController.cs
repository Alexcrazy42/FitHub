using FitHub.BankManager.Application.Payments;
using FitHub.BankManager.Web.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.BankManager.Web.Controllers;

[ApiController]
[Route("api/v1/bank/jobs")]
public class BankJobsController : ControllerBase
{
    private const int OutboxBatchSize = 50;

    private readonly IBankManagerOutboxPublisherService outboxPublisherService;

    public BankJobsController(IBankManagerOutboxPublisherService outboxPublisherService)
    {
        this.outboxPublisherService = outboxPublisherService;
    }

    [HttpPost("outbox/publish")]
    public async Task<PublishOutboxMessagesResponse> PublishOutboxAsync(CancellationToken ct)
    {
        var result = await outboxPublisherService.PublishPendingAsync(OutboxBatchSize, ct);
        return new PublishOutboxMessagesResponse(result.PublishedCount, result.FailedCount);
    }
}
