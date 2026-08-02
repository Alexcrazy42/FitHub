using System.Runtime.CompilerServices;
using AutoFixture;
using FitHub.BankManager.Jobs.Consumers.Payments;
using FitHub.BankManager.Rabbit.Contracts.Payments;
using FitHub.BankManager.Web.Contracts;
using Moq;
using Xunit;

namespace FitHub.BankManager.UnitTests.Application.Consumers;

public class PaymentIntentRequestedConsumerTests : ApplicationTestsBase
{
    private readonly PaymentIntentRequestedConsumer sut;

    public PaymentIntentRequestedConsumerTests()
    {
        sut = new PaymentIntentRequestedConsumer(BankManagerPaymentClientMock.Object);
    }

    [Fact(DisplayName = "happy path")]
    public async Task Handle_ValidMessage_HappyPath()
    {
        // arrange
        var message = Autofixture.Create<PaymentIntentRequestedMessage>();

        // act
        await sut.HandleAsync(message, CancellationToken.None);

        // assert
        BankManagerPaymentClientMock.Verify(x => x.CreatePaymentIntentAsync(
            It.Is<CreatePaymentIntentRequest>(
                req => req.ExternalReference == message.ReservationId
                && req.Amount == message.Amount
                && req.IdempotencyKey == message.IdempotencyKey),
            It.IsAny<CancellationToken>()
            ), Times.Once);
    }
}
