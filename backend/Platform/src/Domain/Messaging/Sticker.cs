using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;
using FitHub.Domain.Files;

namespace FitHub.Domain.Messaging;

public class Sticker : IEntity<StickerId>, IAuditableEntity
{
    private StickerGroup? group;
    private FileEntity? file;

    public Sticker(StickerId id, string name, StickerGroupId groupId, FileId fileId, int position)
    {
        Id = id;
        Name = name;
        GroupId = groupId;
        FileId = fileId;
        Position = position;
    }

    public StickerId Id { get; }

    public string Name { get; private set; }

    public int Position { get; private set; }

    public StickerGroupId GroupId { get; private set; }

    public StickerGroup Group
    {
        get => UnexpectedException.ThrowIfNull(group, "Группа стикеров неожиданно оказалась null");
        private set => group = value;
    }

    public FileId FileId { get; private set; }

    public FileEntity File
    {
        get => UnexpectedException.ThrowIfNull(file, "Файл неожиданно оказался null");
        private set => file = value;
    }

    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; }

    public void SetName(string name) => Name = name;

    public void SetFile(FileId fileId) => FileId = fileId;

    public static Sticker Create(string name, StickerGroup group, FileId fileId, int position)
        => new(StickerId.New(), name, group.Id, fileId, position);
}
