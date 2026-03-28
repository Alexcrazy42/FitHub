# Роадмап: Sandbox E-commerce с динамическими фильтрами

## 🎯 Общая архитектура

```
Backend: .NET 10 + EF Core + PostgreSQL (JSONB + GIN)
Frontend: React 19 + TypeScript + Zod + Tailwind
Структура: 
├── Backend (ASP.NET API)
├── Frontend (Vite + React)
└── Docker Compose (Postgres + API + Frontend)
```

## 📋 Этапы разработки (2-3 дня)

### **Sprint 1: Базовая структура (День 1)**
```
✅ Backend:
  - Models: Product, Category, ProductCategory
  - DbContext + Migrations  
  - Seed data (5 категорий, 100 товаров)
  - API: /products, /categories

✅ Frontend:
  - Vite + React + TypeScript + Mantine
  - Экран товаров (ProductList)
  - Экран категорий (CategoryTree)

✅ Docker: Postgres + API
```

### **Sprint 2: Динамические фильтры + Фасеты (День 2)**
```
✅ Backend:
  - FilterSchema API (/filters/schema?categoryId=1)
  - Typed Filters (RangeFilter, ToggleFilter, CheckboxFilter)
  - Facets API (/facets?filters=...)
  - Search API с фильтрами + фасетами

✅ Frontend:
  - FilterRenderer (универсальный по схеме)
  - DynamicFacets компонент
  - Chips (активные фильтры)
  - Debounced search (300ms)

✅ Интеграция: Schema → UI → Typed Request → Search + Facets
```

### **Sprint 3: Админка (День 3)**
```
✅ Backend:
  - Admin API: /admin/categories, /admin/filters, /admin/products
  - FilterType enum (range, toggle, checkbox)

✅ Frontend:
  - Admin Dashboard
  - CategoryTreeEditor (добавление/перемещение)
  - FilterEditor (тип + параметры)
  - ProductEditor (+ JSONB атрибуты)
```

## 🏗️ Backend API Endpoints

```
GET    /api/categories/tree           # Дерево с count товаров
POST   /api/products/search           # Поиск + фасеты
GET    /api/filters/schema            # Schema фильтров по категории
POST   /api/admin/categories          # CRUD категории
POST   /api/admin/filters             # Добавить фильтр
POST   /api/admin/products            # Добавить товар
```

## 📦 Docker Compose

```yaml
version: '3.8'
services:
  postgres:
    image: postgres:16
    environment:
      POSTGRES_DB: ecommerce
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password
    ports:
      - "5432:5432"
  
  backend:
    build: ./backend
    ports:
      - "5000:80"
    depends_on:
      - postgres
    environment:
      ConnectionStrings__Default: "Host=postgres;Database=ecommerce;..."

  frontend:
    build: ./frontend
    ports:
      - "3000:3000"
    depends_on:
      - backend
```

## 💾 Backend Models (ключевые)

```csharp
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public JsonDocument? Attributes { get; set; }  // JSONB
    public List<ProductCategory> Categories { get; set; }
}

public class FilterSchema
{
    public string Id { get; set; }
    public string Type { get; set; }  // "range", "toggle", "checkbox"
    public string Label { get; set; }
    public object? Min { get; set; }
    public object? Max { get; set; }
    public List<FilterOption>? Options { get; set; }
}
```

## 🎨 Frontend Структура

```
src/
├── components/
│   ├── ProductList.tsx
│   ├── FilterRenderer.tsx      # Универсальный!
│   ├── DynamicFacets.tsx
│   ├── ActiveFilters.tsx       # Chips
│   └── CategoryTree.tsx
├── pages/
│   ├── Products.tsx
│   └── Admin/
│       ├── Categories.tsx
│       ├── Filters.tsx
│       └── Products.tsx
├── hooks/
│   └── useFilters.ts          # Zustand или Context
└── types/
    └── api.ts                 # NSwag generated
```

## 🚀 Минимальный MVP (запуск за 4 часа)

### Backend (Program.cs + 3 контроллера)
```
1. DbContext + Seed (категории + 100 товаров)
2. ProductsController (search + schema)
3. AdminController (CRUD)
4. NSwag для TS типов
```

### Frontend (3 компонента)
```
1. ProductList + mock data
2. FilterRenderer (switch по type)
3. CategoryTree (Mantine TreeView)
```

## 📊 Тестирование сценариев

```
✅ Пользователь:
  - Выбрать "Куртки" → фасеты "капюшон/рукава"
  - Слайдер цена → пересчет фасетов
  - Toggle "эко" → исчезли нерелевантные размеры

✅ Админ:
  - Добавить "Пуховики" в "Зимние куртки"
  - Создать фильтр "Теплость: range 100-500"
  - Добавить товар с атрибутами {"warmth": 300}
```

## 🎯 Результат Sandbox

```
👤 Пользователь видит:
- Живое дерево категорий
- Динамические фильтры (slider/toggle/checkbox)
- Фасеты с count (обновляются instantly)
- Chips активных фильтров

👨‍💼 Админ может:
- Добавить/переместить категории  
- Создать новый фильтр любого типа
- Добавить товары с JSONB атрибутами

⚡ Backend-Driven: типы фильтров из API
```

## 📱 UI Mockup (Mantine)

```
[Поиск] [Сортировка ▼]

📱 Куртки (1234)
  Фильтры ▼
  ════════
  Цена: [──●──────] 5-30к
  Капюшон: ● ON
  Длина рукава:
  ☑ Короткая (45)
  ☑ Длинная (123)
  
  [× Капюшон] [× Цена] [Очистить]

Товар 1 ₽5 200  ⭐⭐⭐⭐
Товар 2 ₽12 500 ⭐⭐⭐
```

**Стек готов к продакшену!** Docker one-click deploy 🚀