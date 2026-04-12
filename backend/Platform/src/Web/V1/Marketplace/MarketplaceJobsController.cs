using FitHub.Application.Marketplace;
using FitHub.Application.Outbox;
using FitHub.Common.Entities;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Marketplace;
using FitHub.Domain.Marketplace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Marketplace;

[ApiController]
[AllowAnonymous]
public class MarketplaceJobsController : ControllerBase
{
    private const int OutboxBatchSize = 50;

    private readonly IMarketplaceCheckoutService checkoutService;
    private readonly IMarketplacePaymentService paymentService;
    private readonly IOutboxPublisherService outboxPublisherService;

    public MarketplaceJobsController(
        IMarketplaceCheckoutService checkoutService,
        IMarketplacePaymentService paymentService,
        IOutboxPublisherService outboxPublisherService)
    {
        this.checkoutService = checkoutService;
        this.paymentService = paymentService;
        this.outboxPublisherService = outboxPublisherService;
    }

    [HttpPost(ApiRoutesV1.MarketplaceJobsReleaseExpiredReservations)]
    public async Task<ReleaseExpiredStockReservationsResponse> ReleaseExpiredReservationsAsync(CancellationToken ct)
    {
        var releasedCount = await checkoutService.ReleaseExpiredReservationsAsync(DateTimeOffset.UtcNow, ct);
        return new ReleaseExpiredStockReservationsResponse(releasedCount);
    }

    [HttpPost(ApiRoutesV1.MarketplaceJobsApplyBankPaymentStatus)]
    public async Task ApplyBankPaymentStatusAsync([FromBody] ApplyBankPaymentStatusRequest? request, CancellationToken ct)
    {
        request = ValidationException.ThrowIfNull(request, "request cannot be null");

        await paymentService.ApplyBankPaymentStatusAsync(
            StockReservationId.Parse(request.ReservationId),
            request.PaymentIntentId,
            request.Status,
            request.Amount,
            request.Currency,
            request.FailureReason,
            ct);
    }

    [HttpPost(ApiRoutesV1.MarketplaceJobsPublishOutbox)]
    public async Task<PublishOutboxMessagesResponse> PublishOutboxAsync(CancellationToken ct)
    {
        var result = await outboxPublisherService.PublishPendingAsync(OutboxBatchSize, ct);
        return new PublishOutboxMessagesResponse(result.PublishedCount, result.FailedCount);
    }
}
