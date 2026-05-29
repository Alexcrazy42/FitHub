# Marketplace Backend: товар, варианты, цены, остатки и фасеты

Документ фиксирует backend-срез каталога маркетплейса FitHub по схеме из `sandbox/dynamicAttributes/test.md`, но расширяет её до модели, похожей на реальный магазин. Фасеты остаются option-based, а покупка происходит по конкретному варианту товара: например "кроссовки Nike Metcon 9, размер 42, цвет Black".

## Цель

`GymAdmin` выбирает категорию, фасеты и товар, затем покупает конкретный вариант. Backend возвращает товары с названием, ценой, скидкой, картинками, доступностью и вариантами. `CmsAdmin` управляет товарами, вариантами, опциями атрибутов, ценами, картинками и остатками.

Правила:

- значения внутри одного `attribute_definition` объединяются через `OR`;
- разные `attribute_definition` объединяются через `AND`;
- фасеты считаются по текущей выборке товаров;
- покупка всегда идёт по `product_variant_id`, а не по `product_id`;
- остатки меняются атомарно, потому что несколько пользователей могут одновременно покупать последний размер;
- картинки используют существующую файловую подсистему `FileEntity` + presigned URL flow.

## Идентификаторы

В FitHub id сущностей делаются через типизированные GUID-идентификаторы: `ProductId : GuidIdentifier<ProductId>, IIdentifierDescription`. `GuidIdentifier<TId>.New()` создаёт `Guid.CreateVersion7()`, а общие EF Core conventions конвертируют `GuidIdentifier<T>` в PostgreSQL `uuid` и ставят `ValueGeneratedNever()`.

Для marketplace нужно следовать тому же паттерну:

- не использовать `bigserial`/`long` id для доменных сущностей;
- для каждой сущности завести `*Id : GuidIdentifier<*Id>`;
- id создавать в домене через `*Id.New()`;
- в DDL и SQL-примерах использовать `uuid`;
- в API отдавать id строками, как это уже делается для `FileId`.

## Существующая файловая схема

В проекте уже есть `Domain/Files/FileEntity`: `FileName`, `S3Key`, `Status`, `EntityId`, `EntityType`, `CreatedAt`, `MultipartUploadId`. Загрузка идёт через `FileController` и `FileService`: получить presigned URL, подтвердить upload, затем сделать файлы active и привязать к сущности через `MakeFilesActiveAsync`.

Для marketplace не нужно дублировать `S3Key` в таблицах товара. Нужна отдельная таблица связи:

```sql
product_image
  id uuid primary key
  productId uuid not null references product(id) on delete cascade
  fileId uuid not null references file_entity(id)
  sortOrder int not null default 0
  altText text null
  isPrimary boolean not null default false
```

При реализации нужно добавить `Product` в `FitHub.Shared.Common.EntityType`, чтобы `FileEntity.EntityType = Product`, а `EntityId = product.Id`. `product_image` хранит порядок, главную картинку и alt-текст; сам файл продолжает жить в общей подсистеме файлов.

## Схема БД

```sql
product_category
  id uuid primary key
  name text not null
  slug text not null unique

brand
  id uuid primary key
  name text not null
  slug text not null unique

product
  id uuid primary key
  productCategoryId uuid not null references product_category(id)
  brandId uuid null references brand(id)
  name text not null
  slug text not null unique
  shortDescription text null
  description text null
  status text not null -- draft | published | archived
  rating numeric(3,2) null
  reviewCount int not null default 0
  payload jsonb not null default '{}'::jsonb
  createdAt timestamptz not null
  updatedAt timestamptz not null
  version bigint not null default 0

product_variant
  id uuid primary key
  productId uuid not null references product(id) on delete cascade
  sku text not null unique
  barcode text null
  nameSuffix text null -- "Size 42 / Black"
  status text not null -- active | hidden | discontinued
  currentPrice numeric(12,2) not null
  compareAtPrice numeric(12,2) null -- old/list price for discount display
  currency char(3) not null default 'USD'
  weightGrams int null
  createdAt timestamptz not null
  updatedAt timestamptz not null
  version bigint not null default 0

product_variant_price_history
  id uuid primary key
  productVariantId uuid not null references product_variant(id) on delete cascade
  price numeric(12,2) not null
  compareAtPrice numeric(12,2) null
  currency char(3) not null
  startsAt timestamptz not null
  endsAt timestamptz null
  reason text null -- manual | sale | import | rollback

product_variant_inventory
  productVariantId uuid primary key references product_variant(id) on delete cascade
  availableQuantity int not null default 0
  reservedQuantity int not null default 0
  soldQuantity int not null default 0
  lowStockThreshold int not null default 3
  updatedAt timestamptz not null
  version bigint not null default 0

stock_reservation
  id uuid primary key
  productVariantId uuid not null references product_variant(id)
  quantity int not null
  status text not null -- active | committed | released | expired
  expiresAt timestamptz not null
  idempotencyKey text not null unique
  createdByUserId uuid null
  createdAt timestamptz not null
```

