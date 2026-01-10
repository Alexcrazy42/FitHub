using FitHub.Authentication;

namespace FitHub.Domain.Messaging.Attachments;

public class TagUserAttachment
{
    public string Name { get; set; }

    public TagUserAttachmentType Type { get; set; }

    public IdentityUserId? TaggedUserId { get; set; }

    public TagUserAttachment(string name, TagUserAttachmentType type, IdentityUserId? taggedUserId)
    {
        Name = name;
        Type = type;
        TaggedUserId = taggedUserId;
    }
}
