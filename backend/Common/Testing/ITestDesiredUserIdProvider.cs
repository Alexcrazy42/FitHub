using FitHub.Authentication;

namespace FitHub.Common.Testing;

public interface ITestDesiredUserIdProvider
{
    IdentityUserId GetUserId();
}
