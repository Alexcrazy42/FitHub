using FitHub.Application.Common;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Equipments.MuscleGroups;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Trainings.MuscleGroups;

public class MuscleGroupService : IMuscleGroupService
{
    private readonly IMuscleGroupRepository muscleGroupRepository;
    private readonly IUnitOfWork unitOfWork;

    public MuscleGroupService(IMuscleGroupRepository muscleGroupRepository, IUnitOfWork unitOfWork)
    {
        this.muscleGroupRepository = muscleGroupRepository;
        this.unitOfWork = unitOfWork;
    }

    public Task<PagedResult<MuscleGroup>> GetAll(PagedQuery pagedQuery, CancellationToken ct)
    {
        return muscleGroupRepository.GetAll(pagedQuery, ct);
    }

    public Task<IReadOnlyList<MuscleGroup>> GetByIds(List<MuscleGroupId> ids, CancellationToken ct)
    {
        return muscleGroupRepository.GetAllAsync(x => ids.Contains(x.Id), ct);
    }

    public async Task<MuscleGroup> GetById(MuscleGroupId id, CancellationToken ct)
    {
        var muscleGroup = await muscleGroupRepository.GetFirstOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(muscleGroup, "Группа мышц не найдена!");
        return muscleGroup;
    }

    public async Task<MuscleGroup> CreateMuscleGroupAsync(CreateMuscleGroupRequest request, CancellationToken ct)
    {
        var name = ValidationException.ThrowIfNull(request.Name, "Имя не передано!");
        MuscleGroup? parent = null;
        if (request.ParentId is not null)
        {
            var parentMuscleGroup = await muscleGroupRepository.GetFirstOrDefaultAsync(x => x.Id == MuscleGroupId.Parse(request.ParentId), ct);
            parent = parentMuscleGroup;
        }

        var muscleGroup = MuscleGroup.Create(name, parent);
        await muscleGroupRepository.PendingAddAsync(muscleGroup, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return muscleGroup;
    }

    public async Task<MuscleGroup> UpdateMuscleGroupAsync(UpdateMuscleGroupRequest request, CancellationToken ct)
    {
        var muscleGroupId = MuscleGroupId.Parse(request.Id);
        var entity = await GetById(muscleGroupId, ct);
        MuscleGroup? parent = null;
        if (request.ParentId is not null)
        {
            var parentMuscleGroup = await muscleGroupRepository.GetFirstOrDefaultAsync(x => x.Id == MuscleGroupId.Parse(request.ParentId), ct);
            parent = parentMuscleGroup;
        }

        ApplyChanges(entity, request, parent);
        muscleGroupRepository.PendingUpdate(entity);
        await unitOfWork.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteMuscleGroupAsync(MuscleGroupId id, CancellationToken ct)
    {
        var entity = await GetById(id, ct);
        muscleGroupRepository.PendingRemove(entity);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private void ApplyChanges(MuscleGroup muscleGroup, UpdateMuscleGroupRequest request, MuscleGroup? parent)
    {
        if (parent is not null)
        {
            muscleGroup.SetParent(parent);
        }

        var name = ValidationException.ThrowIfNull(request.Name, "Имя не передано!");
        muscleGroup.SetName(name);
    }
}
