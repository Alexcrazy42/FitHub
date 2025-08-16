namespace FitHub.Contracts.V1.Equipments.MuscleGroups;

public class UpdateMuscleGroupRequest
{
    public Guid? Id { get; set; }

    public string? Name { get; set; }

    public string? ImageUrl { get; set; }

    public Guid? ParentId { get; set; }
}
