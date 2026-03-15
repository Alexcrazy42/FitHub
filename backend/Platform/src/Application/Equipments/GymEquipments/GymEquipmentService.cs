using FitHub.Application.Common;
using FitHub.Application.Equipments.Gyms;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Equipments.GymEquipments;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments.GymEquipments;

internal class GymEquipmentService : IGymEquipmentService
{
    private readonly IGymEquipmentRepository gymEquipmentRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IEquipmentService equipmentService;
    private readonly IGymService gymService;

    public GymEquipmentService(IGymEquipmentRepository gymEquipmentRepository, IUnitOfWork unitOfWork, IEquipmentService equipmentService, IGymService gymService)
    {
        this.gymEquipmentRepository = gymEquipmentRepository;
        this.unitOfWork = unitOfWork;
        this.equipmentService = equipmentService;
        this.gymService = gymService;
    }

    public Task<PagedResult<GymEquipment>> GetAsync(PagedQuery pagedQuery, SearchGymEquipmentRequest? request, CancellationToken ct)
    {
        return gymEquipmentRepository.GetAsync(pagedQuery, request, ct);
    }

    public Task<GymEquipment> GetAsync(GymEquipmentId id, CancellationToken ct)
    {
        return gymEquipmentRepository.GetAsync(id, ct);
    }

    public async Task<GymEquipment> CreateAsync(AddOrUpdateGymEquipmentRequest request, CancellationToken ct)
    {
        var entity = await Create(request, ct);
        await gymEquipmentRepository.PendingAddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<GymEquipment> UpdateAsync(GymEquipmentId id, AddOrUpdateGymEquipmentRequest request, CancellationToken ct)
    {
        var updatedEntity = await Update(id, request, ct);
        gymEquipmentRepository.PendingUpdate(updatedEntity);
        await unitOfWork.SaveChangesAsync(ct);
        return updatedEntity;
    }

    public async Task DeleteAsync(GymEquipmentId id, CancellationToken ct)
    {
        var entity = await GetAsync(id, ct);
        gymEquipmentRepository.PendingRemove(entity);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<GymEquipment> Create(AddOrUpdateGymEquipmentRequest request, CancellationToken ct)
    {
        ValidationException.ThrowIfNull(request.EquipmentId, "Укажите базовый тренажер!");
        var equipment = await equipmentService.GetByIdAsync(EquipmentId.Parse(request.EquipmentId), ct);

        ValidationException.ThrowIfNull(request.GymId, "Укажите зал!");
        var gym = await gymService.GetByIdAsync(GymId.Parse(request.GymId), ct);

        var isActive = ValidationException.ThrowIfNull(request.IsActive, "Укажите активен ли тренажер!");
        var condition = ValidationException.ThrowIfNull(request.Condition, "Укажите состояние транажера!");

        return GymEquipment.Create(equipment, gym, condition, isActive, request.OpenedAt);
    }

    private async Task<GymEquipment> Update(GymEquipmentId id, AddOrUpdateGymEquipmentRequest request, CancellationToken ct)
    {
        var entity = await GetAsync(id, ct);

        ValidationException.ThrowIfNull(request.GymId, "Укажите зал!");
        var gym = await gymService.GetByIdAsync(GymId.Parse(request.GymId), ct);
        entity.SetGym(gym);

        var isActive = ValidationException.ThrowIfNull(request.IsActive, "Укажите активен ли тренажер!");
        entity.SetActive(isActive, request.OpenedAt);

        var condition = ValidationException.ThrowIfNull(request.Condition, "Укажите состояние транажера!");
        entity.SetCondition(condition, request.OpenedAt);

        return entity;
    }
}
