using FitHub.Shared.Common;

namespace FitHub.Contracts.V1.Entities;

public class EntityResponse
{
    public string? Id { get; set; }

    public EntityType? Type { get; set; }

    public int? MaxFileCount { get; set; }
}
