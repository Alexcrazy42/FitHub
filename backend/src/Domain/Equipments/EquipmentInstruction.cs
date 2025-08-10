using FitHub.Common.Entities;

namespace FitHub.Domain.Equipments;

public class EquipmentInstruction : IEntity<EquipmentInstructionId>
{
    public EquipmentInstruction(EquipmentInstructionId id, string videoUrl)
    {
        Id = id;
        VideoUrl = videoUrl;
    }

    public EquipmentInstructionId Id { get; }

    public string VideoUrl { get; private set; }

    public void SetVideoUrl(string videoUrl)
    {
        VideoUrl = videoUrl;
    }
}