Фасеты и атрибуты:

```sql
attribute_definition
  id uuid primary key
  productCategoryId uuid not null references product_category(id)
  code text not null
  name text not null
  isPurchaseOption boolean not null default false -- size/color that defines variants
  filterable boolean not null default true
  facetable boolean not null default true
  unique (productCategoryId, code)

attribute_options
  id uuid primary key
  attributeDefinitionId uuid not null references attribute_definition(id) on delete cascade
  optionText text not null
  sortOrder int not null default 0
  unique (attributeDefinitionId, optionText)

product_attribute_index
  productId uuid not null references product(id) on delete cascade
  attributeDefinitionId uuid not null references attribute_definition(id) on delete cascade
  attributeOptionId uuid not null references attribute_options(id)
  primary key (productId, attributeDefinitionId, attributeOptionId)

product_variant_attribute
  productVariantId uuid not null references product_variant(id) on delete cascade
  attributeDefinitionId uuid not null references attribute_definition(id)
  attributeOptionId uuid not null references attribute_options(id)
  primary key (productVariantId, attributeDefinitionId)
```

`product_attribute_index` нужен для быстрого каталога и фасетов на уровне товара. `product_variant_attribute` нужен для выбора варианта перед покупкой: размер, цвет, материал комплектации. Если товар может иметь несколько значений одного фасета, например "для дома" и "для зала", это теперь поддерживается ключом `(productId, attributeDefinitionId, attributeOptionId)`.

## Индексы

```sql
create index ix_product_category_status on product(productCategoryId, status);
create index ix_product_category_rating on product(productCategoryId, rating desc);
create index ix_product_payload_gin on product using gin (payload);

create index ix_product_variant_product_status on product_variant(productId, status);
create index ix_product_variant_price on product_variant(currentPrice);

create index ix_product_image_product_sort on product_image(productId, sortOrder);
create unique index ux_product_image_primary on product_image(productId) where isPrimary = true;

create index ix_attr_def_category_filterable
  on attribute_definition(productCategoryId, filterable, facetable);

create index ix_attr_opt_def_sort
  on attribute_options(attributeDefinitionId, sortOrder);

create index ix_pai_attr_opt
  on product_attribute_index(attributeDefinitionId, attributeOptionId, productId);

create index ix_pai_product
  on product_attribute_index(productId);

create index ix_variant_attr_option
  on product_variant_attribute(attributeDefinitionId, attributeOptionId, productVariantId);

create index ix_stock_reservation_variant_status
  on stock_reservation(productVariantId, status, expiresAt);
```

## API

### Поиск товаров

`POST /api/v1/marketplace/catalog/products/search`

Request:

```json
{
  "productCategoryId": "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01",
  "minPrice": 0,
  "maxPrice": 10000,
  "filters": [
    {
      "attributeDefinitionId": "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b11",
      "attributeOptionIds": [
        "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b21",
        "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b22"
      ]
    },
    {
      "attributeDefinitionId": "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b12",
      "attributeOptionIds": ["019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b23"]
    }
  ],
  "sort": "priceAsc",
  "page": 1,
  "pageSize": 36
}
```

Response:

