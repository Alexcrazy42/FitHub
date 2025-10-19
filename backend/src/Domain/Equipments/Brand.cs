using FitHub.Common.Entities;

namespace FitHub.Domain.Equipments;

public class Brand : IEntity<BrandId>
{
    private Brand(BrandId id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public BrandId Id { get; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetDescription(string description)
    {
        Description = description;
    }

    public static Brand Create(string name, string description)
    {
        return new Brand(BrandId.New(), name, description);
    }
}
