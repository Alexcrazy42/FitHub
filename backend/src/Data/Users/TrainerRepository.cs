using FitHub.Application.Common;
using FitHub.Application.Users;
using FitHub.Application.Users.Trainers;
using FitHub.Common.AspNetCore.Accounting;
using FitHub.Common.Entities;
using FitHub.Common.EntityFramework;
using FitHub.Contracts.V1.Users.Trainers;
using FitHub.Domain.Equipments;
using FitHub.Domain.Users;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Users;

public class TrainerRepository :
    DefaultPendingRepository<Trainer, TrainerId, DataContext>,
    ITrainerRepository
{
    public TrainerRepository(DataContext context) : base(context)
    {
    }

    protected override IQueryable<Trainer> ReadRaw()
    {
        return base.ReadRaw()
            .Include(x => x.User)
            .Include(x => x.Gyms)
            .AsQueryable();
    }

    public async Task<PagedResult<Trainer>> GetAll(PagedQuery query, TrainerQuery? trainerQuery, CancellationToken ct)
    {
        var dbQuery = ReadRaw();

        if (trainerQuery?.GymId is not null)
        {
            var gymId = GymId.Parse(trainerQuery.GymId);
            dbQuery = dbQuery.Where(x => x.Gyms.Any(g => g.Id == gymId));
        }

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
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        NotFoundException.ThrowIfNull(trainer, "Тренер не найден!");

        return trainer;
    }

    public async Task<Trainer> GetByUserId(IdentityUserId userId, CancellationToken ct)
    {
        var trainer = await ReadRaw()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

        NotFoundException.ThrowIfNull(trainer, "Тренер не найден!");

        return trainer;
    }
}
