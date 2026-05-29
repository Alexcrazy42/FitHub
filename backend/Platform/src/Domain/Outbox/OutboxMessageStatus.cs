namespace FitHub.Domain.Outbox;

public enum OutboxMessageStatus
{
    Pending,
    Published,
    Failed
}
