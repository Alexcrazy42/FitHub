namespace FitHub.BankManager.Application.Payments;

public record CreatePaymentIntentCommand(
    string ExternalReference,
    decimal Amount,
    string Currency,
    string IdempotencyKey);
