using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Messaging;

public class StickerGroup : IEntity<StickerGroupId>, IAuditableEntity
{
    public StickerGroup(StickerGroupId id, string name, bool isActive)
    {
        Id = id;
        Name = name;
        IsActive = isActive;
        IsDeleted = false;
    }

    public StickerGroupId Id { get; }

    public string Name { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; }

    public void SetName(string name) => Name = name;

    public void SetIsActive(bool isActive) => IsActive = isActive;

    public void SetIsDeleted(bool isDeleted) => IsDeleted = isDeleted;

    public static StickerGroup Create(string name)
        => new(StickerGroupId.New(), name, isActive: false);
}
