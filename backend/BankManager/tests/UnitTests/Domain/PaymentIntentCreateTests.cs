using FitHub.BankManager.Domain;
using FitHub.Common.Entities;
using Shouldly;
using Xunit;

namespace FitHub.BankManager.UnitTests.Domain;

public class PaymentIntentCreateTests : DomainTestsBase
{
    [Theory(DisplayName = "Отрицательна сумма приводит к исключению")]
    [InlineData(-1)]
    [InlineData(0)]
    public void CreatePaymentIntent_LowerZero_ThrowException(decimal amount)
    {
        // arrange

        // act
        var act = () => Create(amount: amount);

        // assert
        var ex = act.ShouldThrow<ValidationException>();
        ex.Message.ShouldContain("must be positive");
    }

    [Theory(DisplayName = "Создание с положительной суммой")]
    [InlineData(1)]
    public void Create_GreaterThanZero_CreateSuccess(decimal amount)
    {
        // arrange

        // act
        var act = () => Create(amount: amount);
        var result = act();

        // arrange
        act.ShouldNotThrow();
        result.Amount.ShouldBe(amount);
    }

    [Theory(DisplayName = "Пустой external reference")]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_EmptyExternalReference_ThrowException(string externalReference)
    {
        // arrange
        // act
        var act = () => Create(externalReference: externalReference);

        // assert
        var ex = act.ShouldThrow<ValidationException>();
        ex.Message.ShouldContain("ExternalReference is required");
    }

    private PaymentIntent Create(string externalReference = "1", decimal amount = 1, string currency = "RUB", string idempotencyKey = "key")
    {
        return PaymentIntent.Create(externalReference, amount, currency, idempotencyKey);
    }
}
