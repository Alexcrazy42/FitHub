using FitHub.Domain.Files;

namespace FitHub.Application.Messaging.Commands;

public class UpdateStickerPhotoCommand
{
    public FileId NewFileId { get; init; }

    public UpdateStickerPhotoCommand(FileId newFileId)
    {
        NewFileId = newFileId;
    }
}
