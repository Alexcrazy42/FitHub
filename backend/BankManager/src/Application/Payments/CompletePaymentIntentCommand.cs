using FitHub.BankManager.Domain;

namespace FitHub.BankManager.Application.Payments;

public record CompletePaymentIntentCommand(
    PaymentIntentId PaymentIntentId,
    bool Succeeded,
    string ExternalEventId,
    string? FailureReason);
