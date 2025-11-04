using FitHub.Contracts.Common;

namespace FitHub.Contracts.V1.Entities;

public class EntityResponse
{
    public string? Id { get; set; }

    public EntityTypeDto? Type { get; set; }

    public int? MaxFileCount { get; set; }
}
