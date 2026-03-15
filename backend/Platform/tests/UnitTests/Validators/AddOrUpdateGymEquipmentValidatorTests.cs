using FitHub.Contracts.V1.Equipments.GymEquipments;
using FitHub.Shared.GymEquipments;
using FitHub.Web.V1.Equipments.Validators;
using Shouldly;
using Xunit;

namespace FitHub.UnitTests.Validators;

public sealed class AddOrUpdateGymEquipmentValidatorTests
{
    private readonly AddOrUpdateGymEquipmentValidator sut = new();

    [Fact(DisplayName = "Valid dto for opened equipment")]
    public void Validate_WhenEquipmentIsOpened_ShouldReturnSuccess()
    {
        var result = sut.Validate(GetRequest());

        result.IsValid.ShouldBe(true);
    }

    [Fact(DisplayName = "Valid dto for inactive equipment")]
    public void Validate_WhenEquipmentIsInactive_ShouldReturnSuccess()
    {
        var request = GetRequest(isActive: false, openedAt: DateTimeOffset.UtcNow);
        var result = sut.Validate(request);

        result.IsValid.ShouldBe(true);
    }

    [Fact(DisplayName = "Invalid dto, inactive without openedAt")]
    public void Validate_WhenInactiveAndOpenedAtIsNull_ShouldReturnError()
    {
        var request = GetRequest(isActive: false);
        var result = sut.Validate(request);

        result.IsValid.ShouldBe(false);
        result.Errors.ShouldHaveSingleItem();
        result.Errors.Single().PropertyName.ShouldBe(nameof(AddOrUpdateGymEquipmentRequest.IsActive));
    }

    private AddOrUpdateGymEquipmentRequest GetRequest(
        string equipmentId = "id1",
        string gymId = "id2",
        bool isActive = true,
        DateTimeOffset? openedAt = null,
        EquipmentCondition? condition = EquipmentCondition.Operational)
    {
        return new AddOrUpdateGymEquipmentRequest
        {
            EquipmentId = equipmentId,
            GymId = gymId,
            IsActive = isActive,
            OpenedAt = openedAt,
            Condition = condition
        };
    }
}
