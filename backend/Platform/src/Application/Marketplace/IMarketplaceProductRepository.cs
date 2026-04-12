using FitHub.Common.Entities.Storage;
using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public interface IMarketplaceProductRepository : IPendingRepository<Product, ProductId>
{
    Task<MarketplaceCatalogSearchResult> SearchAsync(MarketplaceCatalogSearchCommand command, CancellationToken ct);

    Task<Product?> GetDetailsAsync(ProductId productId, CancellationToken ct);
}
