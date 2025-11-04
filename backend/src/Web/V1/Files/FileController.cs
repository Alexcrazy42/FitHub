using FitHub.Application.Files;
using FitHub.Common.Entities;
using FitHub.Contracts;
using FitHub.Contracts.Common;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Files;
using FitHub.Domain.Files;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Files;

public class FileController : ControllerBase
{
    private readonly IFileService fileService;

    public FileController(IFileService fileService)
    {
        this.fileService = fileService;
    }

    [HttpGet(ApiRoutesV1.FileById)]
    public async Task<IActionResult> GetFile([FromRoute] string? id, CancellationToken ct)
    {
        if (String.IsNullOrEmpty(id))
        {
            throw new ValidationException("Не передан id файла!");
        }
        var fileId = FileId.Parse(id);

        var stream = await fileService.DownloadFile(fileId, ct);

        var contentType = GetContentType(id);

        return File(
            stream,
            contentType,
            fileDownloadName: null
        );
    }

    [HttpGet(ApiRoutesV1.Files)]
    public async Task<ListResponse<FileResponse>> GetFiles([FromQuery] string? entityId, [FromQuery] EntityTypeDto? entityType, CancellationToken ct)
    {
        var entityIdRequired = ValidationException.ThrowIfNull(entityId, "EntityId is required!");
        var entityTypeRequired = ValidationException.ThrowIfNull(entityType, "Тип сущности обязателен!").FromDto();
        var files = await fileService.GetFiles(entityTypeRequired, entityIdRequired, ct);
        return ListResponse<FileResponse>.Create(files.Select(FileExtensions.ToFileResponse).ToList());
    }

    [HttpPost(ApiRoutesV1.FileGetPresignedUrl)]
    [Consumes("multipart/form-data")]
    public async Task<PresignedUrlResponse> GetPresignedFile([FromForm] GetPresignedUrlRequest? request, CancellationToken ct)
    {
        var command = request.ToPresignedUrlCommand();
        var result = await fileService.GetPresignedUrlAsync(command, ct);
        return result.ToPresignedUrlResponse();
    }

    [HttpPost(ApiRoutesV1.FileConfirmUpload)]
    public async Task<ActionResult> ConfirmUpload([FromRoute] string? id, CancellationToken ct)
    {
        var fileId = FileId.Parse(id);
        await fileService.ConfirmUploadAsync([fileId], ct);
        return NoContent();
    }

    [HttpPost(ApiRoutesV1.FileMultipleConfirmUpload)]
    public async Task<ActionResult> MultiplyConfirm([FromBody] IReadOnlyList<string> ids, CancellationToken ct)
    {
        var fileIds = ids.Select(FileId.Parse).ToList();
        await fileService.ConfirmUploadAsync(fileIds, ct);
        return NoContent();
    }

    [HttpPost(ApiRoutesV1.FileMakeFilesActive)]
    public async Task<ActionResult> MakeFilesActive([FromBody] MakeFileActiveRequest? request, CancellationToken ct)
    {
        var ids = request?.FileIds.Select(FileId.Parse).ToList() ?? throw new ValidationException("Файлы не могут быть пустыми!");
        var entityType = ValidationException.ThrowIfNull(request.EntityType, "Не передан entityType").FromDto();
        var entityId = ValidationException.ThrowIfNull(request.EntityId, "EntityId не может быть пустым!");

        await fileService.MakeFilesActiveAsync(ids, entityId, entityType, ct);
        return NoContent();
    }

    [HttpDelete(ApiRoutesV1.FileById)]
    public async Task Delete([FromRoute] string? id, CancellationToken ct)
    {
        var fileId = FileId.Parse(id);
        await fileService.DeleteFileAsync(fileId, ct);
    }


    private string GetContentType(string fileName)
    {
        var extensions = new Dictionary<string, string>
        {
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".gif", "image/gif" },
            { ".pdf", "application/pdf" },
            { ".txt", "text/plain" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }
        };

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return extensions.TryGetValue(ext, out var type) ? type : "application/octet-stream";
    }
}
