using FitHub.Common.Entities;
using FitHub.Contracts.V1.Messaging.Messages.Attachments;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Application.Messaging.Commands.Attachments;

public class CreateLinkAttachmentCommand
{
    public string Url { get; init; }

    public string? Title { get; init; }

    public string? Caption { get; init; }

    public string? PhotoUrl { get; init; }

    public CreateLinkAttachmentCommand(string url)
    {
        Url = url;
    }
}
