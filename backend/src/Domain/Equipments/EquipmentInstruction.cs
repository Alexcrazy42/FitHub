using FitHub.Common.Entities;

namespace FitHub.Domain.Equipments;

public class EquipmentInstruction : IEntity<EquipmentInstructionId>
{
    private Equipment? equipment;

    public EquipmentInstruction(EquipmentInstructionId id, string videoUrl)
    {
        Id = id;
        VideoUrl = videoUrl;
    }

    public EquipmentInstructionId Id { get; }

    public string VideoUrl { get; private set; }

    public Equipment Equipment
    {
        get => UnexpectedException.ThrowIfNull(equipment, "Тренажер неожиданно стал null");
        set => equipment = value;
    }

    public void SetVideoUrl(string videoUrl)
    {
        VideoUrl = videoUrl;
    }

    public void SetEquipment(Equipment newEquipment)
    {
        equipment = newEquipment;
    }
}
