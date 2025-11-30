using FitHub.Application.Equipments.Gyms;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Equipments;

namespace FitHub.Data.Equipments.Gyms;

public class VisitorGymRelationRepository : DefaultPendingNoIdRepository<VisitorGymRelation, DataContext>, IVisitorGymRelationRepository
{
    public VisitorGymRelationRepository(DataContext context) : base(context)
    {
    }
}
