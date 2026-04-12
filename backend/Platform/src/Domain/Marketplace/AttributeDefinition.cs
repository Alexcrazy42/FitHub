using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace;

public class AttributeDefinition : IEntity<AttributeDefinitionId>
{
    private readonly List<AttributeOption> options = [];

    private AttributeDefinition(
        AttributeDefinitionId id,
        string code,
        string name,
        bool isPurchaseOption,
        bool isFilterable,
        int sortOrder)
    {
        Id = id;
        Code = code;
        Name = name;
        IsPurchaseOption = isPurchaseOption;
        IsFilterable = isFilterable;
        SortOrder = sortOrder;
    }

    public AttributeDefinitionId Id { get; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public bool IsPurchaseOption { get; private set; }
    public bool IsFilterable { get; private set; }
    public int SortOrder { get; private set; }
    public IReadOnlyList<AttributeOption> Options => options;

    public void AddOption(AttributeOption option)
    {
        options.Add(option);
    }

    public static AttributeDefinition Create(string code, string name, bool isPurchaseOption, bool isFilterable, int sortOrder = 0)
    {
        return new AttributeDefinition(AttributeDefinitionId.New(), code, name, isPurchaseOption, isFilterable, sortOrder);
    }
}
