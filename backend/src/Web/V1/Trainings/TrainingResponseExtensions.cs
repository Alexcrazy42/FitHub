using FitHub.Contracts.V1.Equipments.MuscleGroups;
using FitHub.Contracts.V1.Trainings;
using FitHub.Contracts.V1.Trainings.BaseGroupTrainings;
using FitHub.Contracts.V1.Trainings.GroupTrainings;
using FitHub.Contracts.V1.Trainings.TrainingTypes;
using FitHub.Contracts.V1.Trainings.VideoTrainings;
using FitHub.Domain.Trainings;
using FitHub.Web.V1.Equipments;
using FitHub.Web.V1.Users;

namespace FitHub.Web.V1.Trainings;

public static class TrainingResponseExtensions
{
    public static IReadOnlyList<MuscleGroupResponse> ToMuscleGroupResponses(this IReadOnlyList<MuscleGroup> muscleGroups)
        => muscleGroups.Select(ToMuscleGroupResponse).ToList();


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
            TrainingTypes = baseGroupTraining.TrainingTypes.Select(ToResponse).ToList(),
            IsActive = baseGroupTraining.IsActive,
            PhotoId = baseGroupTraining.Photos.FirstOrDefault()?.FileId.ToString()
        };
    }

    public static GroupTrainingResponse ToResponse(this GroupTraining groupTraining)
    {
        return new GroupTrainingResponse
        {
            Id = groupTraining.Id.ToString(),
            BaseGroupTraining = groupTraining.BaseGroupTraining.ToResponse(),
            Gym = groupTraining.Gym.ToGymResponse(),
            Trainer = groupTraining.Trainer?.ToResponse(),
            Participants = groupTraining.Participants.Select(UserExtensions.ToResponse).ToList(),
            StartTime = groupTraining.StartTime,
            EndTime = groupTraining.EndTime,
            IsActive = groupTraining.IsActive
        };
    }
}
