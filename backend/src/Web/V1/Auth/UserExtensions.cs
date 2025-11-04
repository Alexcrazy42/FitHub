using FitHub.Common.AspNetCore.Accounting;
using FitHub.Contracts.V1.Users;
using FitHub.Domain.Users;

namespace FitHub.Web.V1.Auth;

public static class UserExtensions
{

    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse()
        {
            Surname = user.Surname,
            Name = user.Name,
            Email = user.Email,
            RoleNames = user.UserType.ToRoleNames()
        };
    }
}
