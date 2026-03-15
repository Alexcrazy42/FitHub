using Microsoft.AspNetCore.Http;

namespace FitHub.Contracts.V1.Files;

public class GetPresignedUrlRequest
{
    public IFormFile? File { get; set; }
}
