using FitHub.Contracts.V1.Equipments;
using FitHub.Domain.Equipments;

namespace FitHub.Application.Equipments;

public class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository equipmentRepository;

    public EquipmentService(IEquipmentRepository equipmentRepository)
    {
        this.equipmentRepository = equipmentRepository;
    }

    public Task<Equipment> CreateAsync(CreateEquipmentRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Equipment> UpdateAsync(UpdateEquipmentRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(EquipmentId id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
