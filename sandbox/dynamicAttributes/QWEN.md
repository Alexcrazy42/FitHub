# FitHub Sandbox: Dynamic Attributes & Faceted Search

## Project Overview

This is a **sandbox e-commerce project** demonstrating dynamic attributes and faceted search functionality for products with varying characteristics across categories (clothing, electronics, footwear, etc.).

### Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Docker Compose                           │
├─────────────────────┬───────────────────────────────────────┤
│   Backend (.NET 9)  │   Frontend (React 19 + TypeScript)    │
│   - ASP.NET API     │   - Vite + HMR                        │
│   - EF Core         │   - Tailwind CSS                      │
│   - PostgreSQL      │   - Component-based UI                │
│   - JSONB + GIN     │                                       │
└─────────────────────┴───────────────────────────────────────┘
```

### Key Features

1. **Backend-Driven UI**: Backend sends filter schema → Frontend renders dynamically
2. **Hybrid Storage**: Static columns (price, name) + JSONB for dynamic attributes
3. **Faceted Search**: Real-time facet counts that update after each filter selection
4. **Category Tree**: Adjacency list + Closure Table pattern for hierarchical categories
5. **Type-Safe**: .NET discriminated unions + TypeScript via NSwag

### Tech Stack

| Layer | Technology |
|-------|------------|
| Backend | .NET 9, ASP.NET Core, EF Core |
| Database | PostgreSQL 16 (JSONB + GIN indexes) |
| Frontend | React 19, TypeScript, Vite |
| Styling | Tailwind CSS |
| Container | Docker Compose |

---

## Directory Structure

```
dynamicAttributes/
├── backend/
│   ├── Backend/
│   │   ├── Program.cs          # Minimal API entry point
│   │   ├── appsettings.json    # App configuration
│   │   └── Backend.csproj      # .NET 9 project file
│   ├── Backend.sln             # Solution file
├── frontend/
│   ├── src/
│   │   ├── App.tsx             # Main React component
│   │   ├── main.tsx            # React entry point
│   │   ├── index.css           # Global styles
│   │   └── assets/             # Static assets
│   ├── package.json            # Dependencies & scripts
│   ├── vite.config.ts          # Vite configuration
│   └── tsconfig.json           # TypeScript config
├── images/                     # Documentation images
├── docker-compose.yml          # PostgreSQL service
├── README.md                   # Project requirements & architecture
└── roadmap.md                  # Development sprints & milestones
```

---

## Building and Running

### Prerequisites

- .NET 9 SDK (see `backend/global.json`)
- Node.js 18+ (for frontend)
- Docker & Docker Compose

### Start Database (PostgreSQL)

```bash
docker-compose up -d postgres
```

PostgreSQL will be available at `localhost:5432` with:
- Database: `ecommerce`
- User: `postgres`
- Password: `password`

### Run Backend

```bash
cd backend/Backend
dotnet run
```

API will be available at `http://localhost:5000` (or as configured)

### Run Frontend (Development)

```bash
cd frontend
npm install
npm run dev
```

Frontend will be available at `http://localhost:5173` (Vite default)

### Build Frontend (Production)

```bash
cd frontend
npm run build
```

---

## Development Status

### Current State (Initial Setup)

- ✅ Basic project structure (backend + frontend)
- ✅ Docker Compose with PostgreSQL
- ✅ Minimal .NET 9 API template
- ✅ React 19 + TypeScript + Vite template

### Planned Implementation (from roadmap.md)

#### Sprint 1: Base Structure
- [ ] Models: `Product`, `Category`, `ProductCategory`
- [ ] DbContext + Migrations
- [ ] Seed data (5 categories, 100 products)
- [ ] API: `/products`, `/categories`

#### Sprint 2: Dynamic Filters + Facets
- [ ] FilterSchema API (`/filters/schema?categoryId=1`)
- [ ] Typed Filters (RangeFilter, ToggleFilter, CheckboxFilter)
- [ ] Facets API (`/facets?filters=...`)
- [ ] Search API with filters + facets
- [ ] Frontend: FilterRenderer, DynamicFacets, ActiveFilters (Chips)

#### Sprint 3: Admin Dashboard
- [ ] Admin API for CRUD operations
- [ ] Category tree editor
- [ ] Filter editor (type + parameters)
- [ ] Product editor with JSONB attributes

---

## Database Schema (Planned)

