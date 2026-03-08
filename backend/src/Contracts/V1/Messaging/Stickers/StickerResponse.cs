namespace FitHub.Contracts.V1.Messaging.Stickers;

public sealed class StickerResponse
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? GroupId { get; set; }

    public string? FileId { get; set; }

    public int? Position { get; set; }
}
