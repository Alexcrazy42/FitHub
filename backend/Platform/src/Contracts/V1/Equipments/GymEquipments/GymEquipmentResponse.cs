using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Shared.GymEquipments;

namespace FitHub.Contracts.V1.Equipments.GymEquipments;

public class GymEquipmentResponse
{
    public string? Id { get; set; }

    public EquipmentResponse? Equipment { get; set; }

    public GymResponse? Gym { get; set; }

    public bool? IsActive { get; set; }

    public DateTimeOffset? OpenedAt { get; set; }

    public EquipmentCondition Condition { get; set; }
}
