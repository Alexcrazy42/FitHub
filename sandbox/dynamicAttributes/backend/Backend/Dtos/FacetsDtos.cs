namespace Backend.Dtos;

public enum DisplayType
{
    Toggle,
    OneOfList,
    List,
    RangeSlider
}

public record FacetsResponse
{
    public Pagination Pagination { get; init; }
    public int ProductCount { get; init; }
    public string OrderBy { get; init; } = "BY_POPULARITY";
    public IReadOnlyList<Facet> Facets { get; init; } = Array.Empty<Facet>();
}

public record Pagination
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int PagesCount { get; init; }
}

public record Facet
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public DisplayType DisplayType { get; init; }
    public string? DisplaySubType { get; init; }
    public IReadOnlyList<FacetValue> Values { get; init; } = Array.Empty<FacetValue>();
    public bool CollapsedByDefault { get; init; }
}

public record FacetValue
{
    public string? SubqueryReference { get; init; }
    public string? Value { get; init; }
    public bool SelectedByUser { get; init; }
    public bool IsAvailable { get; init; }
    
    // Для range
    public double? From { get; init; }
    public double? To { get; init; }
    public double? Min { get; init; }
    public double? Max { get; init; }
    
    // Метаданные
    public string? UiCaption { get; init; }
    public int? SubQueryColorModelCount { get; init; }
}
