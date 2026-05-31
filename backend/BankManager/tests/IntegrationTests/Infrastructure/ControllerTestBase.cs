using Xunit;

namespace FitHub.BankManager.IntegrationTests.Infrastructure;

[Trait("Category", IntegrationTestCollection.Category)]
[Collection(IntegrationTestCollection.Name)]
public abstract class ControllerTestsBase : IDisposable
{
    public void Dispose()
    {
        // TODO release managed resources here
    }
}
