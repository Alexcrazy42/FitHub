using FitHub.Common.Http;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Marketplace;
using Microsoft.Extensions.Options;

namespace FitHub.Clients.Marketplace;

internal sealed class MarketplaceJobsClient : IMarketplaceJobsClient
{
    private readonly HttpClient client;
    private readonly Uri baseUri;

    public MarketplaceJobsClient(HttpClient client, IOptions<FitHubClientOptions> options)
    {
        this.client = client;
        baseUri = options.Value.RequiredServerUrl;
    }

    public async Task<ReleaseExpiredStockReservationsResponse> ReleaseExpiredReservationsAsync(CancellationToken ct)
    {
        var response = await client.PostAsync<ReleaseExpiredStockReservationsResponse>(
            new Uri(baseUri, ApiRoutesV1.MarketplaceJobsReleaseExpiredReservations),
            ct);

        return response ?? new ReleaseExpiredStockReservationsResponse(0);
    }

    public async Task ApplyBankPaymentStatusAsync(ApplyBankPaymentStatusRequest request, CancellationToken ct)
    {
        await client.PostAsJsonAsync<ApplyBankPaymentStatusRequest, object>(
            new Uri(baseUri, ApiRoutesV1.MarketplaceJobsApplyBankPaymentStatus),
            request,
            ct);
    }

    public async Task<PublishOutboxMessagesResponse> PublishOutboxAsync(CancellationToken ct)
    {
        var response = await client.PostAsync<PublishOutboxMessagesResponse>(
            new Uri(baseUri, ApiRoutesV1.MarketplaceJobsPublishOutbox),
            ct);

        return response ?? new PublishOutboxMessagesResponse(0, 0);
    }
}
