using FitHub.Application.Common;
using FitHub.Application.Users;
using FitHub.Application.Users.Commands;
using FitHub.Authentication;
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

    public Task<IReadOnlyList<User>> GetUsersAsync(List<IdentityUserId> userIds, CancellationToken ct)
    {
        return ReadRaw()
            .Where(x => userIds.Contains(x.Id))
            .ToReadOnlyListAsync(ct);
    }

    public async Task<PagedResult<User>> GetPagedUsersAsync(GetUserQuery query, PagedQuery pagedQuery, CancellationToken ct)
    {
        var dbQuery = ReadRaw();

        if (query.PartName is not null)
        {
            dbQuery = ReadRaw()
                .Where(x =>
                    x.Name.Contains(query.PartName)
                    || x.Email.Contains(query.PartName)
                    || x.Surname.Contains(query.PartName));
        }

        var totalCount = await dbQuery.CountAsync(ct);

        var items = await dbQuery
            .Skip((pagedQuery.PageNumber - 1) * pagedQuery.PageSize)
            .Take(pagedQuery.PageSize)
            .ToListAsync(ct);

        return PagedResult<User>.Create(items, totalItems: totalCount, currentPage: pagedQuery.PageNumber, pageSize: pagedQuery.PageSize);
    }
}
