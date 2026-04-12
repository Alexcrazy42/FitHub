namespace FitHub.BankManager.Domain;

public enum PaymentIntentStatus
{
    AwaitingPayment,
    Processing,
    Paid,
    Failed,
    Expired,
    Cancelled
}
