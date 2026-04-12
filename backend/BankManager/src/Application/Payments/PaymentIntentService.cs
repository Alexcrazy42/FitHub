using FitHub.BankManager.Domain;
using FitHub.BankManager.Rabbit.Contracts.Payments;
using FitHub.Common.Entities;
using FitHub.Common.Json;

namespace FitHub.BankManager.Application.Payments;

public class PaymentIntentService : IPaymentIntentService
{
    private readonly IPaymentIntentRepository repository;
    private readonly IBankManagerUnitOfWork unitOfWork;

    public PaymentIntentService(IPaymentIntentRepository repository, IBankManagerUnitOfWork unitOfWork)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<PaymentIntent> CreateAsync(CreatePaymentIntentCommand command, CancellationToken ct)
    {
        var existing = await repository.GetByIdempotencyKeyAsync(command.IdempotencyKey, ct);

        if (existing is not null)
        {
            return existing;
        }

        var intent = PaymentIntent.Create(command.ExternalReference, command.Amount, command.Currency, command.IdempotencyKey);
        await repository.AddPaymentIntentAsync(intent, ct);
        await repository.AddOutboxMessageAsync(CreateOutboxMessage(intent), ct);
        await unitOfWork.SaveChangesAsync(ct);

        return intent;
    }

    public Task<PaymentIntent?> GetAsync(PaymentIntentId id, CancellationToken ct)
    {
        return repository.GetByIdAsync(id, ct);
    }

    public async Task<PaymentIntent> CompleteAsync(CompletePaymentIntentCommand command, CancellationToken ct)
    {
        var intent = await repository.GetByIdAsync(command.PaymentIntentId, ct);

        if (intent is null)
        {
            throw new NotFoundException("PaymentIntent не найден.");
        }

        var existingWebhook = await repository.GetWebhookEventAsync(command.ExternalEventId, ct);

        if (existingWebhook is not null)
        {
            return intent;
        }

        if (command.Succeeded)
        {
            intent.MarkPaid(command.ExternalEventId);
            await repository.AddWebhookEventAsync(
                BankWebhookEvent.Create(command.ExternalEventId, intent.Id, PaymentIntentStatus.Paid),
                ct);
            await repository.AddOutboxMessageAsync(CreateOutboxMessage(intent), ct);
        }
        else
        {
            intent.MarkFailed(command.ExternalEventId, command.FailureReason);
            await repository.AddWebhookEventAsync(
                BankWebhookEvent.Create(command.ExternalEventId, intent.Id, PaymentIntentStatus.Failed, command.FailureReason),
                ct);
            await repository.AddOutboxMessageAsync(CreateOutboxMessage(intent), ct);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return intent;
    }

    private static RabbitOutboxMessage CreateOutboxMessage(PaymentIntent intent)
    {
        var message = new PaymentStatusChangedMessage(
            intent.ExternalReference,
            intent.Id.ToString(),
            intent.Status.ToString(),
            intent.Amount,
            intent.Currency,
            intent.FailureReason);
        var payload = CommonJsonSerializer.Serialize(message);

        return RabbitOutboxMessage.Create(
            PaymentStatusChangedMessage.ExchangeName,
            PaymentStatusChangedMessage.DefaultRoutingKey,
            payload);
    }
}
