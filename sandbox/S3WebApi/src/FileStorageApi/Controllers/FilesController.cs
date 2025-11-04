using System.Net;
using FileStorageApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileStorageService _fileService;

    public FilesController(IFileStorageService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        await using var stream = file.OpenReadStream();
        var key = Path.GetFileName(file.FileName);

        try
        {
            await _fileService.UploadFileAsync(key, stream, file.ContentType);
            return Ok(new { key, message = "File uploaded successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error uploading file: {ex.Message}");
        }
    }

    [HttpGet("download/{key}")]
    public async Task<IActionResult> Download(string key)
    {
        try
        {
            if (string.IsNullOrEmpty(key))
                return BadRequest("File name is required.");

            var fileStream = await _fileService.DownloadFileAsync(key);

            // Определяем Content-Type по расширению
            var contentType = GetContentType(key);
            var fileNameSafe = WebUtility.UrlEncode(key); // для кириллицы

            return File(
                fileStream,
                contentType,
                // Уберите имя файла → браузер не будет считать, что это "скачивание"
                fileDownloadName: null // или null, чтобы не добавлять Content-Disposition
            );
        }
        catch (FileNotFoundException)
        {
            return NotFound($"File '{key}' not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error downloading file: {ex.Message}");
        }
    }

    [HttpDelete("delete/{key}")]
    public async Task<IActionResult> Delete(string key)
    {
        try
        {
            var deleted = await _fileService.DeleteFileAsync(key);
            if (!deleted)
                return NotFound($"File '{key}' not found or already deleted.");

            return Ok(new { message = "File deleted." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting file: {ex.Message}");
        }
    }

    [HttpPut("replace/{key}")]
    public async Task<IActionResult> Replace(string key, IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        if (!await _fileService.FileExistsAsync(key))
            return NotFound($"File '{key}' not found.");

        await using var stream = file.OpenReadStream();
        try
        {
            await _fileService.DeleteFileAsync(key); // Удаляем старый
            await _fileService.UploadFileAsync(key, stream, file.ContentType);
            return Ok(new { message = "File replaced successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error replacing file: {ex.Message}");
        }
    }

    [HttpGet("exists/{key}")]
    public async Task<IActionResult> Exists(string key)
    {
        var exists = await _fileService.FileExistsAsync(key);
        return Ok(new { key, exists });
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