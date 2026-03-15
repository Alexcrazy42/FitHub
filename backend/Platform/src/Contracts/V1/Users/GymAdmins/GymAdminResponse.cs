using FitHub.Contracts.V1.Equipments.Gyms;

namespace FitHub.Contracts.V1.Users.GymAdmins;

public class GymAdminResponse
{
    public string? Id { get; set; }

    public UserResponse? User { get; set; }

    public List<GymResponse> Gyms { get; set; } = [];
}
