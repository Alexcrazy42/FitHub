using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Contracts.V1.Messaging.Messages.Attachments;
using FitHub.Domain.Messaging.Attachments;

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