```json
{
  "items": [
    {
      "id": "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b31",
      "name": "Nike Metcon 9",
      "brandName": "Nike",
      "slug": "nike-metcon-9",
      "shortDescription": "Stable training shoes for heavy lifts and HIIT.",
      "price": {
        "from": 129.00,
        "to": 149.00,
        "compareAtPrice": 179.00,
        "currency": "USD",
        "discountPercent": 17
      },
      "availability": {
        "status": "inStock",
        "totalAvailableQuantity": 14
      },
      "primaryImage": {
        "fileId": "7d67a3e7-4c61-4f74-a9a9-4d8ed9bb0fd7",
        "url": "/api/v1/files/7d67a3e7-4c61-4f74-a9a9-4d8ed9bb0fd7",
        "altText": "Nike Metcon 9 black training shoe"
      },
      "badges": ["Sale", "Low stock"]
    }
  ],
  "productCount": 11,
  "facets": [
    {
      "attributeDefinitionId": "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b11",
      "code": "size",
      "name": "Size",
      "isPurchaseOption": true,
      "values": [
        {
          "attributeOptionId": "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b21",
          "optionText": "42",
          "count": 8,
          "selectedByUser": true,
          "isAvailable": true
        }
      ]
    }
  ]
}
```

### Деталка товара

`GET /api/v1/marketplace/catalog/products/{id}`

Возвращает полное описание, картинки, атрибуты, варианты, цены и остатки:

```json
{
  "id": "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b31",
  "name": "Nike Metcon 9",
  "description": "Stable training shoes for heavy lifts and HIIT.",
  "images": [
    {
      "fileId": "7d67a3e7-4c61-4f74-a9a9-4d8ed9bb0fd7",
      "url": "/api/v1/files/7d67a3e7-4c61-4f74-a9a9-4d8ed9bb0fd7",
      "altText": "Nike Metcon 9 black training shoe",
      "isPrimary": true,
      "sortOrder": 0
    }
  ],
  "purchaseAttributes": [
    {
      "attributeDefinitionId": "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b11",
      "code": "size",
      "name": "Size",
      "values": [
        { "attributeOptionId": "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b21", "optionText": "42" },
        { "attributeOptionId": "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b22", "optionText": "43" }
      ]
    }
  ],
  "variants": [
    {
      "id": "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b41",
      "sku": "NIKE-METCON9-BLK-42",
      "price": 149.00,
      "compareAtPrice": 179.00,
      "currency": "USD",
      "availableQuantity": 2,
      "availabilityStatus": "lowStock",
      "attributes": [
        {
          "attributeDefinitionId": "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b11",
          "attributeOptionId": "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b21"
        }
      ]
    }
  ]
}
```

### Резервирование остатка

`POST /api/v1/marketplace/checkout/reservations`

```json
{
  "productVariantId": "019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b41",
  "quantity": 1,
  "idempotencyKey": "cart-8b097c39-variant-019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b41"
}
```

Response:

```json
{
  "reservationId": "7c5219ce-279b-4388-89f7-63a607be9b71",
  "expiresAt": "2026-04-12T11:20:00Z",
  "availableQuantity": 1
}
```

## Конкурентность и EF Core

Остатки нельзя обновлять через "прочитал availableQuantity, изменил объект, SaveChanges". При параллельных покупках это даст oversell. Для резерва нужен один атомарный SQL update в транзакции:

```sql
update product_variant_inventory
set availableQuantity = availableQuantity - @quantity,
    reservedQuantity = reservedQuantity + @quantity,
    updatedAt = now(),
    version = version + 1
where productVariantId = @productVariantId
  and availableQuantity >= @quantity
returning availableQuantity, reservedQuantity, version;
```

Если `returning` не вернул строку, товара уже нет в нужном количестве. После успешного update в той же транзакции создаётся `stock_reservation` с уникальным `idempotencyKey`. Повторный запрос с тем же ключом должен вернуть существующую активную резервацию, а не списывать остаток второй раз.

Коммит заказа:

