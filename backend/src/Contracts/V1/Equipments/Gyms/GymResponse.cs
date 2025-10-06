namespace FitHub.Contracts.V1.Equipments.Gyms;

public sealed class GymResponse
{
    public Guid? Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }
}
