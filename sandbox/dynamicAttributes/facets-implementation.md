# Реализация фасетов на основе Sportmaster API

## Анализ JSON Sportmaster

Sportmaster использует эндпоинт: `GET /web-api/v1/catalog/facets/`

### Ключевые наблюдения

1. **URL-зависимые фасеты**: Каждый value содержит `subqueryReference` — готовый URL для следующего запроса
2. **Мульти-фасеты**: Один и тот же `id` может встречаться несколько раз с разными `displayType`
3. **Статусы**: `selectedByUser` (выбрано пользователем), `isAvailable` (есть товары)
4. **Метаданные**: `subQueryColorModelCount` — количество товаров для этого значения

---

## Структура данных

### Request (запрос фильтров)

```json
{
  "categoryId": "cat_obuv",
  "filters": {
    "price": {"from": 749, "to": 41289},
    "prod_kind": ["prod_kind_botinki_i_sapogi"],
    "brand": ["nike"]
  },
  "pagination": {
    "page": 1,
    "pageSize": 36
  },
  "orderBy": "BY_POPULARITY"
}
```

**URL-параметры (query string):**
```
/catalog/brendy/nike/for-man-all/?f-cat=cat_obuv&f-pricefrom=749&f-priceto=41289&f-prod_kind=prod_kind_botinki_i_sapogi
```

### Response (ответ фасетов)

```typescript
interface FacetsResponse {
  pagination: {
    page: number;
    pageSize: number;
    pagesCount: number;
  };
  productCount: number;        // Общее количество товаров
  orderBy: string;             // Сортировка
  facets: Facet[];             // Массив фасетов
}

interface Facet {
  id: string;                  // Идентификатор фильтра (например, "price", "availability")
  title: string;               // Заголовок для UI ("Цена", "Способ получения")
  displayType: DisplayType;    // Тип отображения (см. ниже)
  displaySubType?: string;     // Подтип (опционально)
  values: FacetValue[];        // Значения фасета
  collapsedByDefault: boolean; // Свёрнут ли по умолчанию
}

type DisplayType = 
  | "toggle"           // Переключатель (да/нет)
  | "one_of_list"      // Радиокнопки (один из списка)
  | "list"             // Чекбоксы (множественный выбор)
  | "range_slider"     // Диапазон (мин-макс)
  | "color"            // Цветовые свотчи
  | "size"             // Размеры
  ;

interface FacetValue {
  // Для навигации
  subqueryReference: string;  // Готовый URL для следующего запроса
  
  // Значение
  value: string;              // Идентификатор значения
  selectedByUser: boolean;    // Выбрано ли пользователем
  isAvailable: boolean;       // Доступно ли (есть товары)
  
  // Для range_slider
  from?: number;
  to?: number;
  min?: number;
  max?: number;
  
  // Метаданные
  uiCaption?: string;         // Отображаемое название
  subQueryColorModelCount?: number;  // Количество товаров
  
  // Для магазинов (сложные объекты)
  shop?: ShopInfo;
}

interface ShopInfo {
  number: string;
  name: string;
  address: string;
  metroStations: MetroStation[];
  type: string;
  typeName: string;
  weekSchedule: WeekSchedule[];
  geoPoint: { lat: number; lon: number };
}
```

---

## Типы дисплея (DisplayType)

### 1. `toggle` — Переключатель

**Использование**: Да/Нет фильтры (например, "В наличии", "Со скидкой")

```json
{
  "id": "availability",
  "title": "Способ получения",
  "displayType": "toggle",
  "displaySubType": "DELIVERY",
  "values": [
    {
      "subqueryReference": "/catalog/...?f-availability=delivery",
      "value": "delivery",
      "selectedByUser": false,
      "isAvailable": true
    }
  ],
  "collapsedByDefault": false
}
```

**UI компонент:**
```tsx
<Toggle
  checked={value.selectedByUser}
  disabled={!value.isAvailable}
  onChange={() => applyFilter(value.subqueryReference)}
/>
```

---

### 2. `one_of_list` — Радиокнопки

**Использование**: Выбор одного значения из списка

