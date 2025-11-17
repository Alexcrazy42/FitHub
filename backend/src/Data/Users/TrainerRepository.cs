using FitHub.Application.Common;
using FitHub.Application.Users;
using FitHub.Application.Users.Trainers;
using FitHub.Common.Entities;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Users;

public class TrainerRepository :
    DefaultPendingRepository<Trainer, TrainerId, DataContext>,
    ITrainerRepository
{
    public TrainerRepository(DataContext context) : base(context)
    {
    }

    public async Task<PagedResult<Trainer>> GetAll(PagedQuery query, CancellationToken ct)
    {
        var dbQuery = ReadRaw()
            .Include(x => x.User)
            .AsQueryable();

        var total = await dbQuery.CountAsync(ct);

        dbQuery = dbQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize);

        var items = await dbQuery.ToListAsync(ct);

        return PagedResult<Trainer>.Create(items, totalItems: total, currentPage: query.PageNumber, pageSize: query.PageSize);
    }

    public async Task<Trainer> GetAsync(TrainerId id, CancellationToken ct)
    {
        var trainer = await ReadRaw()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        NotFoundException.ThrowIfNull(trainer, "Тренер не найден!");

        return trainer;
    }
}
