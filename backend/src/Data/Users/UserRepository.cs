using FitHub.Application.Common;
using FitHub.Application.Users;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Users;

public class UserRepository : DefaultPendingRepository<User, IdentityUserId, DataContext>, IUserRepository
{
    public UserRepository(DataContext context) : base(context)
    {
    }

    public async Task<PagedResult<User>> GetCmsAdminsAsync(PagedQuery query, CancellationToken ct)
    {
        var dbQuery = ReadRaw()
            .Where(x => x.UserType.ToString().Contains(IdentityUserType.CmsAdmin.ToString()))
            .OrderBy(x => x.Id)
            .AsQueryable();

        var totalCount = await dbQuery.CountAsync(ct);

        var items = await dbQuery.ToListAsync(ct);

        return PagedResult<User>.Create(items, totalItems: totalCount, currentPage: query.PageNumber, pageSize: query.PageSize);
    }
}