```json
{
  "id": "availability",
  "title": "Способ получения",
  "displayType": "one_of_list",
  "displaySubType": "TYPE_OF_PICKUP",
  "values": [
    {
      "subqueryReference": "/catalog/...?f-availability=pickup",
      "value": "pickup",
      "selectedByUser": false,
      "isAvailable": true,
      "uiCaption": "Завтра или позже",
      "subQueryColorModelCount": 2
    }
  ]
}
```

**UI компонент:**
```tsx
<Radio.Group value={selectedValue}>
  {values.map(v => (
    <Radio
      key={v.value}
      value={v.value}
      disabled={!v.isAvailable}
      label={`${v.uiCaption} (${v.subQueryColorModelCount})`}
      onChange={() => applyFilter(v.subqueryReference)}
    />
  ))}
</Radio.Group>
```

---

### 3. `list` — Чекбоксы

**Использование**: Множественный выбор (бренды, категории, магазины)

```json
{
  "id": "prod_kind",
  "title": "Тип товара",
  "displayType": "list",
  "displaySubType": "DEFAULT",
  "values": [
    {
      "subqueryReference": "/catalog/...?f-prod_kind=...prod_kind_krossovki_i_kedy",
      "value": "prod_kind_krossovki_i_kedy",
      "selectedByUser": false,
      "isAvailable": true,
      "uiCaption": "Кроссовки и кеды",
      "subQueryColorModelCount": 830
    }
  ]
}
```

**UI компонент:**
```tsx
<Checkbox.Group value={selectedValues}>
  {values.map(v => (
    <Checkbox
      key={v.value}
      value={v.value}
      disabled={!v.isAvailable}
      label={`${v.uiCaption} (${v.subQueryColorModelCount})`}
      onChange={() => toggleFilter(v.subqueryReference)}
    />
  ))}
</Checkbox.Group>
```

---

### 4. `range_slider` — Диапазон

**Использование**: Числовые диапазоны (цена, размер, вес)

```json
{
  "id": "price",
  "title": "Цена",
  "displayType": "range_slider",
  "displaySubType": "DEFAULT",
  "values": [
    {
      "from": 5369.0,
      "to": 29235.0,
      "min": 5369.0,
      "max": 29235.0,
      "subqueryReference": "/catalog/...?f-pricefrom={facetValue}&f-priceto={facetValue}",
    }
  ],
  "collapsedByDefault": false,
  "subqueryWoFacetVals": "/catalog/... без фильтров цены"
}
```

**UI компонент:**
```tsx
<RangeSlider
  min={values[0].min}
  max={values[0].max}
  value={[currentFrom, currentTo]}
  onChange={([from, to]) => {
    const url = values[0].subqueryReference
      .replace('{facetValue}', String(from))
      .replace('{facetValue}', String(to));
    applyFilter(url);
  }}
/>
```

---

## Архитектура реализации

### Backend (.NET)

#### 1. Модели

```csharp
// FacetsResponse.cs
public record FacetsResponse
{
    public Pagination Pagination { get; init; }
    public int ProductCount { get; init; }
    public string OrderBy { get; init; }
    public IReadOnlyList<Facet> Facets { get; init; }
}

public record Pagination
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int PagesCount { get; init; }
}

public record Facet
{
    public string Id { get; init; }
    public string Title { get; init; }
    public DisplayType DisplayType { get; init; }
    public string? DisplaySubType { get; init; }
    public IReadOnlyList<FacetValue> Values { get; init; }
    public bool CollapsedByDefault { get; init; }
}

public enum DisplayType
{
    Toggle,
    OneOfList,
    List,
    RangeSlider,
    Color,
    Size
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
    
    // Для магазинов
    public ShopInfo? Shop { get; init; }
}
```

#### 2. Сервис фасетов

