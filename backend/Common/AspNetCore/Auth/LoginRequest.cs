namespace FitHub.Common.AspNetCore.Auth;

public sealed class LoginRequest
{
    public string? Username { get; set; }

    public string? Password { get; set; }
}
