using FitHub.Application.Common;
using FitHub.Application.Trainings.MuscleGroups;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Contracts.V1.Equipments.Instructions;
using FitHub.Domain.Equipments;
using FitHub.Domain.Trainings;

namespace FitHub.Application.Equipments.Instructions;

public class EquipmentInstructionService : IEquipmentInstructionService
{
    private readonly IEquipmentService equipmentService;
    private readonly IUnitOfWork unitOfWork;
    private readonly IEquipmentInstructionRepository equipmentInstructionRepository;
    private readonly IMuscleGroupService muscleGroupService;

    public EquipmentInstructionService(IUnitOfWork unitOfWork,
        IEquipmentInstructionRepository equipmentInstructionRepository,
        IEquipmentService equipmentService,
        IMuscleGroupService muscleGroupService)
    {
        this.unitOfWork = unitOfWork;
        this.equipmentInstructionRepository = equipmentInstructionRepository;
        this.equipmentService = equipmentService;
        this.muscleGroupService = muscleGroupService;
    }

    public Task<PagedResult<EquipmentInstruction>> GetAll(PagedQuery pagedQuery, CancellationToken ct)
    {
        return equipmentInstructionRepository.GetAll(pagedQuery, ct);
    }

    public async Task<EquipmentInstruction> GetById(EquipmentInstructionId id, CancellationToken ct)
    {
        var entity = await equipmentInstructionRepository.GetFirstOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(entity, "Инструкция не найдена!");
        return entity;
    }

    public async Task<EquipmentInstruction> CreateAsync(CreateEquipmentInstructionRequest request, CancellationToken ct)
    {
        var name = ValidationException.ThrowIfNull(request.Name, "Имя не может быть пустым!");
        var instructionType = ValidationException.ThrowIfNull(request.EquipmentInstructionType, "Тип инструкции не должен быть пустым!");

        var equipment = await equipmentService.GetByIdAsync(EquipmentId.Parse(request.EquipmentId), ct);
        var muscleGroupIds = request.MuscleGroupIds.Select(MuscleGroupId.Parse).ToList();
        var muscleGroups = await muscleGroupService.GetByIds(muscleGroupIds, ct);

        var equipmentInstruction = EquipmentInstruction.Create(name, equipment, instructionType, muscleGroups.ToList(), request.Description, request.AdditionalDescription);

        await equipmentInstructionRepository.PendingAddAsync(equipmentInstruction, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return equipmentInstruction;
    }

    public async Task<EquipmentInstruction> UpdateAsync(UpdateEquipmentInstructionRequest request, CancellationToken ct)
    {
        var instruction = await GetById(EquipmentInstructionId.Parse(request.Id), ct);

        var name = ValidationException.ThrowIfNull(request.Name, "Имя не может быть пустым!");
        var instructionType = ValidationException.ThrowIfNull(request.EquipmentInstructionType, "Тип инструкции не должен быть пустым!");

        var equipment = await equipmentService.GetByIdAsync(instruction.EquipmentId, ct);
        var muscleGroupIds = request.MuscleGroupIds.Select(MuscleGroupId.Parse).ToList();
        var muscleGroups = await muscleGroupService.GetByIds(muscleGroupIds, ct);
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(EquipmentInstructionId id, CancellationToken ct)
    {
        var entity = await GetById(id, ct);
        equipmentInstructionRepository.PendingRemove(entity);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