```sql
update product_variant_inventory
set reservedQuantity = reservedQuantity - @quantity,
    soldQuantity = soldQuantity + @quantity,
    updatedAt = now(),
    version = version + 1
where productVariantId = @productVariantId
  and reservedQuantity >= @quantity;
```

Освобождение или истечение резерва:

```sql
update product_variant_inventory
set availableQuantity = availableQuantity + @quantity,
    reservedQuantity = reservedQuantity - @quantity,
    updatedAt = now(),
    version = version + 1
where productVariantId = @productVariantId
  and reservedQuantity >= @quantity;
```

Для EF Core:

- операции резерва, commit и release делать в явной транзакции;
- использовать EF Core raw SQL query (`SqlQuery`/`FromSql` для projection) или ADO.NET command для conditional update с `returning`, потому что это критичная секция и backend должен получить обновлённые остатки;
- `stock_reservation.idempotencyKey` держать уникальным индексом;
- для редактирования `product`, `product_variant`, `product_variant_inventory` в админке использовать `version` как optimistic concurrency token;
- `PostgresUnitOfWork` уже переводит `DbUpdateConcurrencyException` в пользовательскую `ConcurrencyException`, поэтому UI должен показывать "обновите страницу" для конфликтов админского редактирования;
- не держать долгие транзакции вокруг оплаты; резерв создаётся быстро, оплата идёт с TTL резерва, commit/release выполняется отдельной короткой транзакцией.

EF Core configuration для app-managed `version`:

```csharp
builder.Property(x => x.Version)
    .IsConcurrencyToken();
```

При админском редактировании сервис должен увеличивать `Version` на 1 перед `SaveChangesAsync`. Для SQL-операций резерва `version = version + 1` уже находится в atomic update. Если команда решит использовать PostgreSQL `xmin`, это можно сделать вместо явной колонки `version`, но тогда конфигурация будет Npgsql-specific и её нужно закрепить отдельным EF Core configuration-тестом.

Инварианты:

- `availableQuantity >= 0`;
- `reservedQuantity >= 0`;
- `soldQuantity >= 0`;
- `quantity > 0` у резерва;
- истёкшая резервация может быть released только один раз;
- committed резервация не может быть released обратно без отдельного refund flow.

## Алгоритм поиска

1. Отфильтровать `product` по `productCategoryId`, `status = published`.
2. Применить выбранные option-фильтры через `product_attribute_index`.
3. Для каждого товара требовать совпадение по каждому выбранному `attributeDefinitionId`.
4. Применить price range через `product_variant.currentPrice`: товар подходит, если есть active variant в диапазоне.
5. Вернуть страницу товаров с агрегатами цены, главной картинкой и total availability.
6. Посчитать фасеты по текущей выборке.

Если `filters` пустой, `matched_products` нужно считать из базовой выборки `product` по категории, статусу и цене, без join к `selected_groups`.

Пример фильтрации:

```sql
with selected_filters as (
    select '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b11'::uuid as attributeDefinitionId,
           '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b21'::uuid as attributeOptionId
    union all
    select '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b11'::uuid,
           '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b22'::uuid
    union all
    select '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b12'::uuid,
           '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b23'::uuid
),
selected_groups as (
    select
        attributeDefinitionId,
        array_agg(attributeOptionId) as option_ids
    from selected_filters
    group by attributeDefinitionId
),
matched_products as (
    select pai.productId
    from product_attribute_index pai
    join selected_groups sg
      on sg.attributeDefinitionId = pai.attributeDefinitionId
     and pai.attributeOptionId = any(sg.option_ids)
    group by pai.productId
    having count(distinct pai.attributeDefinitionId) = (select count(*) from selected_groups)
)
select
    p.id,
    p.name,
    min(pv.currentPrice) as priceFrom,
    max(pv.currentPrice) as priceTo,
    sum(inv.availableQuantity) as totalAvailableQuantity
from product p
join matched_products mp on mp.productId = p.id
join product_variant pv on pv.productId = p.id and pv.status = 'active'
join product_variant_inventory inv on inv.productVariantId = pv.id
where p.productCategoryId = '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01'::uuid
  and p.status = 'published'
  and pv.currentPrice between 0 and 10000
group by p.id, p.name
order by priceFrom asc;
```

