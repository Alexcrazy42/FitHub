using FitHub.Domain.Marketplace;

namespace FitHub.Data.Marketplace;

internal static class MarketplaceDemoData
{
    public static readonly ProductCategoryId AccessoriesCategoryId = new(Guid.Parse("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01"));
    public static readonly MarketplaceBrandId FitHubBrandId = new(Guid.Parse("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b02"));
    public static readonly ProductId ProductId = new(Guid.Parse("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b03"));
    public static readonly ProductVariantId ProductVariantId = new(Guid.Parse("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b04"));
    public static readonly ProductVariantInventoryId ProductVariantInventoryId = new(Guid.Parse("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b05"));
    public static readonly AttributeDefinitionId SizeAttributeId = new(Guid.Parse("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b06"));
    public static readonly AttributeOptionId SizeMOptionId = new(Guid.Parse("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b07"));
    public static readonly ProductVariantAttributeId ProductVariantAttributeId = new(Guid.Parse("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b08"));

    public static readonly DateTimeOffset CreatedAt = new(2026, 4, 12, 0, 0, 0, TimeSpan.Zero);
}
