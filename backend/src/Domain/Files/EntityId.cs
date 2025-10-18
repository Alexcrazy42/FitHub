using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Files;

public class EntityId : GuidIdentifier<EntityId>, IIdentifierDescription
{
    public EntityId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Сущность";
    public static string Prefix => FormatPrefix("fithub", "entity");
}
