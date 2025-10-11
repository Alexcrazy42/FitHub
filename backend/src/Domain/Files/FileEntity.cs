using FitHub.Common.Entities;

namespace FitHub.Domain.Files;

public class FileEntity : IEntity<FileId>
{
    public FileEntity(FileId id, string fileName, string s3Key, FileStatus status, DateTimeOffset createdAt)
    {
        Id = id;
        FileName = fileName;
        S3Key = s3Key;
        Status = status;
        CreatedAt = createdAt;
    }

    public FileId Id { get; }

    public string FileName { get; private set; }

    public string S3Key { get; private set; }

    public string? EntityId { get; private set; }

    public EntityType? EntityType { get; private set; }

    public FileStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? DeletedAt { get; private set; }

    public void SetEntity(string entityId, EntityType entityType)
    {
        EntityId = entityId;
        EntityType = entityType;
    }

    public void SetStatus(FileStatus status) => Status = status;

    public static FileEntity Create(
        FileId id,
        string fileName,
        string s3Key,
        FileStatus status)
    {
        return new FileEntity(id, fileName, s3Key, status, DateTimeOffset.Now);
    }
}
