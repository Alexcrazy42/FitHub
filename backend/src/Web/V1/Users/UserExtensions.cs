using FitHub.Common.AspNetCore.Accounting;
using FitHub.Contracts.V1.Users;
using FitHub.Contracts.V1.Users.GymAdmins;
using FitHub.Contracts.V1.Users.Trainers;
using FitHub.Domain.Users;
using EquipmentResponseExtensions = FitHub.Web.V1.Equipments.EquipmentResponseExtensions;

namespace FitHub.Web.V1.Users;

public static class UserExtensions
{

    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse()
        {
            Surname = user.Surname,
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive,
            RoleNames = user.UserType.ToRoleNames()
        };
    }

    public static GymAdminResponse ToResponse(this GymAdmin gymAdmin)
    {
        return new GymAdminResponse()
        {
            Id = gymAdmin.Id.ToString(),
            User = gymAdmin.User.ToResponse(),
            Gyms = gymAdmin.Gyms.Select(EquipmentResponseExtensions.ToGymResponse).ToList(),
        };
    }

    public static TrainerResponse ToResponse(this Trainer trainer)
    {
        return new TrainerResponse()
        {
            Id = trainer.Id.ToString(),
            User = trainer.User.ToResponse()
        };
    }
}
