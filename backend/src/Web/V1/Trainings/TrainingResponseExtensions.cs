using FitHub.Contracts.V1.Equipments.MuscleGroups;
using FitHub.Domain.Trainings;

namespace FitHub.Web.V1.Trainings;

public static class TrainingResponseExtensions
{
    public static IReadOnlyList<MuscleGroupResponse> ToResponses(this IReadOnlyList<MuscleGroup> muscleGroups)
        => muscleGroups.Select(ToResponse).ToList();

    public static MuscleGroupResponse ToResponse(this MuscleGroup muscleGroup)
    {
        return new MuscleGroupResponse()
        {
            Id = muscleGroup.Id.Value,
            Name = muscleGroup.Name,
            ImageUrl = muscleGroup.ImageUrl,
            ParentId = muscleGroup.ParentId?.Value
        };
    }
}
