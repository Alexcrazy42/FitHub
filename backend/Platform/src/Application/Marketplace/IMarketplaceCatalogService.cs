using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public interface IMarketplaceCatalogService
{
    Task<MarketplaceCatalogSearchResult> SearchProductsAsync(MarketplaceCatalogSearchCommand command, CancellationToken ct);

    Task<Product?> GetProductAsync(ProductId productId, CancellationToken ct);
}