```csharp
// FacetService.cs
public class FacetService
{
    private readonly ApplicationDbContext _db;
    
    public async Task<FacetsResponse> GetFacetsAsync(FacetQuery query)
    {
        // 1. Базовый запрос с применёнными фильтрами
        var baseQuery = BuildBaseQuery(query);
        
        // 2. Общее количество товаров
        var productCount = await baseQuery.CountAsync();
        
        // 3. Построение фасетов
        var facets = new List<Facet>();
        
        // 3a. Ценовой фасет
        facets.Add(await BuildPriceFacetAsync(baseQuery, query));
        
        // 3b. Фасет категорий
        facets.Add(await BuildCategoryFacetAsync(baseQuery, query));
        
        // 3c. Динамические атрибуты из JSONB
        facets.AddRange(await BuildDynamicFacetsAsync(baseQuery, query));
        
        // 4. Построение subqueryReference для каждого значения
        var facetsWithRefs = BuildSubqueryReferences(facets, query);
        
        return new FacetsResponse
        {
            Pagination = new Pagination { ... },
            ProductCount = productCount,
            OrderBy = query.OrderBy,
            Facets = facetsWithRefs
        };
    }
    
    private IQueryable<Product> BuildBaseQuery(FacetQuery query)
    {
        var queryable = _db.Products.AsQueryable();
        
        // Фильтр по категории (с учётом иерархии)
        if (query.CategoryId != null)
        {
            queryable = queryable.Where(p => 
                p.Categories.Any(c => c.Id == query.CategoryId) ||
                p.Categories.Any(c => _db.CategoryHierarchy
                    .Any(h => h.AncestorId == query.CategoryId && h.DescendantId == c.Id)));
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
    
    private async Task<Facet> BuildPriceFacetAsync(IQueryable<Product> baseQuery, FacetQuery query)
    {
        // Получаем мин/макс цену для текущей выборки
        var priceRange = await baseQuery
            .Select(p => p.Price)
            .AggregateAsync(
                new { Min = (double?)null, Max = (double?)null },
                (acc, price) => new
                {
                    Min = acc.Min == null ? price : Math.Min(acc.Min.Value, price),
                    Max = acc.Max == null ? price : Math.Max(acc.Max.Value, price)
                });
        
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
                    Min = priceRange.Min,
                    Max = priceRange.Max,
                    From = query.Filters?.GetPriceFrom() ?? priceRange.Min,
                    To = query.Filters?.GetPriceTo() ?? priceRange.Max
                }
            }
        };
    }
    
    private async Task<Facet> BuildDynamicFacetAsync(
        IQueryable<Product> baseQuery, 
        string attributeName,
        string title,
        DisplayType displayType)
    {
        // Агрегация по JSONB атрибуту
        var values = await baseQuery
            .Select(p => EF.Functions.JsonExtractPathText(p.Attributes, attributeName))
            .GroupBy(v => v)
            .Select(g => new { Value = g.Key, Count = g.Count() })
            .ToListAsync();
        
        return new Facet
        {
            Id = attributeName,
            Title = title,
            DisplayType = displayType,
            Values = values
                .Where(v => v.Value != null)
                .Select(v => new FacetValue
                {
                    Value = v.Value,
                    UiCaption = v.Value,
                    SubQueryColorModelCount = v.Count,
                    IsAvailable = true
                })
                .ToList()
        };
    }
}
```

#### 3. Контроллер

```csharp
// FacetsController.cs
[ApiController]
[Route("api/[controller]")]
public class FacetsController : ControllerBase
{
    private readonly FacetService _facetService;
    
    [HttpGet]
    public async Task<ActionResult<FacetsResponse>> GetFacets(
        [FromQuery] FacetQuery query,
        CancellationToken ct)
    {
        var facets = await _facetService.GetFacetsAsync(query, ct);
        return Ok(facets);
    }
}

// FacetQuery.cs
public record FacetQuery
{
    public Guid? CategoryId { get; init; }
    public FilterCollection? Filters { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 36;
    public string OrderBy { get; init; } = "BY_POPULARITY";
}

public record FilterCollection
{
    public Dictionary<string, string[]> Values { get; init; }
    
    public double? GetPriceFrom() => 
        Values.TryGetValue("price", out var v) ? double.Parse(v[0]) : null;
    
    public double? GetPriceTo() => 
        Values.TryGetValue("price", out var v) ? double.Parse(v[1]) : null;
}
```

