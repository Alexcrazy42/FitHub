using FitHub.Contracts.V1.Equipments;
using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Contracts.V1.Equipments.Instructions;
using FitHub.Domain.Equipments;

namespace FitHub.Web.V1.Equipments;

public static class EquipmentResponseExtensions
{
    public static IReadOnlyList<GymResponse> ToGymResponses(this IReadOnlyList<Gym> gyms)
        => gyms.Select(ToGymResponse).ToList();

    public static IReadOnlyList<EquipmentResponse> ToEquipmentResponses(this IReadOnlyList<Equipment> equipments)
        => equipments.Select(ToEquipmentResponse).ToList();

    public static IReadOnlyList<EquipmentInstructionResponse> ToInstructionResponses(this IReadOnlyList<EquipmentInstruction> instructions)
        => instructions.Select(ToInstructionResponse).ToList();

    public static IReadOnlyList<GymZoneResponse> ToGymZoneResponses(this IReadOnlyList<GymZone> gymZones)
        => gymZones.Select(ToZoneResponse).ToList();

    public static GymResponse ToGymResponse(this Gym gym)
    {
        return new GymResponse
        {
            Id = gym.Id.Value,
            Name = gym.Name,
            Description = gym.Description,
            ImageFileId = gym.Files.FirstOrDefault()?.Id.ToString(),
        };
    }

    public static EquipmentResponse ToEquipmentResponse(this Equipment equipment)
    {
        return new EquipmentResponse
        {
            Id = equipment.Id.Value,
            Name = equipment.Name,
            ImageUrl = equipment.ImageUrl,
        };
    }

    public static EquipmentInstructionResponse ToInstructionResponse(this EquipmentInstruction instruction)
    {
        return new EquipmentInstructionResponse
        {
            Id = instruction.Id.Value,
            EquipmentId = instruction.Equipment.Id.Value,
            VideoUrl = instruction.VideoUrl
        };
    }

    public static GymZoneResponse ToZoneResponse(this GymZone gymZone)
    {
        return new GymZoneResponse()
        {
            Id = gymZone.Id.Value,
            Name = gymZone.Name,
            Description = gymZone.Description,
        };
    }
}
