using FitHub.Application.Common;
using FitHub.Domain.Users;

namespace FitHub.Application.Users.GymAdmins;

public class GymAdminService : IGymAdminService
{
    private readonly IGymAdminRepository gymAdminRepository;

    public GymAdminService(IGymAdminRepository gymAdminRepository)
    {
        this.gymAdminRepository = gymAdminRepository;
    }

    public Task<PagedResult<GymAdmin>> GetAll(PagedQuery query, CancellationToken ct)
    {
        return gymAdminRepository.GetAll(query, ct);
    }
}
