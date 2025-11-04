namespace FitHub.Contracts.V1.Equipments.MuscleGroups;

public sealed class MuscleGroupResponse
{
    public Guid? Id { get; set; }

    public string? Name { get; set; }

    public string? ImageId { get; set; }

    public List<MuscleGroupResponse> Childrens { get; set; } = [];

    public string? ParentId { get; set; }
}
