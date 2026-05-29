using FitHub.Application.Common;
using FitHub.Common.Entities;
using FitHub.Domain.Marketplace;

namespace FitHub.Application.Marketplace;

public sealed record MarketplaceCatalogSearchCommand(
    ProductCategoryId? CategoryId,
    string? SearchText,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? InStock,
    IReadOnlyDictionary<AttributeDefinitionId, IReadOnlyList<AttributeOptionId>> Facets,
    string Sort,
    PagedQuery PagedQuery)
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public static MarketplaceCatalogSearchCommand Create(
        string? categoryId,
        string? searchText,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStock,
        IReadOnlyDictionary<string, IReadOnlyList<string>>? facets,
        string? sort,
        int pageNumber,
        int pageSize)
    {
        var parsedFacets = new Dictionary<AttributeDefinitionId, IReadOnlyList<AttributeOptionId>>();

        if (facets is not null)
        {
            foreach (var (attributeDefinitionId, optionIds) in facets)
            {
                if (optionIds.Count == 0)
                {
                    continue;
                }

                parsedFacets[AttributeDefinitionId.Parse(attributeDefinitionId)] = optionIds
                    .Where(x => !String.IsNullOrWhiteSpace(x))
                    .Select(AttributeOptionId.Parse)
                    .ToList();
            }
        }

        var normalizedPageNumber = pageNumber > 0 ? pageNumber : DefaultPageNumber;
        var normalizedPageSize = pageSize > 0 ? Math.Min(pageSize, MaxPageSize) : DefaultPageSize;

        return new MarketplaceCatalogSearchCommand(
            String.IsNullOrWhiteSpace(categoryId) ? null : ProductCategoryId.Parse(categoryId),
            String.IsNullOrWhiteSpace(searchText) ? null : searchText.Trim(),
            minPrice,
            maxPrice,
            inStock,
            parsedFacets,
            MarketplaceCatalogSort.Normalize(sort),
            new PagedQuery(normalizedPageNumber, normalizedPageSize));
    }
}