## Алгоритм фасетов

Для UI лучше возвращать все опции категории, включая нулевые counts. Для purchase options counts можно считать по active variants с остатком, если нужно скрывать недоступные размеры; для обычных фасетов достаточно product-level выборки.

```sql
with selected_filters as (
    select '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b11'::uuid as attributeDefinitionId,
           '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b21'::uuid as attributeOptionId
    union all
    select '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b11'::uuid,
           '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b22'::uuid
    union all
    select '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b12'::uuid,
           '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b23'::uuid
),
selected_groups as (
    select
        attributeDefinitionId,
        array_agg(attributeOptionId) as option_ids
    from selected_filters
    group by attributeDefinitionId
),
matched_products as (
    select pai.productId
    from product_attribute_index pai
    join selected_groups sg
      on sg.attributeDefinitionId = pai.attributeDefinitionId
     and pai.attributeOptionId = any(sg.option_ids)
    group by pai.productId
    having count(distinct pai.attributeDefinitionId) = (select count(*) from selected_groups)
),
all_facet_values as (
    select
        ad.id as attributeDefinitionId,
        ad.code,
        ad.name,
        ad.isPurchaseOption,
        ao.id as attributeOptionId,
        ao.optionText
    from attribute_definition ad
    join attribute_options ao on ao.attributeDefinitionId = ad.id
    where ad.productCategoryId = '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01'::uuid
      and ad.filterable = true
      and ad.facetable = true
)
select
    af.attributeDefinitionId,
    af.code,
    af.name,
    af.isPurchaseOption,
    af.attributeOptionId,
    af.optionText,
    count(distinct mp.productId) as cnt
from all_facet_values af
left join product_attribute_index pai
  on pai.attributeDefinitionId = af.attributeDefinitionId
 and pai.attributeOptionId = af.attributeOptionId
left join matched_products mp on mp.productId = pai.productId
group by af.attributeDefinitionId, af.code, af.name, af.isPurchaseOption, af.attributeOptionId, af.optionText
order by af.attributeDefinitionId, af.attributeOptionId;
```

`selectedByUser` backend выставляет сравнением пары `(attributeDefinitionId, attributeOptionId)` с request. `isAvailable = cnt > 0`.

## Интеграция с FitHub

В `Platform` лучше добавить marketplace-код в отдельные папки:

- `Domain/Marketplace`: `Product`, `ProductVariant`, `ProductImage`, `ProductVariantInventory`, `StockReservation`, `Brand`, `ProductCategory`, `AttributeDefinition`, `AttributeOption`;
- `Application/Marketplace`: поиск, CRUD каталога, фасеты, pricing, reservation service;
- `Data/Marketplace`: EF Core configurations и repositories;
- `Contracts/V1/Marketplace`: request/response DTO.

Авторизация:

- `CmsAdmin`: CRUD категорий, брендов, атрибутов, опций, товаров, вариантов, картинок и остатков;
- `GymAdmin`: поиск, просмотр published товаров, резервирование и покупка.

## Проверка производительности

Для запросов фасетов и каталога смотреть `EXPLAIN ANALYZE`. Подозрительные признаки:

- `Seq Scan` по большой `product_attribute_index`;
- `Seq Scan` по `product_variant_inventory` при расчёте availability;
- `Nested Loop` с большим outer-набором;
- большой разрыв между estimated `rows` и actual rows;
- тяжёлый `Sort` или `GroupAggregate` на миллионах строк.

## Тесты

Покрыть:

- `OR` внутри одного атрибута;
- `AND` между разными атрибутами;
- фасеты с `count = 0`;
- price range по вариантам;
- вывод скидки через `compareAtPrice`;
- главную картинку и порядок картинок;
- выбор purchase option на деталке товара;
- атомарный reserve последней единицы при параллельных запросах;
- идемпотентность reservation request;
- release expired reservation;
- optimistic concurrency conflict при админском редактировании товара или остатка;
- запрет использовать `attributeOptionId` от другой категории;
- интеграционный тест на PostgreSQL Testcontainers с seed-данными из `test.md`.
