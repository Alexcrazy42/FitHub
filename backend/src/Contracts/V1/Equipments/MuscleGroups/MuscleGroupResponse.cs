namespace FitHub.Contracts.V1.Equipments.MuscleGroups;

public sealed class MuscleGroupResponse
{
    public Guid? Id { get; set; }

    public string? Name { get; set; }

    public string? ImageUrl { get; set; }

    public Guid? ParentId { get; set; }
}
