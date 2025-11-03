namespace FitHub.Contracts.V1.Auth;

public class CreateGymAdminRequest
{
    public string? Surname { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? GymId { get; set; }
}
