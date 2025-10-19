using FitHub.Common.Entities;
using FitHub.Contracts.V1.Equipments;
using FitHub.Contracts.V1.Equipments.Brands;
using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Contracts.V1.Equipments.Instructions;
using FitHub.Domain.Equipments;
using FitHub.Web.V1.Trainings;

namespace FitHub.Web.V1.Equipments;

public static class EquipmentResponseExtensions
{
    public static IReadOnlyList<GymResponse> ToGymResponses(this IReadOnlyList<Gym> gyms)
        => gyms.Select(ToGymResponse).ToList();

    public static IReadOnlyList<BrandResponse> ToBrandResponses(this IReadOnlyList<Brand> brands)
        => brands.Select(ToBrandResponse).ToList();

    public static IReadOnlyList<EquipmentResponse> ToEquipmentResponses(this IReadOnlyList<Equipment> equipments)
        => equipments.Select(ToEquipmentResponse).ToList();

    public static IReadOnlyList<EquipmentInstructionResponse> ToShortInstructionResponses(this IReadOnlyList<EquipmentInstruction> instructions)
        => instructions.Select(ToShortInstructionResponse).ToList();

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
            Id = equipment.Id.Value.ToString(),
            Name = equipment.Name,
            Description = equipment.Description,
            AdditionalDescroption = equipment.AdditionalDescroption,
            InstructionAddBefore = equipment.InstructionAddBefore,
            Brand = equipment.Brand?.ToBrandResponse(),
            Instructions = equipment.Instructions.ToShortInstructionResponses()
        };
    }

    public static EquipmentInstructionResponse ToShortInstructionResponse(this EquipmentInstruction instruction)
    {
        return new EquipmentInstructionResponse
        {
            Id = instruction.Id.Value.ToString(),
            EquipmentId = instruction.EquipmentId.ToString(),
            Name = instruction.Name,
            Description = instruction.Description,
            AdditionalDescription = instruction.AdditionalDescription
        };
    }

    public static EquipmentInstructionResponse ToFullInstructionResponse(this EquipmentInstruction instruction)
    {
        return new EquipmentInstructionResponse
        {
            Id = instruction.Id.Value.ToString(),
            EquipmentId = instruction.EquipmentId.ToString(),
            Equipment = instruction.Equipment.ToEquipmentResponse(),
            Name = instruction.Name,
            Description = instruction.Description,
            AdditionalDescription = instruction.AdditionalDescription,
            MuscleGroups = instruction.MuscleGroups.ToMuscleGroupResponses().ToList()
        };
    }

    public static GymZoneResponse ToZoneResponse(this GymZone gymZone)
    {
        return new GymZoneResponse
        {
            Id = gymZone.Id.Value,
            Name = gymZone.Name,
            Description = gymZone.Description,
        };
    }

    public static BrandResponse ToBrandResponse(this Brand brand)
    {
        return new BrandResponse()
        {
            Id = brand.Id.Value.ToString(),
            Name = brand.Name,
            Description = brand.Description,
        };
    }
}
