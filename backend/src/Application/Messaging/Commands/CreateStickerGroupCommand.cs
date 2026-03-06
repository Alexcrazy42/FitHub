namespace FitHub.Application.Messaging.Commands;

public class CreateStickerGroupCommand
{
    public string Name { get; init; }

    public CreateStickerGroupCommand(string name)
    {
        Name = name;
    }
}
