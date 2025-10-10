using System.Net;
using FitHub.Application.Files;
using FitHub.Common.Entities;
using FitHub.Contracts.V1;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Files;

public class FileController : ControllerBase
{
    private readonly IFileService fileService;

    public FileController(IFileService fileService)
    {
        this.fileService = fileService;
    }

    [HttpGet(ApiRoutesV1.FileByKey)]
    public async Task<IActionResult> GetFile([FromRoute] string key)
    {
        try
        {
            if (String.IsNullOrEmpty(key))
            {
                return BadRequest("File name is required.");
            }

            var fileStream = await fileService.DownloadFileAsync(key);

            var contentType = GetContentType(key);

            return File(
                fileStream,
                contentType,
                fileDownloadName: null
            );
        }
        catch (FileNotFoundException)
        {
            throw new NotFoundException("Файл не найден!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error downloading file: {ex.Message}");
        }
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
