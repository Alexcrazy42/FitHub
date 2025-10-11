using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Files;
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

    public async Task<Stream> DownloadFile(FileId id, CancellationToken ct)
    {
        var file = await fileRepository.GetFirstOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(file, "Файл не найден!");

        var decodedKey = Uri.UnescapeDataString(file.S3Key);
        return await s3FileService.DownloadFileAsync(decodedKey);
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
}
