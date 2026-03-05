using FitHub.Common.Entities;
using FitHub.Domain.Files;

namespace FitHub.Domain.Messaging;

public class Gif : IEntity<GifId>
{
    private FileEntity? file;

    public Gif(GifId id, string name, FileId fileId)
    {
        Id = id;
        Name = name;
        FileId = fileId;
    }

    public GifId Id { get; }

    public string Name { get; private set; }

    public FileId FileId { get; private set; }

    public FileEntity File
    {
        get => UnexpectedException.ThrowIfNull(file, "Файл неожиданно оказался null");
        private set => file = value;
    }
}
