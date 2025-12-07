using FitHub.Domain.Files;
using FitHub.Shared.Common;

namespace FitHub.Application.Files;

public interface IFileService
{
    public Task<Stream> DownloadFile(FileId id, CancellationToken ct);

    public Task<FileEntity> GetFile(FileId id, CancellationToken ct);

    public Task<IReadOnlyList<FileEntity>> GetFiles(EntityType entityType, string entityId, CancellationToken ct);

    public Task<PresignedUrlResult> GetPresignedUrlAsync(GetPresignedUrlCommand command, CancellationToken ct);

    public Task ConfirmUploadAsync(List<FileId> fileIds, CancellationToken ct);

    public Task MakeFilesActiveAsync(IReadOnlyList<FileId> fileIds, string entityId, EntityType entityType, CancellationToken ct);

    public Task DeleteFileAsync(FileId id, CancellationToken ct);

    public Task MakeFileNotActivePendingAsync(EntityType entityType, string entityId, CancellationToken ct);
}
