using FitHub.Application.Common;
using FitHub.Application.Equipments.Brands;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Common.Utilities.System;
using FitHub.Contracts;
using FitHub.Contracts.V1.Equipments;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments;

public class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository equipmentRepository;
    private readonly IBrandService brandService;
    private readonly IUnitOfWork unitOfWork;

    public EquipmentService(IEquipmentRepository equipmentRepository, IUnitOfWork unitOfWork, IBrandService brandService)
    {
        this.equipmentRepository = equipmentRepository;
        this.unitOfWork = unitOfWork;
        this.brandService = brandService;
    }

    public Task<PagedResult<Equipment>> GetAllAsync(PagedQuery pagedQuery, CancellationToken ct = default)
    {
        return equipmentRepository.GetAllAsync(pagedQuery, ct);
    }

    public async Task<Equipment> GetByIdAsync(EquipmentId id, CancellationToken ct = default)
    {
        var equipment = await equipmentRepository.GetFirstOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(equipment, "Тренажер не был найден!");
        return equipment;
    }

    public async Task<Equipment> CreateAsync(CreateEquipmentRequest request, CancellationToken ct = default)
    {
        var brand = await brandService.GetByIdAsync(BrandId.Parse(request.BrandId), ct);
        var equipment = CreateEquipment(request, brand);
        await equipmentRepository.PendingAddAsync(equipment, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return equipment;
    }

    public async Task<Equipment> UpdateAsync(UpdateEquipmentRequest request, CancellationToken ct = default)
    {
        var equipmentId = EquipmentId.Parse(request.Id);
        var equipment = await GetByIdAsync(equipmentId, ct);
        var brand = await brandService.GetByIdAsync(BrandId.Parse(request.BrandId), ct);
        if (equipment.BrandId.ToString() != request.BrandId)
        {
            ApplyUpdate(equipment, request, brand);
        }
        else
        {
            ApplyUpdate(equipment, request);
        }

        equipmentRepository.PendingUpdate(equipment);
        await unitOfWork.SaveChangesAsync(ct);
        return equipment;
    }

    public async Task DeleteAsync(EquipmentId id, CancellationToken ct = default)
    {
        var equipment = await GetByIdAsync(id, ct);
        equipmentRepository.PendingRemove(equipment);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private Equipment CreateEquipment(CreateEquipmentRequest request, Brand brand)
    {
        var name = ValidationException.ThrowIfNull(request.Name, "Имя обязательно к заполнению!");
        var active = ValidationException.ThrowIfNull(request.IsActive, "request.IsActive != null");
        ValidateInstruction(active, request.InstructionAddBefore);
        return Equipment.Create(name, brand.Id, active,
            request.Description,
            request.AdditionalDescroption,
            request.InstructionAddBefore);
    }

    private void ApplyUpdate(Equipment equipment, UpdateEquipmentRequest request, Brand? brand = null)
    {
        if (brand != null)
        {
            equipment.SetBrand(brand);
        }

        if (request.Name is not null)
        {
            equipment.SetName(request.Name);
        }
        if (request.Description is not null)
        {
            equipment.SetDescription(request.Description);
        }
        if (request.AdditionalDescroption is not null)
        {
            equipment.SetAdditionalDescription(request.AdditionalDescroption);
        }

        if (request.IsActive is not null)
        {
            if (request.IsActive == true)
            {
                ValidateInstruction(request.IsActive.Required(), request.InstructionAddBefore);
            }
            equipment.SetActive(request.IsActive.Required());
        }

        if (request.InstructionAddBefore is not null)
        {
            equipment.SetInstructionAddBefore(request.InstructionAddBefore.Required());
        }
    }

    private void ValidateInstruction(bool isActive, DateOnly? instructionAddBefore)
    {
        if (isActive && instructionAddBefore is null)
        {
            throw new ValidationException("Необходимо добавить дату добавления инструкции, если хотите сразу сделать тренажёр активным!");
        }
    }
}
