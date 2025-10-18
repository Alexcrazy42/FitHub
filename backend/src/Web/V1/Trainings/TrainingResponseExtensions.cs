using FitHub.Contracts.V1.Equipments.MuscleGroups;
using FitHub.Contracts.V1.Trainings;
using FitHub.Contracts.V1.Trainings.BaseGroupTrainings;
using FitHub.Contracts.V1.Trainings.TrainingTypes;
using FitHub.Contracts.V1.Trainings.VideoTrainings;
using FitHub.Domain.Trainings;

namespace FitHub.Web.V1.Trainings;

public static class TrainingResponseExtensions
{
    public static IReadOnlyList<MuscleGroupResponse> ToResponses(this IReadOnlyList<MuscleGroup> muscleGroups)
        => muscleGroups.Select(ToResponse).ToList();

    public static IReadOnlyList<VideoTrainingResponse> ToResponses(this IReadOnlyList<VideoTraining> videoTrainings)
        => videoTrainings.Select(ToResponse).ToList();

    public static IReadOnlyList<TrainingTypeResponse> ToResponses(this IReadOnlyList<TrainingType> trainingTypes)
        => trainingTypes.Select(ToResponse).ToList();

    public static IReadOnlyList<BaseGroupTrainingResponse> ToResponses(this IReadOnlyList<BaseGroupTraining> baseGroupTrainings)
        => baseGroupTrainings.Select(ToResponse).ToList();

    public static MuscleGroupResponse ToResponse(this MuscleGroup muscleGroup)
    {
        return new MuscleGroupResponse()
        {
            Id = muscleGroup.Id.Value,
            Name = muscleGroup.Name,
            ParentId = muscleGroup.ParentId?.Value
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
            MuscleGroups = videoTraining.MuscleGroups.ToResponses()
        };
    }

    public static TrainingTypeResponse ToResponse(this TrainingType trainingType)
    {
        return new TrainingTypeResponse
        {
            Id = trainingType.Id.Value,
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
