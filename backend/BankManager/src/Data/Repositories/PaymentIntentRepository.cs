using FitHub.BankManager.Application.Payments;
using FitHub.BankManager.Domain;
using Microsoft.EntityFrameworkCore;

namespace FitHub.BankManager.Data.Repositories;

public class PaymentIntentRepository : IPaymentIntentRepository
{
    private readonly BankManagerDataContext context;

    public PaymentIntentRepository(BankManagerDataContext context)
    {
        this.context = context;
    }

    public Task<PaymentIntent?> GetByIdAsync(PaymentIntentId id, CancellationToken ct)
    {
        return context.PaymentIntents
            .Include(x => x.Operations)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<PaymentIntent?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken ct)
    {
        return context.PaymentIntents
            .Include(x => x.Operations)
            .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, ct);
    }

    public Task<BankWebhookEvent?> GetWebhookEventAsync(string externalEventId, CancellationToken ct)
    {
        return context.BankWebhookEvents.FirstOrDefaultAsync(x => x.ExternalEventId == externalEventId, ct);
    }

    public async Task AddPaymentIntentAsync(PaymentIntent intent, CancellationToken ct)
    {
        await context.PaymentIntents.AddAsync(intent, ct);
    }

    public async Task AddWebhookEventAsync(BankWebhookEvent webhookEvent, CancellationToken ct)
    {
        await context.BankWebhookEvents.AddAsync(webhookEvent, ct);
    }

    public async Task AddOutboxMessageAsync(RabbitOutboxMessage message, CancellationToken ct)
    {
        await context.RabbitOutboxMessages.AddAsync(message, ct);
    }
}
