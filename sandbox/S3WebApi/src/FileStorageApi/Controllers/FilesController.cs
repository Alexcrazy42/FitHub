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
    public async Task<IActionResult> Upload([FromForm] IFormFile? file)
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
            var stream = await _fileService.DownloadFileAsync(key);
            var contentType = "application/octet-stream";
            return File(stream, contentType, key);
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
    public async Task<IActionResult> Replace(string key, [FromForm] IFormFile? file)
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
}