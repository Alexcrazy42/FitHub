namespace FitHub.Contracts.V1.Users;

public class UserResponse
{
    public string? Surname { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public IdentityUserTypeDto? UserType { get; set; }
}
