# Marketplace Frontend: каталог, карточка товара и покупка варианта

Документ описывает frontend-срез маркетплейса FitHub с учётом option-based фасетов из `sandbox/dynamicAttributes/test.md` и расширенной backend-модели магазина: товары, варианты, цены со скидками, картинки через существующую файловую подсистему и конкурентное резервирование остатков.

## Цель

`GymAdmin` видит каталог как обычный магазин: название, бренд, цену или диапазон цен, скидку, главную картинку, бейджи, наличие и фасеты. На деталке пользователь выбирает purchase attributes, например размер обуви или одежды, после чего покупка идёт по конкретному `productVariantId`.

`CmsAdmin` управляет категориями, брендами, товарами, вариантами, картинками, атрибутами, ценами и остатками.

Правила UI:

- один блок фильтра = один `attribute_definition`;
- значения в блоке = `attribute_options`;
- внутри блока можно выбрать несколько опций, это `OR`;
- между разными блоками фильтров применяется `AND`;
- `count = 0` показываем disabled, а не скрываем;
- покупка доступна только после выбора конкретного варианта;
- выбранный вариант должен иметь `availableQuantity > 0`;
- ошибки конкурентности и sold out от backend показываем как нормальный сценарий, а не как техническую аварию.

## Структура frontend-модулей

```text
pages/marketplace/
  catalog/
    MarketplaceCatalogPage.tsx
    ProductGrid.tsx
    ProductCard.tsx
    ProductPrice.tsx
    ProductBadges.tsx
    CatalogToolbar.tsx
    ActiveFilterChips.tsx
    facets/
      FacetPanel.tsx
      OptionFacet.tsx
  product/
    ProductDetailsPage.tsx
    ProductImageGallery.tsx
    VariantSelector.tsx
    AvailabilityLabel.tsx
    AddToCartPanel.tsx
  admin/
    ProductEditorPage.tsx
    ProductVariantEditor.tsx
    ProductImagesEditor.tsx
    ProductInventoryEditor.tsx
    CategoryEditorPage.tsx
    AttributeDefinitionEditor.tsx
api/services/marketplaceService.ts
types/marketplace.ts
```

`FacetPanel` остаётся option-only. Варианты товара не нужно смешивать с фасетами: фасеты сужают каталог, а `VariantSelector` на деталке выбирает конкретную SKU для покупки.

## TypeScript-контракты

Все entity id в API приходят строками с GUID. Это соответствует backend-паттерну FitHub: доменные id наследуются от `GuidIdentifier<TId>`, а EF Core хранит их как PostgreSQL `uuid`.

```ts
export interface MarketplaceSearchRequest {
  productCategoryId: string;
  minPrice?: number;
  maxPrice?: number;
  filters: SelectedAttributeFilter[];
  sort?: 'priceAsc' | 'priceDesc' | 'newest' | 'ratingDesc';
  page: number;
  pageSize: number;
}

export interface SelectedAttributeFilter {
  attributeDefinitionId: string;
  attributeOptionIds: string[];
}

export interface MarketplaceSearchResponse {
  items: MarketplaceProductListItem[];
  productCount: number;
  facets: MarketplaceFacet[];
}

export interface MarketplaceProductListItem {
  id: string;
  name: string;
  brandName?: string;
  slug: string;
  shortDescription?: string;
  price: ProductPriceRange;
  availability: ProductAvailabilitySummary;
  primaryImage?: ProductImage;
  badges: string[];
}

export interface ProductPriceRange {
  from: number;
  to?: number;
  compareAtPrice?: number;
  currency: string;
  discountPercent?: number;
}

export interface ProductAvailabilitySummary {
  status: 'inStock' | 'lowStock' | 'outOfStock';
  totalAvailableQuantity: number;
}

export interface ProductImage {
  fileId: string;
  url: string;
  altText?: string;
  isPrimary?: boolean;
  sortOrder?: number;
}

export interface MarketplaceFacet {
  attributeDefinitionId: string;
  code: string;
  name: string;
  isPurchaseOption: boolean;
  values: MarketplaceFacetValue[];
}

export interface MarketplaceFacetValue {
  attributeOptionId: string;
  optionText: string;
  count: number;
  selectedByUser: boolean;
  isAvailable: boolean;
}
```

