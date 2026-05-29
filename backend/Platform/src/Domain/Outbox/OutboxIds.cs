using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Outbox;

public class RabbitOutboxMessageId : GuidIdentifier<RabbitOutboxMessageId>, IIdentifierDescription
{
    public RabbitOutboxMessageId(Guid value) : base(value) { }

    public static string EntityTypeName => "Rabbit outbox message";
    public static string Prefix => FormatPrefix("fithub", "rabbit-outbox-message");
}
