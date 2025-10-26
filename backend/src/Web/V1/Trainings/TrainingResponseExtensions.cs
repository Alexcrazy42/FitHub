using FitHub.Contracts.V1.Equipments.MuscleGroups;
using FitHub.Contracts.V1.Trainings;
using FitHub.Contracts.V1.Trainings.BaseGroupTrainings;
using FitHub.Contracts.V1.Trainings.TrainingTypes;
using FitHub.Contracts.V1.Trainings.VideoTrainings;
using FitHub.Domain.Trainings;

namespace FitHub.Web.V1.Trainings;

public static class TrainingResponseExtensions
{
    public static IReadOnlyList<MuscleGroupResponse> ToMuscleGroupResponses(this IReadOnlyList<MuscleGroup> muscleGroups)
        => muscleGroups.Select(ToMuscleGroupResponse).ToList();

    public static IReadOnlyList<VideoTrainingResponse> ToResponses(this IReadOnlyList<VideoTraining> videoTrainings)
        => videoTrainings.Select(ToResponse).ToList();

    public static IReadOnlyList<TrainingTypeResponse> ToResponses(this IReadOnlyList<TrainingType> trainingTypes)
        => trainingTypes.Select(ToResponse).ToList();

    public static IReadOnlyList<BaseGroupTrainingResponse> ToResponses(this IReadOnlyList<BaseGroupTraining> baseGroupTrainings)
        => baseGroupTrainings.Select(ToResponse).ToList();

    public static MuscleGroupResponse ToMuscleGroupResponse(this MuscleGroup muscleGroup)
    {
        return new MuscleGroupResponse()
        {
            Id = muscleGroup.Id.Value,
            Name = muscleGroup.Name,
            ParentId = muscleGroup.ParentId?.ToString(),
            Childrens = muscleGroup.Childrens.Select(ToMuscleGroupResponse).ToList()
        };
    }

    public static VideoTrainingResponse ToResponse(this VideoTraining videoTraining)
    {
        return new VideoTrainingResponse
        {
            Id = videoTraining.Id.Value,
            Name = videoTraining.Name,
            Description = videoTraining.Description,
            Complexity = videoTraining.Complexity,
            DurationInMinutes = videoTraining.DurationInMinutes,
            TrainingType = videoTraining.TrainingType?.ToResponse(),
            MuscleGroups = videoTraining.MuscleGroups.ToMuscleGroupResponses()
        };
    }

    public static TrainingTypeResponse ToResponse(this TrainingType trainingType)
    {
        return new TrainingTypeResponse
        {
            Id = trainingType.Id.ToString(),
            Name = trainingType.Name,
        };
    }

    public static BaseGroupTrainingResponse ToResponse(this BaseGroupTraining baseGroupTraining)
    {
        return new BaseGroupTrainingResponse
        {
            Id = baseGroupTraining.Id.Value,
            Name = baseGroupTraining.Name,
            Description = baseGroupTraining.Description,
            DurationInMinutes = baseGroupTraining.DurationInMinutes,
            Complexity = baseGroupTraining.Complexity,
            Type = baseGroupTraining.Type?.ToResponse()
        };
    }
}
