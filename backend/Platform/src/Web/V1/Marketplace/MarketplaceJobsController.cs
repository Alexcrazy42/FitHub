using FitHub.Application.Marketplace;
using FitHub.Application.Marketplace.Deliveries;
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
    private const int DeliveryAssignmentBatchSize = 20;
    private static readonly TimeSpan DeliveryAssignmentAcceptanceTimeout = TimeSpan.FromSeconds(20);

    private readonly IMarketplaceCheckoutService checkoutService;
    private readonly IMarketplacePaymentService paymentService;
    private readonly IDeliveryAssignmentService deliveryAssignmentService;
    private readonly IOutboxPublisherService outboxPublisherService;

    public MarketplaceJobsController(
        IMarketplaceCheckoutService checkoutService,
        IMarketplacePaymentService paymentService,
        IDeliveryAssignmentService deliveryAssignmentService,
        IOutboxPublisherService outboxPublisherService)
    {
        this.checkoutService = checkoutService;
        this.paymentService = paymentService;
        this.deliveryAssignmentService = deliveryAssignmentService;
        this.outboxPublisherService = outboxPublisherService;
    }

    [HttpPost(ApiRoutesV1.MarketplaceJobsReleaseExpiredReservations)]
    public async Task<ReleaseExpiredStockReservationsResponse> ReleaseExpiredReservationsAsync(CancellationToken ct)
    {
        var releasedCount = await checkoutService.ReleaseExpiredReservationsAsync(DateTimeOffset.UtcNow, ct);
        return new ReleaseExpiredStockReservationsResponse(releasedCount);
    }

    [HttpPost(ApiRoutesV1.MarketplaceJobsApplyBankPaymentStatus)]
    public async Task ApplyBankPaymentStatusAsync([FromBody] ApplyBankPaymentStatusRequest request, CancellationToken ct)
    {
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

    [HttpPost(ApiRoutesV1.MarketplaceJobsAutoAssignDeliveries)]
    public async Task<AutoAssignDeliveriesResponse> AutoAssignDeliveriesAsync(CancellationToken ct)
    {
        var releasedCount = await deliveryAssignmentService.ReleaseExpiredAssignmentsAsync(
            DateTimeOffset.UtcNow,
            DeliveryAssignmentBatchSize,
            ct);
        var assignedCount = await deliveryAssignmentService.AutoAssignPendingAsync(
            DeliveryAssignmentBatchSize,
            DeliveryAssignmentAcceptanceTimeout,
            ct);

        return new AutoAssignDeliveriesResponse(assignedCount, releasedCount);
    }

    [HttpPost(ApiRoutesV1.MarketplaceJobsEnsureCouriers)]
    public async Task<EnsureCouriersResponse> EnsureCouriersAsync([FromBody] EnsureCouriersRequest request, CancellationToken ct)
    {
        var createdCount = await deliveryAssignmentService.EnsureCouriersAsync(request.Names, ct);
        return new EnsureCouriersResponse(createdCount);
    }

    [HttpPost(ApiRoutesV1.MarketplaceJobsCourierAssignmentDecision)]
    public async Task<CourierAssignmentDecisionResponse> ApplyCourierAssignmentDecisionAsync(
        [FromBody] CourierAssignmentDecisionRequest request,
        CancellationToken ct)
    {
        var deliveryId = DeliveryId.Parse(request.DeliveryId);
        var courierId = CourierId.Parse(request.CourierId);
        var delivery = request.Decision.Equals("reject", StringComparison.OrdinalIgnoreCase)
            ? await deliveryAssignmentService.RejectAssignmentAsync(deliveryId, courierId, request.Reason, ct)
            : await deliveryAssignmentService.AcceptAssignmentAsync(deliveryId, courierId, ct);

        return new CourierAssignmentDecisionResponse(
            delivery.Id.ToString(),
            delivery.Status.ToString(),
            delivery.CourierId?.ToString());
    }
}
