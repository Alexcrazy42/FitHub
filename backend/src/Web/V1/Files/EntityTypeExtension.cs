using FitHub.Contracts.V1.Entities;
using FitHub.Domain.Files;

namespace FitHub.Web.V1.Files;

public static class EntityTypeExtension
{

    public static EntityResponse ToEntityResponse(this Entity entity)
    {
        return new EntityResponse()
        {
            Id = entity.Id.ToString(),
            MaxFileCount = entity.MaxFileCount,
            Type = entity.EntityType,
        };
    }
}
