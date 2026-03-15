using FitHub.Authentication;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Contracts.V1.Users;
using FitHub.Contracts.V1.Users.GymAdmins;
using FitHub.Contracts.V1.Users.Trainers;
using FitHub.Contracts.V1.Users.Visitors;
using FitHub.Domain.Users;
using FitHub.Web.V1.Equipments;
using EquipmentResponseExtensions = FitHub.Web.V1.Equipments.EquipmentResponseExtensions;

namespace FitHub.Web.V1.Users;

public static class UserExtensions
{

    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse
        {
            Id = user.Id.ToString(),
            Surname = user.Surname,
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive,
            StartActiveAt = user.StartActiveAt,
            RoleNames = user.UserType.ToRoleNames()
        };
    }

    public static GymAdminResponse ToResponse(this GymAdmin gymAdmin)
    {
        return new GymAdminResponse
        {
            Id = gymAdmin.Id.ToString(),
            User = gymAdmin.User.ToResponse(),
            Gyms = gymAdmin.Gyms.Select(EquipmentResponseExtensions.ToResponse).ToList(),
        };
    }

    public static TrainerResponse ToResponse(this Trainer trainer)
    {
        return new TrainerResponse
        {
            Id = trainer.Id.ToString(),
            User = trainer.User.ToResponse(),
            Gyms = trainer.Gyms.Select(EquipmentResponseExtensions.ToResponse).ToList(),
        };
    }

    public static VisitorResponse ToResponse(this Visitor visitor)
    {
        return new VisitorResponse
        {
            Id = visitor.Id.ToString(),
            User = visitor.User.ToResponse(),
            Gyms = visitor.Gyms.Select(x => x.Gym.ToResponse()).ToList()
        };
    }
}
