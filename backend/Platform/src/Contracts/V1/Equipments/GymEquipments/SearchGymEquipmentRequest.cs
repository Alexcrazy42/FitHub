using FitHub.Shared.GymEquipments;

namespace FitHub.Contracts.V1.Equipments.GymEquipments;

public class SearchGymEquipmentRequest
{
    public string? EquipmentId { get; set; }

    public string? GymId { get; set; }

    public bool? IsActive { get; set; }

    public EquipmentCondition? Condition { get; set; }
}
