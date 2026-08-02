using AutoFixture;
using FitHub.BankManager.Application.Payments;
using FitHub.BankManager.Domain;
using FitHub.Common.TestsCommon;
using Moq;
using Shouldly;
using Xunit;
using Xunit.Sdk;

namespace FitHub.BankManager.UnitTests.Application;

public class BankManagerOutboxPublisherServiceTests : ApplicationTestsBase
{
    private readonly BankManagerOutboxPublisherService sut;
    private const int BatchSize = 10;

    public BankManagerOutboxPublisherServiceTests()
    {
        sut = new BankManagerOutboxPublisherService(
            OutboxRepositoryMock.Object,
            RmqBasicProducerMock.Object,
            UnitOfWorkMock.Object,
            LoggerMockFactory.CreateLogger<BankManagerOutboxPublisherService>().Object);
    }

    [Fact(DisplayName = "Empty outbox")]
    public async Task Publish_WhenOutboxIsNull_NoAction()
    {
        // arrange
        OutboxRepositoryMock
            .Setup(x => x.GetPendingAsync(BatchSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // act
        var result = await sut.PublishPendingAsync(BatchSize, CancellationToken.None);

        // assert
        result.PublishedCount.ShouldBe(0);
        result.FailedCount.ShouldBe(0);
        RmqBasicProducerMock.Verify(x =>
            x.BasicPublishRawJsonAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Publish processes messages with mixed success/failure")]
    public async Task Publish_WhenMixedResults_UpdatesStatusesCorrectly()
    {
        // arrange
        var failedMessage = Autofixture.Create<string>();
        var messages = Autofixture
            .CreateMany<RabbitOutboxMessage>(3)
            .ToList();

        OutboxRepositoryMock
            .Setup(x => x.GetPendingAsync(BatchSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages);

        // именно для второго сообщения падаем
        RmqBasicProducerMock
            .Setup(x => x.BasicPublishRawJsonAsync(
                It.Is<string>(r => r == messages[1].ExchangeName),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(failedMessage));


        // act
        var result = await sut.PublishPendingAsync(BatchSize, CancellationToken.None);

        // assert
        result.PublishedCount.ShouldBe(2);
        result.FailedCount.ShouldBe(1);
        UnitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        var firstMessage = messages[0];
        var secondMessage = messages[1];
        var thirdMessage = messages[2];

        firstMessage.Status.ShouldBe(OutboxMessageStatus.Published);
        secondMessage.Status.ShouldBe(OutboxMessageStatus.Failed);
        secondMessage.Error.ShouldBe(failedMessage);
        thirdMessage.Status.ShouldBe(OutboxMessageStatus.Published);
    }
}
