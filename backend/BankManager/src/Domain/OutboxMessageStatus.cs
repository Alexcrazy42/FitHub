namespace FitHub.BankManager.Domain;

public enum OutboxMessageStatus
{
    Pending,
    Published,
    Failed
}
