using FitHub.Authentication;

namespace FitHub.Domain.Messaging.Attachments;

public class LeftGroupAttachment
{
    public LeftGroupAttachment(IdentityUserId userId, string userName)
    {
        UserId = userId;
        UserName = userName;
    }

    public IdentityUserId UserId { get; private set; }

    public string UserName { get; private set; }
}
