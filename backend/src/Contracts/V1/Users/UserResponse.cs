namespace FitHub.Contracts.V1.Users;

public class UserResponse
{
    public string? Id { get; set; }

    public string? Surname { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public bool? IsActive { get; set; }

    public DateTimeOffset? StartActiveAt { get; set; }

    public List<string> RoleNames { get; set; } = [];
}
