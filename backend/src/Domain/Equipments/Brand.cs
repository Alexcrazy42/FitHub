using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Equipments;

public class Brand : IEntity<BrandId>, IAuditableEntity
{
    public Brand(BrandId id, string name, string imageUrl)
    {
        Id = id;
        Name = name;
        ImageUrl = imageUrl;
    }

    public BrandId Id { get; }

    public string Name { get; private set; }

    public string ImageUrl { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void SetCreatedAt(DateTimeOffset date)
    {
        CreatedAt = date;
    }

    public void SetUpdatedAt(DateTimeOffset date)
    {
        UpdatedAt = date;
    }
}