Деталка товара:

```ts
export interface MarketplaceProductDetails {
  id: string;
  name: string;
  brandName?: string;
  description?: string;
  images: ProductImage[];
  purchaseAttributes: PurchaseAttribute[];
  variants: ProductVariant[];
}

export interface PurchaseAttribute {
  attributeDefinitionId: string;
  code: string;
  name: string;
  values: AttributeOption[];
}

export interface AttributeOption {
  attributeOptionId: string;
  optionText: string;
}

export interface ProductVariant {
  id: string;
  sku: string;
  price: number;
  compareAtPrice?: number;
  currency: string;
  availableQuantity: number;
  availabilityStatus: 'inStock' | 'lowStock' | 'outOfStock';
  attributes: ProductVariantAttribute[];
}

export interface ProductVariantAttribute {
  attributeDefinitionId: string;
  attributeOptionId: string;
}
```

Admin DTO для редактирования товара, варианта и остатков должен включать `version`, если backend использует явную app-managed concurrency колонку. Frontend отправляет тот `version`, который был получен при загрузке формы; после успешного сохранения backend возвращает новый `version`.

Резервирование:

```ts
export interface CreateStockReservationRequest {
  productVariantId: string;
  quantity: number;
  idempotencyKey: string;
}

export interface StockReservationResponse {
  reservationId: string;
  expiresAt: string;
  availableQuantity: number;
}
```

## Каталог

`ProductCard` показывает:

- главную картинку, полученную через `ProductImage.url`;
- бренд и название;
- короткое описание, если оно есть;
- цену: `from`, `to`, `compareAtPrice`, `discountPercent`;
- бейджи: `Sale`, `Low stock`, `Out of stock`, `New`;
- availability label.

Правила цены:

- если `from === to` или `to` отсутствует, показывать одну цену;
- если `from !== to`, показывать "from $129" или "$129-$149";
- `compareAtPrice` показывать зачёркнутым только если он больше текущей цены;
- `discountPercent` показывать только если backend его вернул или frontend может безопасно вычислить из `compareAtPrice`.

## Фасеты

`OptionFacet` остаётся единственным компонентом фильтра:

```tsx
export function OptionFacet({ facet, selectedOptionIds, onChange }: Props) {
  return (
    <section>
      <h3>{facet.name}</h3>
      {facet.values.map((value) => (
        <label key={value.attributeOptionId}>
          <input
            type="checkbox"
            checked={selectedOptionIds.includes(value.attributeOptionId)}
            disabled={!value.isAvailable && !value.selectedByUser}
            onChange={() => onChange(facet.attributeDefinitionId, value.attributeOptionId)}
          />
          <span>{value.optionText}</span>
          <span>{value.count}</span>
        </label>
      ))}
    </section>
  );
}
```

`isPurchaseOption` можно визуально пометить, но поведение фасета не меняется: в каталоге пользователь может выбрать несколько размеров или цветов как `OR`.

## Деталка и выбор варианта

`ProductDetailsPage` загружает `MarketplaceProductDetails` и строит:

- галерею из `images`, отсортированную по `sortOrder`;
- блок цены выбранного варианта;
- `VariantSelector` по `purchaseAttributes`;
- availability label выбранного варианта;
- кнопку reserve/add to cart.

Алгоритм выбора:

1. Состояние хранит `selectedOptions: Record<attributeDefinitionId, attributeOptionId>`, где оба id - строки с GUID.
2. После каждого клика найти `ProductVariant`, у которого все `attributes` совпадают с выбранными опциями.
3. Опции, которые не приводят ни к одному active/in-stock варианту, показывать disabled.
4. Если выбран полный набор purchase attributes и вариант найден, включить кнопку покупки.
5. Если вариант `outOfStock`, кнопку выключить и показать "Out of stock".

Важно: в форме товара для `CmsAdmin` purchase attribute обычно single-select, потому что один variant имеет одну опцию на один attribute definition. В каталожных фасетах тот же attribute может быть checkbox, потому что это фильтр по нескольким вариантам/товарам.

## Резерв и конкурентность

При клике "Add to cart" frontend вызывает:

