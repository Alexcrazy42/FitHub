using FitHub.Common.Entities.Identity;

namespace FitHub.Domain.Notifications;

public class EmailNotificationId : GuidIdentifier<EmailNotificationId>, IIdentifierDescription
{
    public EmailNotificationId(Guid value) : base(value)
    {
    }

    public static string EntityTypeName => "Почтовое уведомление";
    public static string Prefix => "email-notification";
}
