using AutoFixture;
using AutoFixture.AutoMoq;

namespace FitHub.BankManager.UnitTests.Domain;

public abstract class DomainTestsBase
{
    protected readonly IFixture AutoFixture = new Fixture().Customize(new AutoMoqCustomization());

    protected DomainTestsBase()
    {
        CustomizeEntities();
    }

    private void CustomizeEntities()
    {
    }
}
