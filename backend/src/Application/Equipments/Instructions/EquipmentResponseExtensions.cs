using FitHub.Common.Entities;
using FitHub.Contracts.V1.Equipments.Instructions;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.Instructions;

public static class EquipmentResponseExtensions
{
    public static EquipmentInstructionType FromInstructionDto(this EquipmentInstructionTypeDto dto)
    {
        if (!Enum.IsDefined(typeof(EquipmentInstructionType), dto.ToString()))
        {
            throw new UnexpectedException($"Неизвестный тип: {dto}");
        }

        return Enum.Parse<EquipmentInstructionType>(dto.ToString());
    }

    public static EquipmentInstructionTypeDto FromDomainIntructionType(this EquipmentInstructionType type)
    {
        if (!Enum.IsDefined(typeof(EquipmentInstructionTypeDto), type.ToString()))
        {
            throw new UnexpectedException($"Неизвестный тип: {type}");
        }

        return Enum.Parse<EquipmentInstructionTypeDto>(type.ToString());
    }
}
