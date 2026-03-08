namespace FitHub.Application.Messaging.Commands;

public class UpdateStickerNameCommand
{
    public string Name { get; init; }

    public UpdateStickerNameCommand(string name)
    {
        Name = name;
    }
}
