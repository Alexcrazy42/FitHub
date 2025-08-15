using FitHub.Application.Trainings.MuscleGroups;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Trainings;

namespace FitHub.Data.Trainings.MuscleGroups;

public class MuscleGroupRepository : DefaultPendingRepository<MuscleGroup, MuscleGroupId, DataContext>, IMuscleGroupRepository
{
    public MuscleGroupRepository(DataContext context) : base(context)
    {
    }
}
