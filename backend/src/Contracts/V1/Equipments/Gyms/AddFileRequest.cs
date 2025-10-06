using Microsoft.AspNetCore.Http;

namespace FitHub.Contracts.V1.Equipments.Gyms;

public class AddFileRequest
{
    public string? GymId { get; set; }

    public IFormFile? File { get; set; }
}
