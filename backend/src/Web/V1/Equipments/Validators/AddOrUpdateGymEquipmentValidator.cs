using FitHub.Contracts.V1.Equipments.GymEquipments;
using FitHub.Web.Validation;
using FluentValidation;

namespace FitHub.Web.V1.Equipments.Validators;

public class AddOrUpdateGymEquipmentValidator : AbstractValidator<AddOrUpdateGymEquipmentRequest>
{
    public AddOrUpdateGymEquipmentValidator()
    {
        RuleFor(x => x.EquipmentId).MustBe(AddOrUpdateGymEquipmentRequest.ValidateEquipment);
        RuleFor(x => x.GymId).MustBe(AddOrUpdateGymEquipmentRequest.ValidateGym);
        RuleFor(x => x.IsActive).MustBe(AddOrUpdateGymEquipmentRequest.ValidateIsActive);
        RuleFor(x => x.Condition).MustBe(AddOrUpdateGymEquipmentRequest.ValidateCondition);

        RuleFor(x => x).MustBeValid(AddOrUpdateGymEquipmentRequest.ValidateInactiveWithOpenedAt, nameof(AddOrUpdateGymEquipmentRequest.IsActive));
        RuleFor(x => x).MustBeValid(AddOrUpdateGymEquipmentRequest.ValidateActiveWithOpenedAt, nameof(AddOrUpdateGymEquipmentRequest.OpenedAt));
        RuleFor(x => x).MustBeValid(AddOrUpdateGymEquipmentRequest.ValidateMaintenanceWithOpenedAt, nameof(AddOrUpdateGymEquipmentRequest.OpenedAt));
    }
}
