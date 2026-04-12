namespace FitHub.Contracts.V1.Marketplace;

public record MarketplaceMoneyResponse(decimal Amount, string Currency);

public record MarketplaceStockResponse(int QuantityOnHand, int QuantityReserved, int AvailableQuantity);

public record MarketplaceProductImageResponse(
    string FileId,
    string Url,
    string? AltText,
    int SortOrder,
    bool IsMain);

public record MarketplaceProductVariantResponse(
    string Id,
    string Sku,
    string? Name,
    MarketplaceMoneyResponse Price,
    MarketplaceMoneyResponse? CompareAtPrice,
    bool IsActive,
    MarketplaceStockResponse? Stock,
    IReadOnlyList<MarketplaceVariantAttributeResponse> Attributes);

public record MarketplaceVariantAttributeResponse(
    string AttributeDefinitionId,
    string AttributeOptionId,
    string Code,
    string Name,
    string Value);

public record MarketplaceProductListItemResponse(
    string Id,
    string Name,
    string Slug,
    string? BrandName,
    string CategoryId,
    MarketplaceMoneyResponse PriceFrom,
    bool IsAvailable,
    MarketplaceProductImageResponse? MainImage);

public record MarketplaceProductDetailsResponse(
    string Id,
    string Name,
    string Slug,
    string? Description,
    string? BrandName,
    string CategoryId,
    IReadOnlyList<MarketplaceProductImageResponse> Images,
    IReadOnlyList<MarketplaceProductVariantResponse> Variants);

public record MarketplaceFacetResponse(
    string AttributeDefinitionId,
    string Code,
    string Name,
    bool IsPurchaseOption,
    IReadOnlyList<MarketplaceFacetValueResponse> Values);

public record MarketplaceFacetValueResponse(
    string AttributeOptionId,
    string Value,
    int Count,
    bool Selected);

public record MarketplaceCatalogSearchRequest(
    string? CategoryId,
    string? SearchText,
    decimal? MinPrice,
    decimal? MaxPrice,
    IReadOnlyDictionary<string, IReadOnlyList<string>>? Facets,
    string? Sort,
    int PageNumber = 1,
    int PageSize = 20);

public record MarketplaceCatalogSearchResponse(
    IReadOnlyList<MarketplaceProductListItemResponse> Items,
    int ProductCount,
    IReadOnlyList<MarketplaceFacetResponse> Facets);

public record CreateCheckoutReservationRequest(
    string ProductVariantId,
    int Quantity,
    string IdempotencyKey);

public record CheckoutReservationResponse(
    string ReservationId,
    string ProductVariantId,
    int Quantity,
    string Status,
    DateTimeOffset ExpiresAt);

public record MarketplaceErrorResponse(
    string Code,
    string Message,
    IReadOnlyDictionary<string, string>? Details = null);
