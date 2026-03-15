namespace FitHub.Common.AspNetCore.Auth;

public sealed class LoginResponse
{
    public required string? Email { get; set; }

    public required string? UserId { get; set; }

    public required bool? IsTemporaryPassword { get; set; }

    public required bool? IsActive { get; set; }

    public required bool? LoginFlowDone { get; set; }

    public string? JwtToken { get; set; }

    public required IReadOnlyList<string> RoleNames { get; set; } = [];
}