---

### Frontend (React + TypeScript)

#### 1. Хук для работы с фасетами

```typescript
// hooks/useFacets.ts
import { useQuery, useMutation } from '@tanstack/react-query';

interface UseFacetsOptions {
  categoryId?: string;
  filters?: Record<string, string | string[]>;
  enabled?: boolean;
}

export function useFacets(options: UseFacetsOptions = {}) {
  const { categoryId, filters, enabled = true } = options;
  
  // Загрузка фасетов
  const { data, isLoading, refetch } = useQuery({
    queryKey: ['facets', categoryId, filters],
    queryFn: async () => {
      const params = new URLSearchParams();
      if (categoryId) params.set('categoryId', categoryId);
      if (filters) {
        Object.entries(filters).forEach(([key, value]) => {
          if (Array.isArray(value)) {
            value.forEach(v => params.append(key, v));
          } else {
            params.set(key, value);
          }
        });
      }
      
      const response = await fetch(`/api/facets?${params}`);
      return response.json() as Promise<FacetsResponse>;
    },
    enabled,
  });
  
  // Применение фильтра
  const applyFilter = useCallback((subqueryReference: string) => {
    // Извлекаем параметры из subqueryReference
    const url = new URL(subqueryReference, window.location.origin);
    const newFilters = Object.fromEntries(url.searchParams);
    
    // Обновляем URL и делаем refetch
    window.history.pushState({}, '', `${url.pathname}${url.search}`);
    refetch();
  }, [refetch]);
  
  // Сброс фильтров
  const clearFilters = useCallback(() => {
    window.history.pushState({}, '', window.location.pathname);
    refetch();
  }, [refetch]);
  
  // Построение URL для фильтра
  const buildFilterUrl = useCallback((facetId: string, value: string) => {
    const params = new URLSearchParams(window.location.search);
    params.append(`f-${facetId}`, value);
    return `${window.location.pathname}?${params}`;
  }, []);
  
  return {
    facets: data?.facets ?? [],
    productCount: data?.productCount ?? 0,
    pagination: data?.pagination,
    isLoading,
    applyFilter,
    clearFilters,
    buildFilterUrl,
  };
}
```

#### 2. Универсальный рендерер фасетов

```typescript
// components/FacetRenderer.tsx
import { Toggle } from './Toggle';
import { RadioGroup } from './RadioGroup';
import { CheckboxGroup } from './CheckboxGroup';
import { RangeSlider } from './RangeSlider';

interface FacetRendererProps {
  facet: Facet;
  onApplyFilter: (subqueryReference: string) => void;
}

export function FacetRenderer({ facet, onApplyFilter }: FacetRendererProps) {
  const renderByType = () => {
    switch (facet.displayType) {
      case 'toggle':
        return renderToggle(facet);
      case 'one_of_list':
        return renderRadio(facet);
      case 'list':
        return renderCheckbox(facet);
      case 'range_slider':
        return renderRangeSlider(facet);
      default:
        return null;
    }
  };
  
  const renderToggle = (facet: Facet) => (
    <Toggle
      key={facet.id}
      title={facet.title}
      value={facet.values.find(v => v.selectedByUser)?.isAvailable ?? false}
      onChange={() => {
        const value = facet.values[0];
        if (value?.subqueryReference) {
          onApplyFilter(value.subqueryReference);
        }
      }}
    />
  );
  
  const renderRadio = (facet: Facet) => (
    <RadioGroup
      key={facet.id}
      title={facet.title}
      value={facet.values.find(v => v.selectedByUser)?.value}
      options={facet.values.map(v => ({
        value: v.value!,
        label: `${v.uiCaption ?? v.value} (${v.subQueryColorModelCount ?? 0})`,
        disabled: !v.isAvailable,
      }))}
      onChange={(value) => {
        const facetValue = facet.values.find(v => v.value === value);
        if (facetValue?.subqueryReference) {
          onApplyFilter(facetValue.subqueryReference);
        }
      }}
    />
  );
  
  const renderCheckbox = (facet: Facet) => (
    <CheckboxGroup
      key={facet.id}
      title={facet.title}
      value={facet.values.filter(v => v.selectedByUser).map(v => v.value!)}
      options={facet.values.map(v => ({
        value: v.value!,
        label: `${v.uiCaption ?? v.value} (${v.subQueryColorModelCount ?? 0})`,
        disabled: !v.isAvailable,
      }))}
      onChange={(values) => {
        // Для множественного выбора нужно построить новый URL
        const selectedValues = facet.values
          .filter(v => values.includes(v.value!))
          .map(v => v.value!)
          .join(',');
        
        const params = new URLSearchParams(window.location.search);
        params.set(`f-${facet.id}`, selectedValues);
        onApplyFilter(`${window.location.pathname}?${params}`);
      }}
    />
  );
  
  const renderRangeSlider = (facet: Facet) => {
    const value = facet.values[0];
    if (!value) return null;
    
    return (
      <RangeSlider
        key={facet.id}
        title={facet.title}
        min={value.min!}
        max={value.max!}
        value={[value.from!, value.to!]}
        onChange={([from, to]) => {
          // Строим URL с новыми значениями
          const params = new URLSearchParams(window.location.search);
          params.set('f-pricefrom', String(from));
          params.set('f-priceto', String(to));
          onApplyFilter(`${window.location.pathname}?${params}`);
        }}
      />
    );
  };
  
  return (
    <div className="facet" data-collapsed={facet.collapsedByDefault}>
      <h3 className="facet-title">{facet.title}</h3>
      {renderByType()}
    </div>
  );
}
```

