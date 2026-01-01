using FitHub.Authentication;

namespace FitHub.Domain.Messaging.Attachments;

public class InitiatorAndTargetUserActionAttachment
{
    public InitiatorAndTargetUserActionAttachment(IdentityUserId initiatorUserId, string initiatorUserName, IdentityUserId targetUserId, string targetUsername)
    {
        InitiatorUserId = initiatorUserId;
        InitiatorUserName = initiatorUserName;
        TargetUserId = targetUserId;
        TargetUsername = targetUsername;
    }

    public IdentityUserId InitiatorUserId { get; private set; }

    public string InitiatorUserName { get; private set; }

    public IdentityUserId TargetUserId { get; private set; }

    public string TargetUsername { get; private set; }
}
