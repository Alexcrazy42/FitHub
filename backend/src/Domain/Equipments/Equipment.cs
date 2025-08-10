using FitHub.Domain.Trainings;
using FitHub.Common.Entities;

namespace FitHub.Domain.Equipments;

public class Equipment : IEntity<EquipmentId>
{
    private readonly List<MuscleGroup> muscleGroups = [];

    public Equipment(EquipmentId id, string imageUrl, List<MuscleGroup> muscleGroups)
    {
        Id = id;
        this.muscleGroups = muscleGroups;
        ImageUrl = imageUrl;
    }

    public EquipmentId Id { get; }

    /// <summary>
    /// Ссылка на фото
    /// </summary>
    public string ImageUrl { get; private set; }


    public BrandId? BrandId { get; private set; }

    public Brand? Brand { get; set; }

    public DateOnly? InstructionAddBefore { get; private set; }

    public EquipmentInstructionId? InstructionId { get; private set; }

    public EquipmentInstruction? Instruction { get; set; }

    /// <summary>
    /// Группы мышц
    /// </summary>
    public IReadOnlyList<MuscleGroup> MuscleGroups => muscleGroups;

    public void SetBrand(Brand newBrand)
    {
        BrandId = newBrand.Id;
        Brand = newBrand;
    }

    public void SetInstruction(EquipmentInstruction newInstruction)
    {
        Instruction = newInstruction;
        InstructionId = newInstruction.Id;
        InstructionAddBefore = null;
    }

    public void SetImageUrl(string imageUrl)
    {
        ImageUrl = imageUrl;
    }

    public void SetInstructionAddBefore(DateOnly newValue)
    {
        InstructionAddBefore = newValue;
    }

    /// <summary>
    /// Добавить группы мышц для тренажера
    /// </summary>
    public void AddMuscleGroups(IReadOnlyList<MuscleGroup> newMuscleGroups)
        => muscleGroups.AddRange(newMuscleGroups);
}
