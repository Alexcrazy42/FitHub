using FitHub.Shared.Common;

namespace FitHub.Contracts.V1.Files;

public class MakeFileActiveRequest
{
    public List<string> FileIds { get; set; } = [];
    public string? EntityId { get; set; }
    public EntityType? EntityType { get; set; }
}
