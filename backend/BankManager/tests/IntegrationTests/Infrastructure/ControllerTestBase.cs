using AutoFixture;
using AutoFixture.AutoMoq;
using FitHub.BankManager.Application.Mocks;
using FitHub.BankManager.Clients.Payment;
using FitHub.BankManager.Clients.Tests;
using FitHub.BankManager.Data;
using FitHub.BankManager.Domain;
using FitHub.Common.TestsCommon;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace FitHub.BankManager.IntegrationTests.Infrastructure;

[Trait("Category", IntegrationTestCollection.Category)]
[Collection(IntegrationTestCollection.Name)]
public class ControllerTestsBase : IAsyncLifetime
{
    protected readonly IFixture Autofixture = new Fixture().Customize(new AutoMoqCustomization());

    // сервисы из апи
    private readonly BankManagerDataContext dataContext;

    // клиенты апи
    protected readonly IBankManagerPaymentClient PaymentClient;
    protected readonly ITestClient TestClient;

    // моки зависимостей
    protected readonly Mock<IMockTestService> MockTestService;

    protected ControllerTestsBase(ServerFixture serverFixture)
    {
        dataContext = serverFixture.DataContext;

        PaymentClient = serverFixture.PaymentClient;
        TestClient = serverFixture.TestClient;

        MockTestService = serverFixture.MockTestService;

        CustomizeEntities();
    }


    private async Task ClearOutboxAsync(CancellationToken ct)
    {
        await dataContext.RabbitOutboxMessages.ExecuteDeleteAsync(ct);
    }

    public async Task CheckOutboxExistsAsync(string exchangeName, string routingKey, int messageCount, CancellationToken ct)
    {
        var outboxMessages = await dataContext
            .RabbitOutboxMessages
            .Where(x =>
                x.ExchangeName == exchangeName
                && x.RoutingKey == routingKey
                && x.Status == OutboxMessageStatus.Pending
                && x.PublishedAt == null
                && x.Error == null)
            .ToListAsync(ct);

        if (outboxMessages.Count != messageCount)
        {
            throw new Exception($"Excepted outbox message count: {messageCount}, actual: {outboxMessages.Count}");
        }
    }

    private void CustomizeEntities()
    {
        Autofixture.ResidueCollectors.Add(new NonPublicConstructorBuilder());

        ClassCustomizeApplier.ApplyCustomizes(Autofixture);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        MockTestService.Reset();
        await ClearOutboxAsync(CancellationToken.None);
        await dataContext.DisposeAsync();
    }
}
