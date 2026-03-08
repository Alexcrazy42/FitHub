namespace FitHub.Contracts.V1.Users;

public class ChangePasswordRequest
{
    public string OldPassword { get; set; } = String.Empty;
    public string NewPassword { get; set; } = String.Empty;
}