`POST /api/v1/marketplace/checkout/reservations`

`idempotencyKey` генерировать стабильно для попытки добавления в корзину, например `cartId + productVariantId`. На повтор клика или retry backend вернёт существующий reserve вместо повторного списания.

UI-обработка:

- `409 Conflict` или доменная ошибка "out of stock": обновить деталку товара и показать, что вариант закончился;
- `ConcurrencyException` из админского сохранения: показать сообщение "Данные изменились, обновите страницу" и предложить reload;
- истёкший reserve: убрать строку из корзины или попросить зарезервировать заново;
- после успешного reserve показывать TTL до `expiresAt`.

Frontend не должен локально уменьшать остатки как источник истины. Можно оптимистично обновить UI, но после ответа backend нужно принять `availableQuantity` из response или заново загрузить товар.

## URL и состояние фильтров

Источник правды для выбранных фильтров - URL и search response.

```text
/marketplace/catalog?categoryId=019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01&price=0..10000&f019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b11=019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b21,019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b22&sort=priceAsc
```

Flow:

1. При загрузке страницы распарсить query string в `MarketplaceSearchRequest`.
2. Вызвать `POST /api/v1/marketplace/catalog/products/search`.
3. Сохранить `items`, `facets`, `productCount`.
4. При клике по option обновить URL и повторить search.
5. Не пересчитывать counts на клиенте.

## Admin UI

### Product

- category;
- brand;
- name;
- slug;
- shortDescription;
- description;
- status: draft/published/archived;
- badges или computed badges;
- payload для read/snapshot-полей, которые не участвуют в фасетах.

### Product images

Использовать существующий file flow:

1. запросить presigned URL;
2. загрузить файл;
3. подтвердить upload;
4. сделать файл active и привязать к `EntityType.Product`;
5. создать/обновить `product_image` с `fileId`, `sortOrder`, `altText`, `isPrimary`.

В UI нужен drag-and-drop порядок картинок и явная главная картинка. На backend есть уникальный индекс на одну primary image на товар, поэтому frontend должен не давать выбрать две главные картинки.

### Attribute definition

- `productCategoryId`;
- `code`: `size`, `color`, `material`, `season`;
- `name`;
- `isPurchaseOption`;
- `filterable`;
- `facetable`.

### Product variants

Для каждого варианта:

- sku;
- barcode;
- nameSuffix;
- status;
- currentPrice;
- compareAtPrice;
- currency;
- purchase attributes: size/color/etc.;
- inventory: available/reserved/sold/lowStockThreshold.

При сохранении остатка админка должна отправлять concurrency token, если backend отдаёт его в DTO. Если backend отвечает conflict, UI не должен перетирать изменения: нужно перезагрузить текущие значения и попросить повторить правку.

## Валидация

Frontend проверяет:

- `name`, `slug`, `sku` не пустые;
- `currentPrice >= 0`;
- `compareAtPrice` пустой или больше `currentPrice`;
- `availableQuantity`, `reservedQuantity`, `soldQuantity` не отрицательные;
- у товара есть не больше одной primary image;
- в search request не отправляется пустой `attributeOptionIds`;
- `minPrice <= maxPrice`, если оба поля заданы;
- для published товара есть хотя бы один active variant;
- для active variant заполнены все required purchase attributes категории.

Backend остаётся источником истины: он должен отклонять option от другой категории, неизвестный `attributeDefinitionId`, дубли SKU, некорректную цену, конфликт остатков и конкурентное редактирование.

## Проверки перед завершением фичи

- `GymAdmin` видит каталог с картинками, ценами, скидками и наличием.
- В одном фасете можно выбрать несколько опций, и они работают как `OR`.
- Между разными фасетами работает `AND`.
- Counts пересчитываются после каждого выбора.
- Значения с `count = 0` видны и disabled.
- URL переживает reload страницы.
- Деталка товара выбирает корректный variant по purchase attributes.
- Покупка недоступна без выбранного variant.
- Последняя единица товара не может быть успешно зарезервирована двумя пользователями одновременно.
- `CmsAdmin` может управлять картинками через существующий file flow.
- Конфликт конкурентного админского сохранения не перетирает чужие изменения.
