namespace Backend.Dtos;

public record SearchProductsRequest
{
    public Guid? CategoryId { get; init; }
    public IReadOnlyList<ActiveFilter>? Filters { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 36;
    public string OrderBy { get; init; } = "BY_POPULARITY";
}

public record ActiveFilter
{
    public string Id { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public JsonValue? Value { get; init; }
}

public record JsonValue
{
    public string? StringValue { get; init; }
    public double? NumberValue { get; init; }
    public bool? BoolValue { get; init; }
    public string[]? ArrayValue { get; init; }
}

public record SearchProductsResponse
{
    public IReadOnlyList<ProductDto> Products { get; init; } = Array.Empty<ProductDto>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int PagesCount { get; init; }
}

public record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? ImageUrl { get; init; }
    public IReadOnlyList<string> Categories { get; init; } = Array.Empty<string>();
}
