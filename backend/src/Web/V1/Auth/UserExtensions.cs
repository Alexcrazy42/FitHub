using FitHub.Common.AspNetCore.Accounting;
using FitHub.Contracts.V1.Users;
using FitHub.Domain.Users;

namespace FitHub.Web.V1.Auth;

public static class UserExtensions
{
    public static IdentityUserTypeDto ToDto(this IdentityUserType userType)
    {
        throw new NotImplementedException();
    }

    public static UserResponse ToResponse(this User user)
    {
        throw new NotImplementedException();
    }
}