#### 3. Компонент списка товаров с фасетами

```typescript
// pages/Products.tsx
import { useFacets } from '../hooks/useFacets';
import { FacetRenderer } from '../components/FacetRenderer';
import { ProductList } from '../components/ProductList';
import { ActiveFiltersChips } from '../components/ActiveFiltersChips';

export function ProductsPage() {
  const { categoryId } = useParams();
  
  const {
    facets,
    productCount,
    pagination,
    isLoading,
    applyFilter,
    clearFilters,
  } = useFacets({ categoryId });
  
  return (
    <div className="products-page">
      <aside className="filters-sidebar">
        {facets.map(facet => (
          <FacetRenderer
            key={`${facet.id}-${facet.displayType}`}
            facet={facet}
            onApplyFilter={applyFilter}
          />
        ))}
      </aside>
      
      <main className="products-main">
        <header className="products-header">
          <h1>Товары ({productCount})</h1>
          <ActiveFiltersChips onClear={clearFilters} />
        </header>
        
        {isLoading ? (
          <Spinner />
        ) : (
          <ProductList pagination={pagination} />
        )}
      </main>
    </div>
  );
}
```

---

## SQL запросы для фасетов

### 1. Фасет цены (range)

```sql
SELECT 
    MIN(price) as min_price,
    MAX(price) as max_price,
    COUNT(*) as total_count
FROM products
WHERE category_id IN (
    SELECT descendant_id 
    FROM category_hierarchy 
    WHERE ancestor_id = @categoryId
)
-- Применить другие фильтры
AND price BETWEEN @minPrice AND @maxPrice;
```

### 2. Фасет бренда (list с count)

```sql
SELECT 
    attributes->>'brand' as brand,
    COUNT(*) as product_count
FROM products
WHERE category_id IN (...)
GROUP BY attributes->>'brand'
ORDER BY product_count DESC;
```

### 3. Фасет с JSONB агрегацией

```sql
-- Для динамического атрибута "hood" (булево)
SELECT 
    (attributes->>'hood')::boolean as has_hood,
    COUNT(*) as product_count
FROM products
WHERE ...
GROUP BY (attributes->>'hood')::boolean;

-- Для строкового атрибута "size"
SELECT 
    attributes->>'size' as size,
    COUNT(*) as product_count
FROM products
WHERE ...
GROUP BY attributes->>'size'
HAVING COUNT(*) > 0
ORDER BY product_count DESC;
```

### 4. Фасет с диапазоном и гистограммой

