using AutoFixture;
using AutoFixture.AutoMoq;
using FitHub.BankManager.Application.Payments;
using FitHub.Common.TestsCommon;
using Moq;

namespace FitHub.BankManager.UnitTests.Application;

public class ApplicationTestsBase
{
    protected readonly IFixture AutoFixture = new Fixture().Customize(new AutoMoqCustomization());

    // mocks
    protected readonly Mock<IBankManagerUnitOfWork> UnitOfWorkMock;
    protected readonly Mock<IPaymentIntentRepository> PaymentIntentRepositoryMock;

    protected ApplicationTestsBase()
    {
        CustomizeEntities();
        UnitOfWorkMock = new Mock<IBankManagerUnitOfWork>();
        PaymentIntentRepositoryMock = new Mock<IPaymentIntentRepository>();
    }

    protected void SetupUnitOfWorkSuccess()
    {
        UnitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    protected void SetupUnitOfWorkFailure<TException>()
        where TException : Exception, new()
    {
        UnitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws<TException>();
    }

    private void CustomizeEntities()
    {
        // Регаем билдера для создания сущностей без публичного конструктора (подразумевает наличие оного в принципе)
        AutoFixture.ResidueCollectors.Add(new NonPublicConstructorBuilder());
    }
}
