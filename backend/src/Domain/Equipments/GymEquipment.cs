using FitHub.Common.Entities;
using FitHub.Shared.GymEquipments;

namespace FitHub.Domain.Equipments;

public class GymEquipment : IEntity<GymEquipmentId>
{
    private Equipment? equipment;
    private Gym? gym;

    public GymEquipment(GymEquipmentId id, EquipmentId equipmentId, GymId gymId, EquipmentCondition condition, bool isActive)
    {
        Id = id;
        EquipmentId = equipmentId;
        GymId = gymId;
        Condition = condition;
        IsActive = isActive;
    }

    public GymEquipmentId Id { get; }

    public EquipmentId EquipmentId { get; private set; }

    public Equipment Equipment
    {
        get => UnexpectedException.ThrowIfNull(equipment, "Тренажер неожиданно оказался null");
        private set => equipment = value;
    }

    public GymId GymId { get; set; }

    public Gym Gym
    {
        get => UnexpectedException.ThrowIfNull(gym, "Зал неожиданно оказался null");
        private set => gym = value;
    }

    public bool IsActive { get; private set; }

    public DateTimeOffset? OpenedAt { get; private set; }

    public EquipmentCondition Condition { get; private set; }

    public void SetActive(bool isActive, DateTimeOffset? openedAt = null)
    {
        if (!isActive && !openedAt.HasValue)
        {
            throw new ValidationException("Невозможно сделать тренажер неактивным и не указать дату открытия!");
        }

        IsActive = isActive;
        if (openedAt.HasValue)
        {
            SetOpenedAt(openedAt.Value);
        }
    }

    public void SetOpenedAt(DateTimeOffset openedAt)
    {
        OpenedAt = openedAt;
    }

    public void SetCondition(EquipmentCondition condition, DateTimeOffset? openedAt = null)
    {
        if (openedAt.HasValue)
        {
            SetOpenedAt(openedAt.Value);
        }

        if (condition is EquipmentCondition.Maintenance or EquipmentCondition.UnderRepair && !openedAt.HasValue)
        {
            throw new ValidationException("Необходимо указать, когда тренежар заново начнет функционировать!");
        }

        Condition = condition;
    }

    public void SetGym(Gym newGym)
    {
        GymId = newGym.Id;
        Gym = newGym;
    }

    public static GymEquipment Create(Equipment equipment, Gym gym, EquipmentCondition condition, bool isActive, DateTimeOffset? openedAt = null)
    {
        if (!isActive && !openedAt.HasValue)
        {
            throw new ValidationException("Невозможно сделать тренажер неактивным и не указать дату открытия!");
        }

        return new GymEquipment(GymEquipmentId.New(), equipment.Id, gym.Id, condition, isActive)
        {
            OpenedAt = openedAt
        };
    }
}
