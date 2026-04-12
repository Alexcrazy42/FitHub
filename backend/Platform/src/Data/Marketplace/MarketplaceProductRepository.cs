using FitHub.Application.Common;
using FitHub.Application.Marketplace;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Marketplace;

public class MarketplaceProductRepository : DefaultPendingRepository<Product, ProductId, DataContext>, IMarketplaceProductRepository
{
    public MarketplaceProductRepository(DataContext context) : base(context)
    {
    }

    public async Task<MarketplaceCatalogSearchResult> SearchAsync(MarketplaceCatalogSearchCommand command, CancellationToken ct)
    {
        var dbQuery = ReadRaw()
            .Where(x => x.IsActive);

        if (command.CategoryId is not null)
        {
            dbQuery = dbQuery.Where(x => x.CategoryId == command.CategoryId);
        }

        if (!String.IsNullOrWhiteSpace(command.SearchText))
        {
            dbQuery = dbQuery.Where(x =>
                x.Name.Contains(command.SearchText) ||
                (x.Description != null && x.Description.Contains(command.SearchText)));
        }

        if (command.MinPrice is not null)
        {
            dbQuery = dbQuery.Where(x => x.Variants.Any(v => v.IsActive && v.PriceAmount >= command.MinPrice.Value));
        }

        if (command.MaxPrice is not null)
        {
            dbQuery = dbQuery.Where(x => x.Variants.Any(v => v.IsActive && v.PriceAmount <= command.MaxPrice.Value));
        }

        if (command.InStock == true)
        {
            dbQuery = dbQuery.Where(x => x.Variants.Any(v =>
                v.IsActive &&
                v.Inventory != null &&
                v.Inventory.QuantityOnHand > v.Inventory.QuantityReserved));
        }

        foreach (var (attributeDefinitionId, optionIds) in command.Facets)
        {
            if (optionIds.Count == 0)
            {
                continue;
            }

            var filterOptionIds = optionIds;
            dbQuery = dbQuery.Where(x => x.Variants.Any(v =>
                v.IsActive &&
                v.Attributes.Any(a =>
                    a.AttributeDefinitionId == attributeDefinitionId &&
                    filterOptionIds.Contains(a.AttributeOptionId))));
        }

        var totalItems = await dbQuery.CountAsync(ct);
        var categories = await GetCategoriesAsync(dbQuery, command, ct);
        var facets = await GetFacetsAsync(dbQuery, command, ct);

        dbQuery = ApplySort(dbQuery, command.Sort)
            .Include(x => x.Brand)
            .Include(x => x.Images)
            .Include(x => x.Variants)
                .ThenInclude(x => x.Inventory)
            .AsSplitQuery()
            .Skip((command.PagedQuery.PageNumber - 1) * command.PagedQuery.PageSize)
            .Take(command.PagedQuery.PageSize);

        var items = await dbQuery.ToListAsync(ct);
        var products = PagedResult<Product>.Create(
            items,
            totalItems,
            command.PagedQuery.PageNumber,
            command.PagedQuery.PageSize);

        return new MarketplaceCatalogSearchResult(products, categories, facets);
    }

    public Task<Product?> GetDetailsAsync(ProductId productId, CancellationToken ct)
    {
        return ReadRaw()
            .Where(x => x.IsActive)
            .Include(x => x.Brand)
            .Include(x => x.Images)
            .Include(x => x.Variants)
                .ThenInclude(x => x.Inventory)
            .Include(x => x.Variants)
                .ThenInclude(x => x.Attributes)
                    .ThenInclude(x => x.AttributeDefinition)
            .Include(x => x.Variants)
                .ThenInclude(x => x.Attributes)
                    .ThenInclude(x => x.AttributeOption)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == productId, ct);
    }

    private static IQueryable<Product> ApplySort(IQueryable<Product> dbQuery, string sort)
    {
        return sort switch
        {
            MarketplaceCatalogSort.Newest => dbQuery
                .OrderByDescending(x => x.CreatedAt)
                .ThenBy(x => x.Name),
            MarketplaceCatalogSort.PriceAsc => dbQuery
                .OrderBy(x => x.Variants.Where(v => v.IsActive).Min(v => (decimal?)v.PriceAmount) ?? Decimal.MaxValue)
                .ThenBy(x => x.Name),
            MarketplaceCatalogSort.PriceDesc => dbQuery
                .OrderByDescending(x => x.Variants.Where(v => v.IsActive).Max(v => (decimal?)v.PriceAmount) ?? Decimal.Zero)
                .ThenBy(x => x.Name),
            _ => dbQuery
                .OrderBy(x => x.Name)
                .ThenByDescending(x => x.CreatedAt)
        };
    }

    private async Task<IReadOnlyList<MarketplaceCatalogFacet>> GetFacetsAsync(
        IQueryable<Product> dbQuery,
        MarketplaceCatalogSearchCommand command,
        CancellationToken ct)
    {
        var productIds = dbQuery.Select(x => x.Id);
        var counts = await Context.Set<ProductVariantAttribute>()
            .Where(x =>
                x.ProductVariant != null &&
                x.ProductVariant.IsActive &&
                productIds.Contains(x.ProductVariant.ProductId))
            .GroupBy(x => new { x.AttributeDefinitionId, x.AttributeOptionId })
            .Select(x => new
            {
                x.Key.AttributeDefinitionId,
                x.Key.AttributeOptionId,
                Count = x.Select(a => a.ProductVariant!.ProductId).Distinct().Count()
            })
            .ToListAsync(ct);

        var countByOption = counts.ToDictionary(
            x => (x.AttributeDefinitionId, x.AttributeOptionId),
            x => x.Count);
        var definitions = await Context.Set<AttributeDefinition>()
            .Include(x => x.Options)
            .Where(x => x.IsFilterable)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(ct);

        return definitions.Select(definition =>
            new MarketplaceCatalogFacet(
                definition.Id,
                definition.Code,
                definition.Name,
                definition.IsPurchaseOption,
                definition.Options
                    .OrderBy(x => x.SortOrder)
                    .ThenBy(x => x.Value)
                    .Select(option => new MarketplaceCatalogFacetValue(
                        option.Id,
                        option.Value,
                        countByOption.GetValueOrDefault((definition.Id, option.Id), 0),
                        command.Facets.TryGetValue(definition.Id, out var selectedOptions) &&
                        selectedOptions.Contains(option.Id)))
                    .ToList()))
            .ToList();
    }

    private async Task<IReadOnlyList<MarketplaceCatalogCategoryFacetValue>> GetCategoriesAsync(
        IQueryable<Product> dbQuery,
        MarketplaceCatalogSearchCommand command,
        CancellationToken ct)
    {
        var counts = await dbQuery
            .GroupBy(x => x.CategoryId)
            .Select(x => new { CategoryId = x.Key, Count = x.Count() })
            .ToListAsync(ct);
        var countByCategoryId = counts.ToDictionary(x => x.CategoryId, x => x.Count);
        var categories = await Context.Set<ProductCategory>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        return categories
            .Select(category => new MarketplaceCatalogCategoryFacetValue(
                category.Id,
                category.Name,
                category.Slug,
                countByCategoryId.GetValueOrDefault(category.Id, 0),
                category.Id == command.CategoryId))
            .ToList();
    }
}
