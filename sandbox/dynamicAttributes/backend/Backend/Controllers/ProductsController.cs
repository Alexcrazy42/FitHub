using Backend.Data;
using Backend.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly FacetService _facetService;
    private readonly CategoryRepository _categoryRepository;

    public ProductsController(
        ApplicationDbContext db,
        FacetService facetService,
        CategoryRepository categoryRepository)
    {
        _db = db;
        _facetService = facetService;
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Поиск товаров с фильтрами и пагинацией
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<SearchProductsResponse>> Search([FromBody] SearchProductsRequest request, CancellationToken ct)
    {
        var query = _db.Products
            .Include(p => p.Categories)
            .ThenInclude(pc => pc.Category)
            .AsQueryable();

        // Фильтр по категории (с учётом иерархии)
        if (request.CategoryId != null)
        {
            var descendantIds = await _db.CategoryHierarchies
                .Where(h => h.AncestorId == request.CategoryId)
                .Select(h => h.DescendantId)
                .ToListAsync(ct);

            query = query.Where(p =>
                p.Categories.Any(pc => descendantIds.Contains(pc.CategoryId)));
        }

        // Применить фильтры
        if (request.Filters != null)
        {
            foreach (var filter in request.Filters)
            {
                query = ApplyFilter(query, filter);
            }
        }

        var totalCount = await query.CountAsync(ct);
        var products = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = "/images/product-placeholder.png",
                Categories = p.Categories.Select(pc => pc.Category.Name).ToList()
            })
            .ToListAsync(ct);

        return Ok(new SearchProductsResponse
        {
            Products = products,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            PagesCount = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        });
    }

    /// <summary>
    /// Получить фасеты для текущей выборки
    /// </summary>
    [HttpGet("facets")]
    public async Task<ActionResult<FacetsResponse>> GetFacets(
        [FromQuery] Guid? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 36,
        [FromQuery] string orderBy = "BY_POPULARITY",
        CancellationToken ct = default)
    {
        // Собираем активные фильтры из query string
        var filters = ParseFiltersFromQuery();

        var query = new FacetQuery
        {
            CategoryId = categoryId,
            Filters = filters,
            Page = page,
            PageSize = pageSize,
            OrderBy = orderBy
        };

        var facets = await _facetService.GetFacetsAsync(query, ct);
        return Ok(facets);
    }

    /// <summary>
    /// Получить схему фильтров для категории
    /// </summary>
    [HttpGet("filters/schema")]
    public async Task<ActionResult<FilterSchemaResponse>> GetFilterSchema(
        [FromQuery] Guid? categoryId,
        CancellationToken ct)
    {
        var staticFilters = new List<FilterSchema>
        {
            new()
            {
                Id = "price",
                Type = "range",
                Label = "Цена",
                Unit = "₽"
            }
        };

        var dynamicFilters = new List<FilterSchema>();

        // Получаем уникальные бренды для схемы
        var brands = await _db.Products
            .Select(p => EF.Functions.JsonExtractPathText(p.Attributes!, "brand"))
            .Distinct()
            .Where(b => !string.IsNullOrEmpty(b))
            .ToListAsync(ct);

        dynamicFilters.Add(new()
        {
            Id = "brand",
            Type = "checkbox",
            Label = "Бренд",
            Options = brands.Select(b => new FilterOption
            {
                Id = b,
                Name = b,
                Count = 0,
                Disabled = false
            }).ToList()
        });

        // Получаем размеры
        var sizes = await _db.Products
            .Select(p => EF.Functions.JsonExtractPathText(p.Attributes!, "size"))
            .Distinct()
            .Where(s => !string.IsNullOrEmpty(s))
            .ToListAsync(ct);

        dynamicFilters.Add(new()
        {
            Id = "size",
            Type = "checkbox",
            Label = "Размер",
            Options = sizes.Select(s => new FilterOption
            {
                Id = s,
                Name = s,
                Count = 0,
                Disabled = false
            }).ToList()
        });

        // Toggle фильтры
        dynamicFilters.Add(new()
        {
            Id = "hood",
            Type = "toggle",
            Label = "С капюшоном"
        });

        dynamicFilters.Add(new()
        {
            Id = "waterproof",
            Type = "toggle",
            Label = "Водонепроницаемый"
        });

        return Ok(new FilterSchemaResponse
        {
            StaticFilters = staticFilters,
            DynamicFilters = dynamicFilters
        });
    }

    /// <summary>
    /// Получить дерево категорий
    /// </summary>
    [HttpGet("categories/tree")]
    public async Task<ActionResult<List<CategoryNode>>> GetCategoryTree(CancellationToken ct)
    {
        var tree = await _categoryRepository.GetCategoryTreeAsync(ct);
        return Ok(tree);
    }

    private IQueryable<Product> ApplyFilter(IQueryable<Product> query, Dtos.ActiveFilter filter)
    {
        return filter.Type switch
        {
            "range" => ApplyRangeFilter(query, filter.Id, filter.Value),
            "toggle" => ApplyToggleFilter(query, filter.Id, filter.Value),
            "checkbox" or "radio" => ApplyMultiFilter(query, filter.Id, filter.Value),
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

    private List<Dtos.ActiveFilter>? ParseFiltersFromQuery()
    {
        var filters = new List<Dtos.ActiveFilter>();
        
        // Парсим filter.{id}.type и filter.{id}.value[] из query string
        var filterGroups = Request.Query
            .Where(k => k.Key.StartsWith("filter."))
            .GroupBy(k => k.Key.Split('.')[1])
            .ToList();

        foreach (var group in filterGroups)
        {
            var filterId = group.Key;
            var typeKey = $"filter.{filterId}.type";
            var valueKeys = Request.Query.Where(k => k.Key.StartsWith($"filter.{filterId}.value"));

            var type = Request.Query[typeKey].FirstOrDefault();
            if (string.IsNullOrEmpty(type)) continue;

            var values = valueKeys.SelectMany(k => k.Value.ToArray()).ToArray();

            filters.Add(new Dtos.ActiveFilter
            {
                Id = filterId,
                Type = type,
                Value = new Dtos.JsonValue
                {
                    ArrayValue = values.Length > 0 ? values : null,
                    BoolValue = values.Contains("true") ? true : null,
                    StringValue = values.Length == 1 ? values[0] : null
                }
            });
        }

        return filters.Count > 0 ? filters : null;
    }
}
