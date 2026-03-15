namespace FitHub.Common.AspNetCore.Auth;

public interface IAccessService
{
    Task EnsureHasAnyPolicyAsync(params string[] policies);
}
