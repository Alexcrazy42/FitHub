using FitHub.Common.AspNetCore.Auth;
using FitHub.Contracts.V1;

namespace FitHub.Web;

public sealed class AuthOptions : IAuthOptions
{
    public static string SectionName => "Auth";

    public string? Issuer { get; set; }

    public string? SecretKey { get; set; }

    public TimeSpan? CookieExpiration { get; set; }

    public string LoginRoute => ApiRoutesV1.Login;
}
