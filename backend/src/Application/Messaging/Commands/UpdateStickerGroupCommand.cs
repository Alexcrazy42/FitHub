namespace FitHub.Application.Messaging.Commands;

public class UpdateStickerGroupCommand
{
    public string Name { get; init; }

    public UpdateStickerGroupCommand(string name)
    {
        Name = name;
    }
}
