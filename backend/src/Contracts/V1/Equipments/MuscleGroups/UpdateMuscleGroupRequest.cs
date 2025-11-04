namespace FitHub.Contracts.V1.Equipments.MuscleGroups;

public class UpdateMuscleGroupRequest
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public Guid? ParentId { get; set; }
}
