using FitHub.Application.Common;
using FitHub.Application.Trainings.GroupTrainings;
using FitHub.Common.Entities;
using FitHub.Common.EntityFramework;
using FitHub.Contracts.V1.Trainings.GroupTrainings;
using FitHub.Domain.Equipments;
using FitHub.Domain.Trainings;
using FitHub.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Trainings.GroupTrainings;

internal sealed class GroupTrainingRepository : DefaultPendingRepository<GroupTraining, GroupTrainingId, DataContext>, IGroupTrainingRepository
{
    public GroupTrainingRepository(DataContext context) : base(context)
    {
    }

    protected override IQueryable<GroupTraining> ReadRaw()
    {
        return base.ReadRaw()
            .Include(x => x.Gym)
            .Include(x => x.Trainer)
                .ThenInclude(x => x.User)
            .Include(x => x.Participants)
                .ThenInclude(x => x.User)
            .Include(x => x.BaseGroupTraining)
            .AsQueryable();
    }

    public async Task<PagedResult<GroupTraining>> GetAsync(PagedQuery query, GroupTrainingSearchRequest? searchRequest, CancellationToken ct)
    {
        var dbQuery = ReadRaw();

        if (searchRequest?.GymId is not null)
        {
            dbQuery = dbQuery.Where(x => x.GymId == GymId.Parse(searchRequest.GymId));
        }

        if (searchRequest?.TrainerId is not null)
        {
            dbQuery = dbQuery.Where(x => x.TrainerId == TrainerId.Parse(searchRequest.TrainerId));
        }

        if (searchRequest?.StartTime is not null && searchRequest.EndTime is not null)
        {
            if (searchRequest.StartTime > searchRequest.EndTime)
            {
                throw new ValidationException("Дата начала поиска должна быть раньше чем дата конца!");
            }

            var startUtc = searchRequest.StartTime.Value.UtcDateTime.Date;
            var endUtc = searchRequest.EndTime.Value.UtcDateTime.Date;

            if (startUtc == endUtc)
            {
                var dayStart = new DateTimeOffset(startUtc, TimeSpan.Zero);
                var dayEnd = dayStart.AddDays(1).AddTicks(-1);

                dbQuery = dbQuery.Where(x => x.StartTime >= dayStart && x.StartTime <= dayEnd);
            }
            else
            {
                dbQuery = dbQuery.Where(x => x.StartTime >= searchRequest.StartTime);
                dbQuery = dbQuery.Where(x => x.StartTime <= searchRequest.EndTime);
            }
        }

        var total = await dbQuery.CountAsync(ct);

        var items = await dbQuery
            .OrderBy(x => x.Id)
            .Skip(query.PageSize * (query.PageNumber - 1))
            .Take(query.PageSize)
            .ToReadOnlyListAsync(ct);

        return PagedResult<GroupTraining>.Create(items: items, totalItems: total, currentPage: query.PageNumber, pageSize: query.PageSize);
    }

    public async Task<GroupTraining> GetAsync(GroupTrainingId groupTrainingId, CancellationToken ct)
    {
        var entity = await ReadRaw()
            .FirstOrDefaultAsync(x => x.Id == groupTrainingId, ct);

        NotFoundException.ThrowIfNull(entity, "Тренировка не найдена!");

        return entity;
    }
}