```sql
-- Categories (Adjacency List + Closure Table)
CREATE TABLE Categories (
    Id UUID PRIMARY KEY,
    Name NVARCHAR(255),
    ParentId UUID REFERENCES Categories(Id)
);

-- Products (static + dynamic attributes)
CREATE TABLE Products (
    Id UUID PRIMARY KEY,
    Name NVARCHAR(255),
    Price DECIMAL(10,2),
    Attributes JSONB,  -- Dynamic attributes
    INDEX idx_price (Price),
    INDEX idx_attributes ON Products USING GIN(Attributes)
);

-- M:N Product-Category mapping
CREATE TABLE ProductCategory (
    ProductId UUID REFERENCES Products(Id),
    CategoryId UUID REFERENCES Categories(Id),
    PRIMARY KEY (ProductId, CategoryId)
);

-- Closure Table for category hierarchy
CREATE TABLE CategoryHierarchy (
    AncestorId UUID REFERENCES Categories(Id),
    DescendantId UUID REFERENCES Categories(Id),
    Depth INT,
    PRIMARY KEY (AncestorId, DescendantId)
);
```

---

## API Design (Planned)

### Filter Schema Endpoint

**GET** `/api/filters/schema?categoryId={id}`

Response:
```json
{
  "staticFilters": [
    {"id": "price", "type": "range", "min": 0, "max": 100000, "unit": "₽"},
    {"id": "stores", "type": "checkbox", "multiple": true}
  ],
  "dynamicFilters": [
    {"id": "hood", "type": "toggle", "label": "С капюшоном"},
    {"id": "shoeSize", "type": "range", "min": 35, "max": 47, "step": 0.5},
    {"id": "category", "type": "checkbox", "options": [...]}
  ]
}
```

### Search Endpoint

**POST** `/api/products/search`

Request:
```json
{
  "categoryId": 1,
  "filters": {
    "price": {"type": "range", "value": [5000, 30000]},
    "hood": {"type": "toggle", "value": true}
  }
}
```

### Facets Endpoint

**GET** `/api/facets?filters=...`

Returns available facet values with counts for current selection.

---

## Filter Types

| Type | UI Component | Example |
|------|--------------|---------|
| `range` | Slider | Price: 5k ─── 30k |
| `toggle` | Switch | Eco-product ON |
| `checkbox` | Checkboxes | Nike ☐ Adidas ☑ |
| `radio` | Radio buttons | M / F |
| `dropdown` | Select | Sort by: Price ↑ |
| `searchInput` | Text input | Product name |

---

## Key Concepts

### Why JSONB for Dynamic Attributes?

Different product categories have different attributes:
- **Footwear**: sole length (25-30cm), fastener type (laces/velcro)
- **Clothing**: hood (true/false), sleeve length (short/long)
- **Electronics**: screen diagonal (32-85"), resolution (FullHD/4K)

JSONB with GIN index provides:
- ✅ Fast queries: `WHERE Attributes @> '{"hood": true}'`
- ✅ No schema migrations for new attributes
- ✅ Flexible per-category attributes

### Why NOT EAV (Entity-Attribute-Value)?

- ❌ Slow JOINs
- ❌ No type safety
- ❌ Complex indexing
- ❌ O(n) scaling issues

### Faceted Search Algorithm

```csharp
// 1. Base query with active filters
var baseQuery = _db.Products.Where(/* current filters */);

// 2. For each facet — aggregate count
var hoodFacet = await baseQuery
    .GroupBy(p => p.Attributes["hood"]?.GetBoolean())
    .Select(g => new { Value = g.Key, Count = g.Count() })
    .ToListAsync();
```

---

## Development Conventions

### Backend (.NET)

- Nullable reference types enabled
- Implicit usings enabled
- Minimal API pattern (no controllers initially)
- EF Core for data access

### Frontend (React/TypeScript)

- TypeScript strict mode
- Functional components with hooks
- ESLint with React hooks rules
- Vite for bundling and HMR

### Code Style

- C#: Standard .NET conventions (PascalCase for public members)
- TypeScript: camelCase for variables/functions, PascalCase for types
- Consistent formatting via editor configs

---

## Performance Targets

| Operation | Target |
|-----------|--------|
| Static filters (B-tree) | ~2ms |
| JSONB GIN index | ~8ms |
| Facets aggregation | ~15ms |
| **Total** | **~25ms** |

---

## References

- [README.md](./README.md) — Detailed requirements and architecture
- [roadmap.md](./roadmap.md) — Development sprints and milestones
