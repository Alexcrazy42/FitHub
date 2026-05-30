using FitHub.BankManager.Application.Payments;
using FitHub.BankManager.Domain;
using Moq;
using Shouldly;
using Xunit;

namespace FitHub.BankManager.UnitTests.Application;

public class PaymentIntentServiceTests : ApplicationTestsBase
{
    private readonly PaymentIntentService sut;

    public PaymentIntentServiceTests()
    {
        sut = new PaymentIntentService(
            PaymentIntentRepositoryMock.Object,
            UnitOfWorkMock.Object);
    }

    [Fact(DisplayName = "PaymentIntent create is idempotent")]
    public async Task CreateAsync_ShouldReturnExistingIntentForSameIdempotencyKey()
    {
        // arrange
        var existingIntent = CreateDefault();

        PaymentIntentRepositoryMock
            .Setup(x => x.GetByIdempotencyKeyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingIntent);

        // act
        var intent = await sut.CreateAsync(
            new CreatePaymentIntentCommand("reservation-1", 100m, "RUB", "key-1"),
            CancellationToken.None);

        // assert
        intent.ShouldBeSameAs(existingIntent);
    }

    [Fact(DisplayName = "PaymentIntent can be completed as paid")]
    public async Task CompleteAsync_ShouldMarkIntentAsPaid()
    {
        // arrange
        var intent = CreateDefault();
        PaymentIntentRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<PaymentIntentId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(intent);

        // act
        var result = await sut.CompleteAsync(
            new CompletePaymentIntentCommand(intent.Id, true, "event-paid-1", null),
            CancellationToken.None);

        // assert
        result.Status.ShouldBe(PaymentIntentStatus.Paid);
        PaymentIntentRepositoryMock
            .Verify(x =>
                    x.AddOutboxMessageAsync(It.IsAny<RabbitOutboxMessage>(), It.IsAny<CancellationToken>()),
                Times.Once);
        PaymentIntentRepositoryMock
            .Verify(x =>
                    x.AddWebhookEventAsync(It.IsAny<BankWebhookEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact(DisplayName = "PaymentIntent can be completed as failed")]
    public async Task CompleteAsync_ShouldMarkIntentAsFailed()
    {
        // arrange
        var intent = CreateDefault();
        var failureReason = "reason";
        PaymentIntentRepositoryMock.Setup(x => x.GetByIdAsync(intent.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(intent);


        // act
        var result = await sut.CompleteAsync(
            new CompletePaymentIntentCommand(intent.Id, false, "event-failed-1", failureReason),
            CancellationToken.None);

        // assert
        result.Status.ShouldBe(PaymentIntentStatus.Failed);
        result.FailureReason.ShouldBe(failureReason);

        PaymentIntentRepositoryMock
            .Verify(x =>
                x.AddWebhookEventAsync(It.IsAny<BankWebhookEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
        PaymentIntentRepositoryMock
            .Verify(x =>
                    x.AddOutboxMessageAsync(It.IsAny<RabbitOutboxMessage>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact(DisplayName = "PaymentIntent duplicate webhook does not add operations")]
    public async Task CompleteAsync_ShouldIgnoreDuplicateWebhook()
    {
        // arrange
        var intent = CreateDefault();
        var webHookEvent = CreateDefault(intent, PaymentIntentStatus.Paid);
        PaymentIntentRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<PaymentIntentId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(intent);
        intent.MarkPaid("event-paid-1");
        PaymentIntentRepositoryMock.Setup(x => x.GetWebhookEventAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(webHookEvent);

        // act
        var result = await sut.CompleteAsync(
            new CompletePaymentIntentCommand(intent.Id, true, "event-paid-1", null),
            CancellationToken.None);

        // assert
        result.Status.ShouldBe(PaymentIntentStatus.Paid);

        PaymentIntentRepositoryMock.Verify(x => x.AddOutboxMessageAsync(It.IsAny<RabbitOutboxMessage>(), It.IsAny<CancellationToken>()), Times.Never);
        PaymentIntentRepositoryMock.Verify(x => x.AddWebhookEventAsync(It.IsAny<BankWebhookEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        UnitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        intent.Operations.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "Pending payment intents can be auto-completed as paid")]
    public async Task CompletePendingAsync_ShouldMarkAwaitingIntentAsPaid()
    {
        // arrange
        var intent = CreateDefault();
        var batchSize = 50;
        PaymentIntentRepositoryMock
            .Setup(x
                => x.GetAwaitingPaymentCreatedBeforeAsync(It.IsAny<DateTimeOffset>(), batchSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync([intent]);

        // act
        var result = await sut.CompletePendingAsync(DateTimeOffset.UtcNow, 50, CancellationToken.None);

        // assert
        result.CompletedCount.ShouldBe(1);
        intent.Status.ShouldBe(PaymentIntentStatus.Paid);
        PaymentIntentRepositoryMock.Verify(x => x.AddWebhookEventAsync(
            It.Is<BankWebhookEvent>(e =>
                e.ExternalEventId == $"auto-paid:{intent.Id}" &&
                e.PaymentIntentId == intent.Id &&
                e.Status == PaymentIntentStatus.Paid
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    private PaymentIntent CreateDefault(
        string externalReference = "reservation-1",
        decimal amount = 100m,
        string currency = "RUB",
        string idempotencyKey = "key-1")
    {
        return PaymentIntent.Create(externalReference, amount, currency, idempotencyKey);
    }

    private BankWebhookEvent CreateDefault(PaymentIntent paymentIntent, PaymentIntentStatus status)
    {
        return BankWebhookEvent.Create("externalEventId", paymentIntent.Id, status);
    }
}
