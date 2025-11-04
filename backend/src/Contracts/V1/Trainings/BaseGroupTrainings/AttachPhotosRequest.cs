namespace FitHub.Contracts.V1.Trainings.BaseGroupTrainings;

public sealed class AttachPhotosRequest
{
    public string? BaseGroupTrainingId { get; set; }

    public string? FileId { get; set; }
}
