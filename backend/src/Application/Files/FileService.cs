using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Files;
using FitHub.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Application.Files;

public class FileService : IFileService
{
    private readonly IFileRepository fileRepository;
    private readonly IS3FileService s3FileService;
    private readonly IUnitOfWork unitOfWork;

    public FileService(IFileRepository fileRepository, IS3FileService s3FileService, IUnitOfWork unitOfWork)
    {
        this.fileRepository = fileRepository;
        this.s3FileService = s3FileService;
        this.unitOfWork = unitOfWork;
    }

    public async Task<(Stream stream, string fileName)> DownloadFile(FileId id, CancellationToken ct)
    {
        var file = await GetFile(id, ct);

        var decodedKey = Uri.UnescapeDataString(file.S3Key);
        var stream = await s3FileService.DownloadFileAsync(decodedKey);
        return (stream, file.FileName);
    }

    public async Task<FileEntity> GetFile(FileId id, CancellationToken ct)
    {
        var file = await fileRepository.GetFirstOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(file, "Файл не найден!");
        return file;
    }

    public async Task<IReadOnlyList<FileEntity>> GetFiles(EntityType entityType, string entityId, CancellationToken ct)
    {
        var files = await fileRepository.GetAllAsync(x =>
            x.EntityType == entityType
            && x.EntityId == entityId
            && x.Status == FileStatus.Active, ct);
        return files;
    }

    public async Task<PresignedUrlResult> GetPresignedUrlAsync(GetPresignedUrlCommand command, CancellationToken ct)
    {
        var fileId = FileId.New();
        var fileName = command.File.FileName;
        var objectKey = $"uploads/{fileId}/{fileName}";

        var file = FileEntity.Create(fileId, fileName, objectKey, FileStatus.WaitingUpload);

        await fileRepository.PendingAddAsync(file, ct);

        var url = await s3FileService.GetPresignedUrlAsync(command, file.Id.ToString(), objectKey);

        await unitOfWork.SaveChangesAsync(ct);
        return url;
    }

    public async Task ConfirmUploadAsync(List<FileId> fileIds, CancellationToken ct)
    {
        var files = await fileRepository.GetAllAsync(x => fileIds.Contains(x.Id), ct);
        if (files.Count != fileIds.Count)
        {
            throw new ValidationException("Не все переданные файлы были найдены!");
        }

        foreach (var file in files)
        {
            file.SetStatus(FileStatus.WaitingForActive);
        }
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task MakeFilesActiveAsync(IReadOnlyList<FileId> fileIds, string entityId, EntityType entityType, CancellationToken ct)
    {
        var files = await fileRepository.GetAllAsync(x => fileIds.Contains(x.Id), ct);

        foreach (var file in files)
        {
            file.SetStatus(FileStatus.Active);
            file.SetEntity(entityId, entityType);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteFileAsync(FileId id, CancellationToken ct)
    {
        var file = await fileRepository.GetFirstOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(file);
        fileRepository.PendingRemove(file);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task MakeFileNotActivePendingAsync(EntityType entityType, string entityId, CancellationToken ct)
    {
        var files = await fileRepository.GetAllAsync(x => x.EntityType == entityType && x.EntityId == entityId, ct);
        foreach (var file in files)
        {
            file.SetStatus(FileStatus.NotActive);
        }
    }
}
