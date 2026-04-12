using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public class MarketplaceCatalogService : IMarketplaceCatalogService
{
    private readonly IMarketplaceProductRepository productRepository;

    public MarketplaceCatalogService(IMarketplaceProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }

    public Task<MarketplaceCatalogSearchResult> SearchProductsAsync(MarketplaceCatalogSearchCommand command, CancellationToken ct)
        => productRepository.SearchAsync(command, ct);

    public Task<Product?> GetProductAsync(ProductId productId, CancellationToken ct)
        => productRepository.GetDetailsAsync(productId, ct);
}
