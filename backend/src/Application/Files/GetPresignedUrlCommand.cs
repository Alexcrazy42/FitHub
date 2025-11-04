using Microsoft.AspNetCore.Http;

namespace FitHub.Application.Files;

public class GetPresignedUrlCommand
{
    public required IFormFile File { get; set; }
}
