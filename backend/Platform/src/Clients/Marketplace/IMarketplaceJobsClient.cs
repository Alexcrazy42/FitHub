using FitHub.Contracts.V1.Marketplace;

namespace FitHub.Clients.Marketplace;

public interface IMarketplaceJobsClient
{
    Task<ReleaseExpiredStockReservationsResponse> ReleaseExpiredReservationsAsync(CancellationToken ct);

    Task ApplyBankPaymentStatusAsync(ApplyBankPaymentStatusRequest request, CancellationToken ct);

    Task<PublishOutboxMessagesResponse> PublishOutboxAsync(CancellationToken ct);
}
