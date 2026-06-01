using AutoFixture;
using FitHub.BankManager.Clients.Payment;
using FitHub.BankManager.Domain;
using FitHub.BankManager.IntegrationTests.Infrastructure;
using FitHub.BankManager.Rabbit.Contracts.Payments;
using FitHub.BankManager.Web.Contracts;
using Shouldly;
using Xunit;

namespace FitHub.BankManager.IntegrationTests.Tests;

public class PaymentIntentTests : ControllerTestsBase
{
    private readonly IBankManagerPaymentClient sut;

    public PaymentIntentTests(ServerFixture serverFixture) : base(serverFixture)
    {
        sut = serverFixture.PaymentClient;
    }

    [Fact(DisplayName = "Create intent")]
    public async Task CreateNewIntent_NotExistsAlready_ShouldCreating()
    {
        // arrange
        var request = Autofixture.Create<CreatePaymentIntentRequest>();

        // act
        var response = await sut.CreatePaymentIntentAsync(
            request,
            CancellationToken.None);

        // assert
        response.Status.ShouldBe(PaymentIntentStatus.AwaitingPayment.ToString());
        response.ExternalReference.ShouldBe(request.ExternalReference);
        response.Amount.ShouldBe(request.Amount);
        response.Currency.ShouldBe(request.Currency);
        await CheckOutboxExistsAsync(PaymentStatusChangedMessage.ExchangeName, PaymentStatusChangedMessage.DefaultRoutingKey, 1, CancellationToken.None);
    }
}
