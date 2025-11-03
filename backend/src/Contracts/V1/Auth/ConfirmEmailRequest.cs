namespace FitHub.Contracts.V1.Auth;

public class ConfirmEmailRequest
{
    public string? UserId { get; set; }

    public string? Token { get; set; }
}
