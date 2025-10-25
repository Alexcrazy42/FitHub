using FitHub.Common.Entities;

namespace FitHub.Domain.Files;

public class Entity : IEntity<EntityId>
{
    public Entity(EntityId id, EntityType entityType, int maxFileCount)
    {
        Id = id;
        EntityType = entityType;
        MaxFileCount = maxFileCount;
    }

    public EntityId Id { get; }

    public EntityType EntityType { get; private set; }

    public int MaxFileCount { get; private set; }

    public void SetMaxFileCount(int maxFileCount)
    {
        MaxFileCount = maxFileCount;
    }

    public static Entity Create(EntityId id, EntityType entityType, int maxFileCount)
    {
        return new Entity(id, entityType, maxFileCount);
    }
}


public class EntityExtension
{
    public static List<Entity> GetAll()
    {
        return
        [
            Entity.Create(EntityId.Parse("6206cfea-d518-4b09-9257-0f790473545e"), EntityType.Gym, 1),
            Entity.Create(EntityId.Parse("6206cfea-d518-4b09-9257-0f7904735451"), EntityType.Equipment, 5),
        ];
    }
}
