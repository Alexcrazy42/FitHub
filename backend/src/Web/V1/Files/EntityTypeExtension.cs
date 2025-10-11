using FitHub.Contracts.Common;
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
            default:
                throw new ValidationException("Invalid entity type");
        }
    }
}
