using FitHub.Application.Equipments.Instructions;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Equipments;

namespace FitHub.Data.Equipments.Instructions;

public class EquipmentInstructionRepository : DefaultPendingRepository<EquipmentInstruction, EquipmentInstructionId, DataContext>, IEquipmentInstructionRepository
{
    public EquipmentInstructionRepository(DataContext context) : base(context)
    {
    }
}
