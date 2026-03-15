using FitHub.Authentication;
using FitHub.Shared.Messaging;

namespace FitHub.Application.Messaging.Commands.Attachments;

public class CreateTagUserAttachmentCommand
{
    public string Name { get; init; }

    public TagUserAttachmentType Type { get; init; }

    public IdentityUserId? TaggetUserId { get; set; }

    public CreateTagUserAttachmentCommand(string name, TagUserAttachmentType type)
    {
        Name = name;
        Type = type;
    }
}
