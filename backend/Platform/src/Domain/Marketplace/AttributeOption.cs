using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace;

public class AttributeOption : IEntity<AttributeOptionId>
{
    private AttributeOption(AttributeOptionId id, AttributeDefinitionId attributeDefinitionId, string value, int sortOrder)
    {
        Id = id;
        AttributeDefinitionId = attributeDefinitionId;
        Value = value;
        SortOrder = sortOrder;
    }

    public AttributeOptionId Id { get; }
    public AttributeDefinitionId AttributeDefinitionId { get; private set; }
    public AttributeDefinition? AttributeDefinition { get; private set; }
    public string Value { get; private set; }
    public int SortOrder { get; private set; }

    public static AttributeOption Create(AttributeDefinitionId attributeDefinitionId, string value, int sortOrder = 0)
    {
        return new AttributeOption(AttributeOptionId.New(), attributeDefinitionId, value, sortOrder);
    }
}
