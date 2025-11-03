namespace FitHub.Contracts.V1.Auth;

public class VerifyTwoFactorRequest
{
    public string? UserId { get; set; }

    public string? Code { get; set; }
}
