using FitHub.Application.Equipments.Brands;
using FitHub.Common.Entities;
using FitHub.Contracts.V1.Equipments;
using FitHub.Contracts.V1.Equipments.Brands;
using FitHub.Contracts.V1.Equipments.GymEquipments;
using FitHub.Contracts.V1.Equipments.Gyms;
using FitHub.Contracts.V1.Equipments.Instructions;
using FitHub.Domain.Equipments;
using FitHub.Domain.Files;
using FitHub.Web.V1.Trainings;

namespace FitHub.Web.V1.Equipments;

public static class EquipmentResponseExtensions
{
    public static IReadOnlyList<GymResponse> ToResponses(this IReadOnlyList<Gym> gyms)
        => gyms.Select(ToResponse).ToList();

    public static IReadOnlyList<BrandResponse> ToResponses(this IReadOnlyList<Brand> brands)
        => brands.Select(ToResponse).ToList();

    public static IReadOnlyList<EquipmentResponse> ToResponses(this IReadOnlyList<Equipment> equipments)
        => equipments.Select(ToResponse).ToList();

    public static IReadOnlyList<EquipmentInstructionResponse> ToShortInstructionResponses(this IReadOnlyList<EquipmentInstruction> instructions)
        => instructions.Select(ToResponse).ToList();

    public static IReadOnlyList<GymEquipmentResponse> ToResponses(this IReadOnlyList<GymEquipment> entities)
        => entities.Select(ToResponse).ToList();

    public static IReadOnlyList<GymZoneResponse> ToResponses(this IReadOnlyList<GymZone> gymZones)
        => gymZones.Select(ToResponse).ToList();

    public static GymResponse ToResponse(this Gym gym)
    {
        return new GymResponse
        {
            Id = gym.Id.ToString(),
            Name = gym.Name,
            Description = gym.Description,
            ImageFileId = gym.Files.FirstOrDefault()?.Id.ToString(),
        };
    }

    public static EquipmentResponse ToResponse(this Equipment equipment)
    {
        return new EquipmentResponse
        {
            Id = equipment.Id.Value.ToString(),
            Name = equipment.Name,
            Description = equipment.Description,
            AdditionalDescroption = equipment.AdditionalDescroption,
            InstructionAddBefore = equipment.InstructionAddBefore,
            IsActive = equipment.IsActive,
            Brand = equipment.Brand?.ToResponse(),
            Instructions = equipment.Instructions.ToShortInstructionResponses()
        };
    }

    public static EquipmentInstructionResponse ToResponse(this EquipmentInstruction instruction)
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
            Equipment = instruction.Equipment.ToResponse(),
            Name = instruction.Name,
            Description = instruction.Description,
            AdditionalDescription = instruction.AdditionalDescription,
            MuscleGroups = instruction.MuscleGroups.ToMuscleGroupResponses().ToList()
        };
    }

    public static GymZoneResponse ToResponse(this GymZone gymZone)
    {
        return new GymZoneResponse
        {
            Id = gymZone.Id.ToString(),
            Name = gymZone.Name,
            Description = gymZone.Description,
        };
    }

    public static BrandResponse ToResponse(this Brand brand)
    {
        return new BrandResponse()
        {
            Id = brand.Id.Value.ToString(),
            Name = brand.Name,
            Description = brand.Description,
        };
    }

    public static SearchBrandCommand ToCommand(this SearchBrandRequest? request)
    {
        return new SearchBrandCommand()
        {
            Name = request?.Name,
        };
    }

    public static GymEquipmentResponse ToResponse(this GymEquipment entity)
    {
        return new GymEquipmentResponse
        {
            Id = entity.Id.ToString(),
            Equipment = entity.Equipment.ToResponse(),
            Gym = entity.Gym.ToResponse(),
            IsActive = entity.IsActive,
            OpenedAt = entity.OpenedAt,
            Condition = entity.Condition
        };
    }
}
