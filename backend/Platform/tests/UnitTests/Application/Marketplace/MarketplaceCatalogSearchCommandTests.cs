using FitHub.Application.Marketplace;
using FitHub.Domain.Marketplace;
using Shouldly;
using Xunit;

namespace FitHub.UnitTests.Application.Marketplace;

public class MarketplaceCatalogSearchCommandTests
{
    [Fact(DisplayName = "Catalog search command normalizes pagination and sort")]
    public void Create_ShouldNormalizePaginationAndSort()
    {
        var command = MarketplaceCatalogSearchCommand.Create(
            categoryId: null,
            searchText: "  mat  ",
            minPrice: null,
            maxPrice: null,
            inStock: null,
            facets: null,
            sort: "unknown",
            pageNumber: 0,
            pageSize: 200);

        command.SearchText.ShouldBe("mat");
        command.Sort.ShouldBe(MarketplaceCatalogSort.Popular);
        command.PagedQuery.PageNumber.ShouldBe(MarketplaceCatalogSearchCommand.DefaultPageNumber);
        command.PagedQuery.PageSize.ShouldBe(MarketplaceCatalogSearchCommand.MaxPageSize);
    }

    [Fact(DisplayName = "Catalog search command parses category and attribute facets")]
    public void Create_ShouldParseCategoryAndFacets()
    {
        var categoryId = ProductCategoryId.New();
        var attributeDefinitionId = AttributeDefinitionId.New();
        var optionId = AttributeOptionId.New();
        var facets = new Dictionary<string, IReadOnlyList<string>>
        {
            [attributeDefinitionId.ToString()] = [optionId.ToString()]
        };

        var command = MarketplaceCatalogSearchCommand.Create(
            categoryId: categoryId.ToString(),
            searchText: null,
            minPrice: 100m,
            maxPrice: 200m,
            inStock: true,
            facets: facets,
            sort: MarketplaceCatalogSort.PriceAsc,
            pageNumber: 2,
            pageSize: 10);

        command.CategoryId.ShouldBe(categoryId);
        command.MinPrice.ShouldBe(100m);
        command.MaxPrice.ShouldBe(200m);
        command.InStock.ShouldBe(true);
        command.Sort.ShouldBe(MarketplaceCatalogSort.PriceAsc);
        command.Facets.ShouldContainKey(attributeDefinitionId);
        command.Facets[attributeDefinitionId].ShouldContain(optionId);
    }
}
