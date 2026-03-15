using FitHub.Application.Common;
using FitHub.Application.Equipments.Brands;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Equipments;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Equipments.Brands;

public class BrandRepository : DefaultPendingRepository<Brand, BrandId, DataContext>, IBrandRepository
{
    public BrandRepository(DataContext context) : base(context)
    {
    }

    public async Task<PagedResult<Brand>> GetAll(SearchBrandCommand command, PagedQuery query, CancellationToken ct)
    {
        var dbQuery = ReadRaw();

        if (command.Name is not null)
        {
            dbQuery = dbQuery.Where(b => b.Name.Contains(command.Name));
        }

        var total = await dbQuery.CountAsync(ct);

        var items = await dbQuery
            .OrderBy(x => x.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return PagedResult<Brand>.Create(items, totalItems: total, currentPage: query.PageNumber, pageSize: query.PageSize);
    }
}
