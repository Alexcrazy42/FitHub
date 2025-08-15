using FitHub.Application.Equipments;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Equipments;

namespace FitHub.Data.Equipments;

public class EquipmentRepository : DefaultPendingRepository<Equipment, EquipmentId, DataContext>, IEquipmentRepository
{
    public EquipmentRepository(DataContext context) : base(context)
    {
    }
}
