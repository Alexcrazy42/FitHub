namespace FitHub.Contracts.V1.Auth;

public class SetPasswordRequest
{
    public string? UserId { get; set; }

    public string? Password { get; set; }
}
