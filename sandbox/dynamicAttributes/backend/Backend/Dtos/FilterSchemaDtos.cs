namespace Backend.Dtos;

public record FilterSchemaResponse
{
    public IReadOnlyList<FilterSchema> StaticFilters { get; init; } = Array.Empty<FilterSchema>();
    public IReadOnlyList<FilterSchema> DynamicFilters { get; init; } = Array.Empty<FilterSchema>();
}

public record FilterSchema
{
    public string Id { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public double? Min { get; init; }
    public double? Max { get; init; }
    public double? Step { get; init; }
    public string? Unit { get; init; }
    public IReadOnlyList<FilterOption>? Options { get; init; }
}

public record FilterOption
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int Count { get; init; }
    public bool Disabled { get; init; }
}
