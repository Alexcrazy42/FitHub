using FitHub.Contracts.Common;
using FitHub.Contracts.V1.Entities;
using FitHub.Domain.Files;
using FluentValidation;

namespace FitHub.Web.V1.Files;

public static class EntityTypeExtension
{
    public static EntityType FromDto(this EntityTypeDto dto)
    {
        switch (dto)
        {
            case EntityTypeDto.Gym:
                return EntityType.Gym;
            case EntityTypeDto.Equipment:
                return EntityType.Equipment;
            case EntityTypeDto.BaseGroupTraining:
                return EntityType.BaseGroupTraining;
            default:
                throw new ValidationException("Invalid entity type");
        }
    }

    public static EntityTypeDto ToDto(this EntityType entityType)
    {
        switch (entityType)
        {
            case EntityType.Gym:
                return EntityTypeDto.Gym;
            case EntityType.Equipment:
                return EntityTypeDto.Equipment;
            case EntityType.BaseGroupTraining:
                return EntityTypeDto.BaseGroupTraining;
            default:
                throw new ValidationException("Invalid entity type");
        }
    }

    public static EntityResponse ToEntityResponse(this Entity entity)
    {
        return new EntityResponse()
        {
            Id = entity.Id.ToString(),
            MaxFileCount = entity.MaxFileCount,
            Type = entity.EntityType.ToDto(),
        };
    }
}
