using AutoFixture;
using AutoFixture.AutoMoq;
using FitHub.BankManager.Application.Payments;
using FitHub.Common.TestsCommon;
using FitHub.RabbitMQ.Configuration;
using FitHub.RabbitMQ.Producers;
using Moq;

namespace FitHub.BankManager.UnitTests.Application;

public class ApplicationTestsBase
{
    protected readonly IFixture Autofixture = new Fixture();

    // mocks
    protected readonly Mock<IBankManagerUnitOfWork> UnitOfWorkMock;
    protected readonly Mock<IPaymentIntentRepository> PaymentIntentRepositoryMock;
    protected readonly Mock<IBankManagerOutboxRepository> OutboxRepositoryMock;
    protected readonly Mock<IBasicProducer<RabbitMqClusterOptions>> RmqBasicProducerMock;

    protected ApplicationTestsBase()
    {
        CustomizeEntities();
        UnitOfWorkMock = new Mock<IBankManagerUnitOfWork>();
        PaymentIntentRepositoryMock = new Mock<IPaymentIntentRepository>();
        OutboxRepositoryMock = new Mock<IBankManagerOutboxRepository>();
        RmqBasicProducerMock = new Mock<IBasicProducer<RabbitMqClusterOptions>>();

    }

    protected void SetupUnitOfWorkFailure<TException>(TException exception)
        where TException : Exception, new()
    {
        UnitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
    }

    private void CustomizeEntities()
    {
        // Регаем билдера для создания сущностей без публичного конструктора (подразумевает наличие оного в принципе)
        Autofixture.ResidueCollectors.Add(new NonPublicConstructorBuilder());
    }
}
