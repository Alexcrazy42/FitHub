using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace;

public class ProductVariantAttribute : IEntity<ProductVariantAttributeId>
{
    private ProductVariantAttribute(
        ProductVariantAttributeId id,
        ProductVariantId productVariantId,
        AttributeDefinitionId attributeDefinitionId,
        AttributeOptionId attributeOptionId)
    {
        Id = id;
        ProductVariantId = productVariantId;
        AttributeDefinitionId = attributeDefinitionId;
        AttributeOptionId = attributeOptionId;
    }

    public ProductVariantAttributeId Id { get; }
    public ProductVariantId ProductVariantId { get; private set; }
    public ProductVariant? ProductVariant { get; private set; }
    public AttributeDefinitionId AttributeDefinitionId { get; private set; }
    public AttributeDefinition? AttributeDefinition { get; private set; }
    public AttributeOptionId AttributeOptionId { get; private set; }
    public AttributeOption? AttributeOption { get; private set; }

    public static ProductVariantAttribute Create(
        ProductVariantId productVariantId,
        AttributeDefinitionId attributeDefinitionId,
        AttributeOptionId attributeOptionId)
    {
        return new ProductVariantAttribute(ProductVariantAttributeId.New(), productVariantId, attributeDefinitionId, attributeOptionId);
    }
}
