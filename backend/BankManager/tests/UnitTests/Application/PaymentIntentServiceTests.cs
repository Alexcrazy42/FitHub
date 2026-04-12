using FitHub.BankManager.Application;
using FitHub.BankManager.Application.Payments;
using FitHub.BankManager.Domain;
using FitHub.BankManager.Rabbit.Contracts.Payments;
using Shouldly;
using Xunit;

namespace FitHub.BankManager.UnitTests.Application;

public class PaymentIntentServiceTests
{
    [Fact(DisplayName = "PaymentIntent create is idempotent")]
    public async Task CreateAsync_ShouldReturnExistingIntentForSameIdempotencyKey()
    {
        var existingIntent = PaymentIntent.Create("reservation-1", 100m, "RUB", "key-1");
        var repository = new FakePaymentIntentRepository
        {
            IntentByIdempotencyKey = existingIntent
        };
        var service = new PaymentIntentService(repository, new FakeUnitOfWork());

        var intent = await service.CreateAsync(
            new CreatePaymentIntentCommand("reservation-1", 100m, "RUB", "key-1"),
            CancellationToken.None);

        intent.ShouldBeSameAs(existingIntent);
        repository.AddedPaymentIntents.ShouldBeEmpty();
        repository.AddedOutboxMessages.ShouldBeEmpty();
    }

    [Fact(DisplayName = "PaymentIntent can be completed as paid")]
    public async Task CompleteAsync_ShouldMarkIntentAsPaid()
    {
        var intent = PaymentIntent.Create("reservation-1", 100m, "RUB", "key-1");
        var repository = new FakePaymentIntentRepository
        {
            IntentById = intent
        };
        var service = new PaymentIntentService(repository, new FakeUnitOfWork());

        var result = await service.CompleteAsync(
            new CompletePaymentIntentCommand(intent.Id, true, "event-paid-1", null),
            CancellationToken.None);

        result.Status.ShouldBe(PaymentIntentStatus.Paid);
        repository.AddedWebhookEvents.Single().Status.ShouldBe(PaymentIntentStatus.Paid);
        repository.AddedOutboxMessages.Single().RoutingKey.ShouldBe(PaymentStatusChangedMessage.DefaultRoutingKey);
    }

    [Fact(DisplayName = "PaymentIntent can be completed as failed")]
    public async Task CompleteAsync_ShouldMarkIntentAsFailed()
    {
        var intent = PaymentIntent.Create("reservation-1", 100m, "RUB", "key-1");
        var repository = new FakePaymentIntentRepository
        {
            IntentById = intent
        };
        var service = new PaymentIntentService(repository, new FakeUnitOfWork());

        var result = await service.CompleteAsync(
            new CompletePaymentIntentCommand(intent.Id, false, "event-failed-1", "declined"),
            CancellationToken.None);

        result.Status.ShouldBe(PaymentIntentStatus.Failed);
        result.FailureReason.ShouldBe("declined");
        repository.AddedWebhookEvents.Single().Status.ShouldBe(PaymentIntentStatus.Failed);
        repository.AddedOutboxMessages.Single().RoutingKey.ShouldBe(PaymentStatusChangedMessage.DefaultRoutingKey);
    }

    [Fact(DisplayName = "PaymentIntent duplicate webhook does not add operations")]
    public async Task CompleteAsync_ShouldIgnoreDuplicateWebhook()
    {
        var intent = PaymentIntent.Create("reservation-1", 100m, "RUB", "key-1");
        intent.MarkPaid("event-paid-1");
        var repository = new FakePaymentIntentRepository
        {
            IntentById = intent,
            WebhookEvent = BankWebhookEvent.Create("event-paid-1", intent.Id, PaymentIntentStatus.Paid)
        };
        var service = new PaymentIntentService(repository, new FakeUnitOfWork());

        var result = await service.CompleteAsync(
            new CompletePaymentIntentCommand(intent.Id, true, "event-paid-1", null),
            CancellationToken.None);

        result.Status.ShouldBe(PaymentIntentStatus.Paid);
        repository.AddedWebhookEvents.ShouldBeEmpty();
        repository.AddedOutboxMessages.ShouldBeEmpty();
        intent.Operations.Count.ShouldBe(1);
    }

    private sealed class FakeUnitOfWork : IBankManagerUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return Task.FromResult(1);
        }
    }

    private sealed class FakePaymentIntentRepository : IPaymentIntentRepository
    {
        public PaymentIntent? IntentById { get; init; }

        public PaymentIntent? IntentByIdempotencyKey { get; init; }

        public BankWebhookEvent? WebhookEvent { get; init; }

        public List<PaymentIntent> AddedPaymentIntents { get; } = [];

        public List<BankWebhookEvent> AddedWebhookEvents { get; } = [];

        public List<RabbitOutboxMessage> AddedOutboxMessages { get; } = [];

        public Task<PaymentIntent?> GetByIdAsync(PaymentIntentId id, CancellationToken ct)
        {
            return Task.FromResult(IntentById?.Id == id ? IntentById : null);
        }

        public Task<PaymentIntent?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct)
        {
            return Task.FromResult(IntentByIdempotencyKey?.IdempotencyKey == idempotencyKey ? IntentByIdempotencyKey : null);
        }

        public Task<BankWebhookEvent?> GetWebhookEventAsync(string externalEventId, CancellationToken ct)
        {
            return Task.FromResult(WebhookEvent?.ExternalEventId == externalEventId ? WebhookEvent : null);
        }

        public Task AddPaymentIntentAsync(PaymentIntent intent, CancellationToken ct)
        {
            AddedPaymentIntents.Add(intent);
            return Task.CompletedTask;
        }

        public Task AddWebhookEventAsync(BankWebhookEvent webhookEvent, CancellationToken ct)
        {
            AddedWebhookEvents.Add(webhookEvent);
            return Task.CompletedTask;
        }

        public Task AddOutboxMessageAsync(RabbitOutboxMessage message, CancellationToken ct)
        {
            AddedOutboxMessages.Add(message);
            return Task.CompletedTask;
        }
    }
}
