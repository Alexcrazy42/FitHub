using FitHub.Application.Users;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Users;

namespace FitHub.Data.Users;

public class GymAdminRepository : DefaultPendingRepository<GymAdmin, GymAdminId, DataContext>, IGymAdminRepository
{
    private readonly DataContext context;

    public GymAdminRepository(DataContext context) : base(context)
    {
        this.context = context;
    }
}
