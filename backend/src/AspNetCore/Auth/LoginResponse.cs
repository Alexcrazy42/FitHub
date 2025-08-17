namespace FitHub.Common.AspNetCore.Auth;

public sealed class LoginResponse
{
    public string? Login { get; set; }

    public string? UserId { get; set; }

    public string? Name { get; set; }

    public DateTimeOffset? LoginExpirationAt { get; set; }

    public IReadOnlyList<string> RoleNames { get; set; } = [];
}
