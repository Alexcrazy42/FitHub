using FitHub.BankManager.Domain;

namespace FitHub.BankManager.Application.Payments;

public interface IPaymentIntentService
{
    Task<PaymentIntent> CreateAsync(CreatePaymentIntentCommand command, CancellationToken ct);

    Task<PaymentIntent?> GetAsync(PaymentIntentId id, CancellationToken ct);

    Task<PaymentIntent> CompleteAsync(CompletePaymentIntentCommand command, CancellationToken ct);
}
