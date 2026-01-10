using AutoFixture.AutoMoq;
using AutoFixture;
using FitHub.Authentication;

namespace FitHub.UnitTests.Domain;

public abstract class DomainTestsBase
{
    protected readonly IFixture AutoFixture = new Fixture().Customize(new AutoMoqCustomization());

    protected IdentityUserId FirstUserId = IdentityUserId.Parse(Guid.NewGuid().ToString());
    protected IdentityUserId SecondUserId = IdentityUserId.Parse(Guid.NewGuid().ToString());
    protected IdentityUserId ThirdUserId = IdentityUserId.Parse(Guid.NewGuid().ToString());

    // Здесь можно объявить замоканные сервисы Mock<ISomeService>

    protected DomainTestsBase()
    {
        // Здесь можно замокать сервисы с помощью Moq и AutoFixture
        CustomizeEntities();
    }

    private void CustomizeEntities()
    {
    }
}
