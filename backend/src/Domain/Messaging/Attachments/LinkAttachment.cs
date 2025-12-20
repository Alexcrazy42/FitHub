namespace FitHub.Domain.Messaging.Attachments;

public class LinkAttachment
{
    public LinkAttachment(string url, string? title, string? caption, string? photoUrl)
    {
        Url = url;
        Title = title;
        Caption = caption;
        PhotoUrl = photoUrl;
    }

    public string Url { get; private set; }

    public string? Title { get; private set; }

    public string? Caption { get; private set; }

    public string? PhotoUrl { get; private set; }
}
