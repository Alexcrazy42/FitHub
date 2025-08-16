namespace FitHub.Contracts.V1.Equipments.Gyms;

public class UpdateGymRequest
{
    public Guid? Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
}
