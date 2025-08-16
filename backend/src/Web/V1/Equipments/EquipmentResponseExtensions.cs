using FitHub.Contracts.V1.Equipments;
using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Contracts.V1.Equipments.Instructions;
using FitHub.Domain.Equipments;

namespace FitHub.Web.V1.Equipments;

public static class EquipmentResponseExtensions
{
    public static IReadOnlyList<GymResponse> ToResponses(this IReadOnlyList<Gym> gyms)
        => gyms.Select(ToResponse).ToList();

    public static IReadOnlyList<EquipmentResponse> ToResponses(this IReadOnlyList<Equipment> equipments)
        => equipments.Select(ToResponse).ToList();

    public static IReadOnlyList<EquipmentInstructionResponse> ToResponses(this IReadOnlyList<EquipmentInstruction> instructions)
        => instructions.Select(ToResponse).ToList();

    public static GymResponse ToResponse(this Gym gym)
    {
        return new GymResponse
        {
            Id = gym.Id.Value,
            Name = gym.Name,
            Description = gym.Description
        };
    }

    public static EquipmentResponse ToResponse(this Equipment equipment)
    {
        return new EquipmentResponse
        {
            Id = equipment.Id.Value,
            Name = equipment.Name,
            ImageUrl = equipment.ImageUrl,
        };
    }

    public static EquipmentInstructionResponse ToResponse(this EquipmentInstruction instruction)
    {
        return new EquipmentInstructionResponse
        {
            Id = instruction.Id.Value,
            EquipmentId = instruction.Equipment.Id.Value,
            VideoUrl = instruction.VideoUrl
        };
    }
}
