namespace FitHub.Contracts.V1.Messaging.Stickers;

public sealed class AddStickerRequest
{
    public string? GroupId { get; set; }

    public string? FileId { get; set; }

    public string? Name { get; set; }
}
