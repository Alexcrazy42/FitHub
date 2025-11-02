namespace FitHub.Common.AspNetCore.Auth;

public sealed class LoginResponse
{
    public string? Email { get; set; }

    public string? UserId { get; set; }

    public DateTimeOffset? LoginExpirationAt { get; set; }

    public IReadOnlyList<string> RoleNames { get; set; } = [];
}
