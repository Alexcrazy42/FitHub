using FitHub.Common.Entities.Identity;

namespace FitHub.BankManager.Domain;

public class BankAccountId : GuidIdentifier<BankAccountId>, IIdentifierDescription
{
    public BankAccountId(Guid value) : base(value) { }

    public static string EntityTypeName => "Bank account";
    public static string Prefix => FormatPrefix("fithub", "bank-account");
}

public class PaymentIntentId : GuidIdentifier<PaymentIntentId>, IIdentifierDescription
{
    public PaymentIntentId(Guid value) : base(value) { }

    public static string EntityTypeName => "Payment intent";
    public static string Prefix => FormatPrefix("fithub", "payment-intent");
}

public class PaymentOperationId : GuidIdentifier<PaymentOperationId>, IIdentifierDescription
{
    public PaymentOperationId(Guid value) : base(value) { }

    public static string EntityTypeName => "Payment operation";
    public static string Prefix => FormatPrefix("fithub", "payment-operation");
}

public class BankWebhookEventId : GuidIdentifier<BankWebhookEventId>, IIdentifierDescription
{
    public BankWebhookEventId(Guid value) : base(value) { }

    public static string EntityTypeName => "Bank webhook event";
    public static string Prefix => FormatPrefix("fithub", "bank-webhook-event");
}

public class RabbitOutboxMessageId : GuidIdentifier<RabbitOutboxMessageId>, IIdentifierDescription
{
    public RabbitOutboxMessageId(Guid value) : base(value) { }

    public static string EntityTypeName => "Rabbit outbox message";
    public static string Prefix => FormatPrefix("fithub", "rabbit-outbox-message");
}
