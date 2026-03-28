using System.Text;
using Backend.Data;
using Backend.Dtos;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class FacetService
{
    private readonly ApplicationDbContext _db;

    public FacetService(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Получить фасеты для текущей выборки товаров
    /// </summary>
    public async Task<FacetsResponse> GetFacetsAsync(FacetQuery query, CancellationToken ct = default)
    {
        // 1. Базовый запрос с применёнными фильтрами
        var baseQuery = BuildBaseQuery(query);

        // 2. Общее количество товаров
        var productCount = await baseQuery.CountAsync(ct);

        // 3. Параллельное вычисление фасетов
        var facetsTask = await Task.WhenAll(
            BuildPriceFacetAsync(baseQuery, query, ct),
            BuildBrandFacetAsync(baseQuery, query, ct),
            BuildSizeFacetAsync(baseQuery, query, ct),
            BuildColorFacetAsync(baseQuery, query, ct),
            BuildCategoryFacetAsync(baseQuery, query, ct),
            BuildToggleFacetAsync(baseQuery, query, "hood", "С капюшоном", ct),
            BuildToggleFacetAsync(baseQuery, query, "waterproof", "Водонепроницаемый", ct),
            BuildToggleFacetAsync(baseQuery, query, "smartTV", "Smart TV", ct),
            BuildToggleFacetAsync(baseQuery, query, "5g", "5G", ct)
        );

        var facets = facetsTask.ToList();

        // 4. Построение subqueryReference для каждого значения
        var facetsWithRefs = BuildSubqueryReferences(facets, query);

        return new FacetsResponse
        {
            Pagination = new Pagination
            {
                Page = query.Page,
                PageSize = query.PageSize,
                PagesCount = (int)Math.Ceiling(productCount / (double)query.PageSize)
            },
            ProductCount = productCount,
            OrderBy = query.OrderBy,
            Facets = facetsWithRefs
        };
    }

    private IQueryable<Product> BuildBaseQuery(FacetQuery query)
    {
        var queryable = _db.Products
            .Include(p => p.Categories)
            .ThenInclude(pc => pc.Category)
            .AsQueryable();

        // Фильтр по категории (с учётом иерархии через Closure Table)
        if (query.CategoryId != null)
        {
            var descendantIds = _db.CategoryHierarchies
                .Where(h => h.AncestorId == query.CategoryId)
                .Select(h => h.DescendantId);

            queryable = queryable.Where(p =>
                p.Categories.Any(pc => descendantIds.Contains(pc.CategoryId)));
        }

        // Применить активные фильтры
        if (query.Filters != null)
        {
            foreach (var filter in query.Filters)
            {
                queryable = ApplyFilter(queryable, filter);
            }
        }

        return queryable;
    }

    private IQueryable<Product> ApplyFilter(IQueryable<Product> query, Dtos.ActiveFilter filter)
    {
        return filter.Type switch
        {
            "range" => ApplyRangeFilter(query, filter.Id, filter.Value),
            "toggle" => ApplyToggleFilter(query, filter.Id, filter.Value),
            "checkbox" or "one_of_list" or "radio" => ApplyMultiFilter(query, filter.Id, filter.Value),
            _ => query
        };
    }

    private IQueryable<Product> ApplyRangeFilter(IQueryable<Product> query, string id, Dtos.JsonValue? value)
    {
        if (id == "price" && value?.ArrayValue != null && value.ArrayValue.Length >= 2)
        {
            var min = decimal.Parse(value.ArrayValue[0]);
            var max = decimal.Parse(value.ArrayValue[1]);
            query = query.Where(p => p.Price >= min && p.Price <= max);
        }

        return query;
    }

    private IQueryable<Product> ApplyToggleFilter(IQueryable<Product> query, string id, Dtos.JsonValue? value)
    {
        if (value?.BoolValue == true)
        {
            query = query.Where(p =>
                EF.Functions.JsonExtractPathText(p.Attributes!, id) == "true");
        }
        return query;
    }

    private IQueryable<Product> ApplyMultiFilter(IQueryable<Product> query, string id, Dtos.JsonValue? value)
    {
        if (value?.ArrayValue != null && value.ArrayValue.Length > 0)
        {
            var values = value.ArrayValue;
            query = query.Where(p => values.Contains(EF.Functions.JsonExtractPathText(p.Attributes!, id)));
        }

        return query;
    }

    private async Task<Facet> BuildPriceFacetAsync(IQueryable<Product> baseQuery, FacetQuery query, CancellationToken ct)
    {
        var priceRange = await baseQuery
            .Select(p => p.Price)
            .AggregateAsync(
                new { Min = (double?)null, Max = (double?)null },
                (acc, price) => new
                {
                    Min = acc.Min == null ? (double)price : Math.Min(acc.Min.Value, (double)price),
                    Max = acc.Max == null ? (double)price : Math.Max(acc.Max.Value, (double)price)
                },
                ct);

        var currentValue = baseQuery.Select(p => p.Price).ToList();
        var currentMin = currentValue.Any() ? currentValue.Min() : 0;
        var currentMax = currentValue.Any() ? currentValue.Max() : 0;

        return new Facet
        {
            Id = "price",
            Title = "Цена",
            DisplayType = DisplayType.RangeSlider,
            DisplaySubType = "DEFAULT",
            CollapsedByDefault = false,
            Values = new[]
            {
                new FacetValue
                {
                    Min = priceRange.Min ?? 0,
                    Max = priceRange.Max ?? 0,
                    From = (double)currentMin,
                    To = (double)currentMax
                }
            }
        };
    }

    private async Task<Facet> BuildBrandFacetAsync(IQueryable<Product> baseQuery, FacetQuery query, CancellationToken ct)
    {
        var brands = await baseQuery
            .Select(p => EF.Functions.JsonExtractPathText(p.Attributes!, "brand"))
            .GroupBy(b => b)
            .Where(g => !string.IsNullOrEmpty(g.Key))
            .Select(g => new { Brand = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToListAsync(ct);

        var selectedBrands = query.Filters?.FirstOrDefault(f => f.Id == "brand" && f.Type == "checkbox")?.Value?.ArrayValue ?? Array.Empty<string>();

        return new Facet
        {
            Id = "brand",
            Title = "Бренд",
            DisplayType = DisplayType.List,
            DisplaySubType = "DEFAULT",
            CollapsedByDefault = false,
            Values = brands.Select(b => new FacetValue
            {
                Value = b.Brand,
                UiCaption = b.Brand,
                SubQueryColorModelCount = b.Count,
                IsAvailable = true,
                SelectedByUser = selectedBrands.Contains(b.Brand)
            }).ToList()
        };
    }

    private async Task<Facet> BuildSizeFacetAsync(IQueryable<Product> baseQuery, FacetQuery query, CancellationToken ct)
    {
        var sizes = await baseQuery
            .Select(p => EF.Functions.JsonExtractPathText(p.Attributes!, "size"))
            .GroupBy(s => s)
            .Where(g => !string.IsNullOrEmpty(g.Key))
            .Select(g => new { Size = g.Key, Count = g.Count() })
            .OrderBy(g => g.Size)
            .ToListAsync(ct);

        var selectedSizes = query.Filters?.FirstOrDefault(f => f.Id == "size" && f.Type == "checkbox")?.Value?.ArrayValue ?? Array.Empty<string>();

        return new Facet
        {
            Id = "size",
            Title = "Размер",
            DisplayType = DisplayType.List,
            DisplaySubType = "DEFAULT",
            CollapsedByDefault = false,
            Values = sizes.Select(s => new FacetValue
            {
                Value = s.Size,
                UiCaption = s.Size,
                SubQueryColorModelCount = s.Count,
                IsAvailable = true,
                SelectedByUser = selectedSizes.Contains(s.Size)
            }).ToList()
        };
    }

    private async Task<Facet> BuildColorFacetAsync(IQueryable<Product> baseQuery, FacetQuery query, CancellationToken ct)
    {
        var colors = await baseQuery
            .Select(p => EF.Functions.JsonExtractPathText(p.Attributes!, "color"))
            .GroupBy(c => c)
            .Where(g => !string.IsNullOrEmpty(g.Key))
            .Select(g => new { Color = g.Key, Count = g.Count() })
            .OrderBy(g => g.Color)
            .ToListAsync(ct);

        var selectedColors = query.Filters?.FirstOrDefault(f => f.Id == "color" && f.Type == "checkbox")?.Value?.ArrayValue ?? Array.Empty<string>();

        return new Facet
        {
            Id = "color",
            Title = "Цвет",
            DisplayType = DisplayType.List,
            DisplaySubType = "DEFAULT",
            CollapsedByDefault = false,
            Values = colors.Select(c => new FacetValue
            {
                Value = c.Color,
                UiCaption = c.Color,
                SubQueryColorModelCount = c.Count,
                IsAvailable = true,
                SelectedByUser = selectedColors.Contains(c.Color)
            }).ToList()
        };
    }

    private async Task<Facet> BuildCategoryFacetAsync(IQueryable<Product> baseQuery, FacetQuery query, CancellationToken ct)
    {
        var categories = await baseQuery
            .SelectMany(p => p.Categories)
            .Select(pc => pc.Category)
            .GroupBy(c => new { c.Id, c.Name })
            .Select(g => new { g.Key.Id, g.Key.Name, Count = g.Count() })
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

        return new Facet
        {
            Id = "category",
            Title = "Категория",
            DisplayType = DisplayType.List,
            DisplaySubType = "DEFAULT",
            CollapsedByDefault = true,
            Values = categories.Select(c => new FacetValue
            {
                Value = c.Id.ToString(),
                UiCaption = c.Name,
                SubQueryColorModelCount = c.Count,
                IsAvailable = true
            }).ToList()
        };
    }

    private async Task<Facet> BuildToggleFacetAsync(
        IQueryable<Product> baseQuery, 
        FacetQuery query, 
        string attributeName, 
        string title,
        CancellationToken ct)
    {
        var trueCount = await baseQuery
            .CountAsync(p => EF.Functions.JsonExtractPathText(p.Attributes!, attributeName) == "true", ct);

        var totalCount = await baseQuery.CountAsync(ct);
        var falseCount = totalCount - trueCount;

        var isSelected = query.Filters?.Any(f => 
            f.Id == attributeName && 
            f.Type == "toggle" && 
            f.Value?.BoolValue == true) ?? false;

        return new Facet
        {
            Id = attributeName,
            Title = title,
            DisplayType = DisplayType.Toggle,
            DisplaySubType = "DEFAULT",
            CollapsedByDefault = false,
            Values = new[]
            {
                new FacetValue
                {
                    Value = "true",
                    UiCaption = title,
                    SubQueryColorModelCount = trueCount,
                    IsAvailable = trueCount > 0,
                    SelectedByUser = isSelected
                }
            }
        };
    }

    private List<Facet> BuildSubqueryReferences(List<Facet> facets, FacetQuery query)
    {
        var result = new List<Facet>();

        foreach (var facet in facets)
        {
            var values = new List<FacetValue>();

            foreach (var value in facet.Values)
            {
                var subqueryRef = BuildSubqueryReference(facet, value, query);
                values.Add(value with { SubqueryReference = subqueryRef });
            }

            result.Add(facet with { Values = values });
        }

        return result;
    }

    private string BuildSubqueryReference(Facet facet, FacetValue value, FacetQuery query)
    {
        var sb = new StringBuilder("/api/products/search?");

        // Добавляем категорию
        if (query.CategoryId != null)
        {
            sb.Append($"categoryId={query.CategoryId}&");
        }

        // Добавляем текущие фильтры
        if (query.Filters != null)
        {
            foreach (var filter in query.Filters)
            {
                // Пропускаем текущий фасет (он будет заменён)
                if (filter.Id == facet.Id)
                    continue;

                sb.Append($"filter.{filter.Id}.type={filter.Type}&");

                if (filter.Value?.ArrayValue != null)
                {
                    foreach (var v in filter.Value.ArrayValue)
                    {
                        sb.Append($"filter.{filter.Id}.value={Uri.EscapeDataString(v)}&");
                    }
                }
                else if (filter.Value?.BoolValue == true)
                {
                    sb.Append($"filter.{filter.Id}.value=true&");
                }
                else if (filter.Value?.NumberValue != null)
                {
                    sb.Append($"filter.{filter.Id}.value={filter.Value.NumberValue}&");
                }
            }
        }

        // Добавляем новый фильтр
        sb.Append($"filter.{facet.Id}.type={GetFilterTypeForDisplay(facet.DisplayType)}&");

        if (facet.DisplayType == DisplayType.RangeSlider && value.From != null && value.To != null)
        {
            sb.Append($"filter.{facet.Id}.value[]={value.From}&");
            sb.Append($"filter.{facet.Id}.value[]={value.To}&");
        }
        else if (value.Value != null)
        {
            sb.Append($"filter.{facet.Id}.value={Uri.EscapeDataString(value.Value)}&");
        }

        // Pagination и сортировка
        sb.Append($"page={query.Page}&pageSize={query.PageSize}&orderBy={query.OrderBy}");

        return sb.ToString().TrimEnd('&');
    }

    private string GetFilterTypeForDisplay(DisplayType displayType) => displayType switch
    {
        DisplayType.Toggle => "toggle",
        DisplayType.OneOfList => "radio",
        DisplayType.List => "checkbox",
        DisplayType.RangeSlider => "range",
        _ => "checkbox"
    };
}

public record FacetQuery
{
    public Guid? CategoryId { get; init; }
    public IReadOnlyList<Dtos.ActiveFilter>? Filters { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 36;
    public string OrderBy { get; init; } = "BY_POPULARITY";
}
