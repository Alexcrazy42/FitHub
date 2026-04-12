using FitHub.Application.Common;
using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public sealed record MarketplaceCatalogSearchResult(
    PagedResult<Product> Products,
    IReadOnlyList<MarketplaceCatalogCategoryFacetValue> Categories,
    IReadOnlyList<MarketplaceCatalogFacet> Facets);

public sealed record MarketplaceCatalogCategoryFacetValue(
    ProductCategoryId CategoryId,
    string Name,
    string Slug,
    int Count,
    bool Selected);

public sealed record MarketplaceCatalogFacet(
    AttributeDefinitionId AttributeDefinitionId,
    string Code,
    string Name,
    bool IsPurchaseOption,
    IReadOnlyList<MarketplaceCatalogFacetValue> Values);

public sealed record MarketplaceCatalogFacetValue(
    AttributeOptionId AttributeOptionId,
    string Value,
    int Count,
    bool Selected);
