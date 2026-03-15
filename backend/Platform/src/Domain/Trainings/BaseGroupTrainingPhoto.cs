using FitHub.Common.Entities;
using FitHub.Domain.Files;

namespace FitHub.Domain.Trainings;

public class BaseGroupTrainingPhoto : IEntity<BaseGroupTrainingPhotoId>
{
    private BaseGroupTraining? training;
    private FileEntity? file;

    private BaseGroupTrainingPhoto(BaseGroupTrainingPhotoId id, BaseGroupTrainingId trainingId, FileId fileId)
    {
        Id = id;
        TrainingId = trainingId;
        FileId = fileId;
    }

    public BaseGroupTrainingPhotoId Id { get; private set; }

    public BaseGroupTrainingId TrainingId { get; private set; }

    public BaseGroupTraining Training
    {
        get => UnexpectedException.ThrowIfNull(training, "Тренировка неожиданно оказалась null");
        private set => training = value;
    }

    public FileId FileId { get; private set; }

    public FileEntity File
    {
        get => UnexpectedException.ThrowIfNull(file, "Файл неожиданно оказался null");
        private set => file = value;
    }


    public static BaseGroupTrainingPhoto Create(BaseGroupTraining training, FileEntity file)
    {
        return new BaseGroupTrainingPhoto(BaseGroupTrainingPhotoId.New(), training.Id, file.Id)
        {
            Training = training,
            File = file
        };
    }
}
