namespace FitHub.Common.AspNetCore.Auth;

public static class AuthorizationPolicies
{
    public const string GymAdminOnly = "GymAdminOnly";

    public const string CmsAdminOnly = "CmsAdminOnly";

    public const string TrainerOnly = "TrainerOnly";

    public const string GymVisitorOnly = "GymVisitorOnly";
}
