using FitHub.Domain.Files;

namespace FitHub.Application.Files;

public interface IFileService
{
    public Task<Stream> DownloadFile(FileId id, CancellationToken ct);

    public Task<PresignedUrlResult> GetPresignedUrlAsync(GetPresignedUrlCommand command, CancellationToken ct);

    public Task ConfirmUploadAsync(List<FileId> fileIds, CancellationToken ct);

    public Task MakeFilesActiveAsync(IReadOnlyList<FileId> fileIds, string entityId, EntityType entityType, CancellationToken ct);
}
