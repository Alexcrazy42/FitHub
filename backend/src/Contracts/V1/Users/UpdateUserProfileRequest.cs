namespace FitHub.Contracts.V1.Users;

public class UpdateUserProfileRequest
{
    public string Name { get; set; } = String.Empty;
    public string Surname { get; set; } = String.Empty;
}
