
using FitHub.Common.Entities;
using FitHub.Shared.GymEquipments;

namespace FitHub.Contracts.V1.Equipments.GymEquipments;

public class AddOrUpdateGymEquipmentRequest
{
    public string? EquipmentId { get; set; }

    public string? GymId { get; set; }

    public bool? IsActive { get; set; }

    public DateTimeOffset? OpenedAt { get; set; }

    public EquipmentCondition? Condition { get; set; }

    public static void ValidateEquipment(string? equipmentId)
    {
        if (equipmentId is null)
        {
            throw new ValidationException("Укажите транежар");
        }
    }

    public static void ValidateGym(string? gymId)
    {
        if (gymId is null)
        {
            throw new ValidationException("Укажите зал");
        }
    }

    public static void ValidateIsActive(bool? isActive)
    {
        if (isActive is null)
        {
            throw new ValidationException("Укажите активен ли тренажер");
        }
    }

    public static void ValidateCondition(EquipmentCondition? condition)
    {
        if (condition is null)
        {
            throw new ValidationException("Укажите состояние тренажера!");
        }
    }

    public static void ValidateInactiveWithOpenedAt(AddOrUpdateGymEquipmentRequest req)
    {
        if (req is { IsActive: false, OpenedAt: null })
        {
            throw new ValidationException(
                "Невозможно сделать тренажер неактивным и не указать дату открытия!");
        }
    }

    public static void ValidateActiveWithOpenedAt(AddOrUpdateGymEquipmentRequest req)
    {
        if (req is { IsActive: true, OpenedAt: not null, Condition: EquipmentCondition.Operational })
        {
            throw new ValidationException(
                "Нельзя указать дату открытия для активного тренажера!");
        }
    }

    public static void ValidateMaintenanceWithOpenedAt(AddOrUpdateGymEquipmentRequest req)
    {
        if ((req.Condition is EquipmentCondition.Maintenance or EquipmentCondition.UnderRepair)
            && !req.OpenedAt.HasValue)
        {
            throw new ValidationException(
                "Необходимо указать, когда тренажер заново начнет функционировать!");
        }
    }
}