```sql
-- Гистограмма цен по диапазонам
SELECT 
    CASE 
        WHEN price < 5000 THEN '0-5000'
        WHEN price < 10000 THEN '5000-10000'
        WHEN price < 20000 THEN '10000-20000'
        ELSE '20000+'
    END as price_range,
    COUNT(*) as product_count
FROM products
WHERE ...
GROUP BY price_range
ORDER BY MIN(price);
```

---

## Оптимизация производительности

### 1. Кэширование фасетов

```csharp
// Redis кэширование
public async Task<FacetsResponse> GetFacetsAsync(FacetQuery query)
{
    var cacheKey = BuildCacheKey(query);
    
    var cached = await _cache.GetAsync<FacetsResponse>(cacheKey);
    if (cached != null) return cached;
    
    var facets = await ComputeFacetsAsync(query);
    
    // Кэш на 5 минут
    await _cache.SetAsync(cacheKey, facets, TimeSpan.FromMinutes(5));
    
    return facets;
}
```

### 2. Параллельное вычисление фасетов

```csharp
var facets = await Task.WhenAll(
    BuildPriceFacetAsync(baseQuery, query),
    BuildBrandFacetAsync(baseQuery, query),
    BuildSizeFacetAsync(baseQuery, query),
    BuildColorFacetAsync(baseQuery, query)
);
```

### 3. Materialized View для частых фасетов

```sql
CREATE MATERIALIZED VIEW category_facets AS
SELECT 
    category_id,
    attributes->>'brand' as brand,
    COUNT(*) as product_count
FROM products
JOIN product_category ON products.id = product_category.product_id
GROUP BY category_id, attributes->>'brand';

-- Индекс для ускорения
CREATE INDEX idx_category_facets ON category_facets(category_id, brand);

-- Обновление по расписанию
REFRESH MATERIALIZED VIEW CONCURRENTLY category_facets;
```

---

## Итоговая архитектура

```
┌─────────────────────────────────────────────────────────────┐
│                     Frontend (React)                        │
│  ┌─────────────┐  ┌──────────────┐  ┌─────────────────┐   │
│  │ useFacets() │  │ FacetRenderer│  │ActiveFiltersChips│   │
│  └──────┬──────┘  └──────┬───────┘  └────────┬────────┘   │
│         │                │                    │             │
│         └────────────────┴────────────────────┘             │
│                           │                                 │
│                    REST API /api/facets                     │
└───────────────────────────┼─────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────┐
│                   Backend (.NET)                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ FacetService                                          │  │
│  │  ├─ BuildBaseQuery()  ← фильтрация + категория       │  │
│  │  ├─ BuildPriceFacet() ← MIN/MAX агрегация            │  │
│  │  ├─ BuildListFacet()  ← GROUP BY + COUNT             │  │
│  │  └─ BuildSubqueryReferences() ← URL генерация        │  │
│  └──────────────────────────────────────────────────────┘  │
│                           │                                 │
│                    EF Core + PostgreSQL                     │
└───────────────────────────┼─────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────┐
│                  PostgreSQL                                 │
│  ┌──────────────┐  ┌──────────────┐  ┌─────────────────┐  │
│  │  Products    │  │  Categories  │  │ CategoryHierarchy│  │
│  │  (JSONB+GIN) │  │  (Adjacency) │  │  (Closure Table)│  │
│  └──────────────┘  └──────────────┘  └─────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

---

## Чеклист реализации

- [ ] Создать модели `FacetsResponse`, `Facet`, `FacetValue`
- [ ] Реализовать `FacetService` с методами агрегации
- [ ] Создать эндпоинт `GET /api/facets`
- [ ] Реализовать `useFacets` хук на фронтенде
- [ ] Создать универсальный `FacetRenderer` компонент
- [ ] Добавить `ActiveFiltersChips` для отображения выбранных фильтров
- [ ] Реализовать SQL запросы с GROUP BY для фасетов
- [ ] Добавить кэширование (Redis/MemoryCache)
- [ ] Оптимизировать с помощью GIN индексов на JSONB
- [ ] Протестировать производительность на 1M+ товаров
