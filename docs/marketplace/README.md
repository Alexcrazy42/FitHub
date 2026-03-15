# FitHub Marketplace — Роадмап

Система маркетплейса внутри FitHub: каталог товаров, корзина, симулированная оплата через собственный BankManager-сервис, доставка с отслеживанием курьеров в реальном времени.

---

## Содержание

- [Контекст и ограничения](#контекст-и-ограничения)
- [Вопросы, которые нужно задать себе до старта](#вопросы-которые-нужно-задать-себе-до-старта)
- [Карта навыков](#карта-навыков)
- [Архитектура системы](#архитектура-системы)
- [Роадмап по фазам](#роадмап-по-фазам)
- [Тестирование без реальных курьеров](#тестирование-без-реальных-курьеров)

---

## Контекст и ограничения

**Существующие роли** (новых не добавлять):
- `CmsAdmin` — администратор сети залов → в маркетплейсе: **продавец** (единственный селлер — сеть как организация), ведёт каталог товаров, видит все заказы, управляет курьерами, подтверждает сборку
- `GymAdmin` — администратор конкретного зала → в маркетплейсе: **покупатель**, заказывает товары для своего зала, видит статус заказа и трекинг курьера в реальном времени (как в Яндекс Еде)

**Существующая инфраструктура**, которую можно переиспользовать:
- SignalR → уже есть, добавить хаб для трекинга курьеров
- S3/Minio → фотографии товаров
- RabbitMQ → оркестрация заказов (Saga)
- EF Core + PostgreSQL → каталог, заказы; PostGIS — геопозиция курьеров
- JWT + Claims → авторизация без изменений

**Оплата**: собственный сервис `BankManager` — отдельный .NET-микросервис или отдельный модуль, инкапсулирующий симулированный банк. Реальных денег нет, реальных провайдеров нет.

---

## Вопросы, которые нужно задать себе до старта

Ответить на каждый вопрос **письменно** перед тем как писать первую строчку кода. Это сэкономит переделки.

### Доменная модель

1. **Что продаётся?**
   - Только физические товары (спортинвентарь, добавки, одежда)

2. **Кто продаёт и кто покупает?**
   - Продавец — `CmsAdmin` (сеть), единый каталог и единый счёт продавца
   - Покупатель — `GymAdmin` (зал), у каждого зала свой счёт в BankManager
   - Есть ли комиссия между "счётом сети" и "счётом зала"? (интересно для симуляции расчётов даже без реального смысла)

3. **Откуда и куда доставка?**
   - Откуда: единый склад сети (`CmsAdmin` задаёт координаты склада)
   - Куда: адрес конкретного зала (`GymAdmin`) — у каждого зала уже есть адрес в системе, его и использовать
   - Как хранить адреса? Зал уже есть в БД — добавить поле `Location: geometry(Point, 4326)` к существующей сущности `Gym`

4. **Кто такие курьеры?**
   - Отдельная сущность `Courier` без привязки к `User` — курьер не является пользователем системы
   - `CmsAdmin` создаёт курьеров, назначает их на заказы, видит их позиции на карте
   - `GymAdmin` (покупатель) видит только курьера своего активного заказа — его позицию и ETA

5. **Жизненный цикл заказа — конечный автомат:**
   ```
   [Новый] → [Оплачивается] → [Оплачен] → [Собирается] → [Передан курьеру]
           ↓                                                      ↓
   [Отменён]                                            [В пути] → [Доставлен]
                                                              ↓
                                                         [Не доставлен]
   ```

### BankManager

6. **Что умеет BankManager?**
   - Создание счетов (покупатель, продавец, маркетплейс-комиссия)
   - Пополнение баланса (симулированное)
   - Холдирование суммы при оформлении заказа
   - Списание при подтверждении доставки
   - Возврат при отмене

7. **Как разворачивается BankManager?**
   - Отдельный процесс/контейнер с REST/gRPC API
   - Что делает основное приложение, если BankManager недоступен - circuit breaker, retry, fallback

8. **Как общаться с BankManager?**
   - Асинхронный RabbitMQ + некоторые вызов по gRPC

### Поиск

9. **Какой объём данных и требования к поиску?**
   - Просто ILIKE по PostgreSQL — достаточно для пет-проекта
   - Какие фильтры: категория, цена, рейтинг, зал-продавец, наличие
   - Нужна ли фасетная навигация (aggregations) - да

### Геолокация и курьеры

10. **Насколько реалистична симуляция?**
    - Курьер движется по прямой линии (интерполяция координат)?
    - Для пет-проекта рекомендую: **прямолинейная интерполяция + реальные координаты города**

12. **Какие данные хранить о позиции курьера?**
    - Только текущая позиция в Redis (низкая задержка)
    - История позиций в PostgreSQL (PostGIS) для аудита и аналитики

---

## Карта навыков

Что ты прокачаешь при реализации каждого модуля:

### Каталог и поиск
| Навык | Технология | Сложность |
|-------|------------|-----------|
| Фасетная навигация | Aggregations API | Средняя |
| Cursor-based pagination | Keyset pagination vs OFFSET | Средняя |
| Sync PostgreSQL → Search | Outbox Pattern | Высокая |
| Оптимизация изображений | ImageSharp + WebP pipeline | Низкая |

### Корзина и оформление заказа
| Навык | Технология | Сложность |
|-------|------------|-----------|
| Временное хранилище | Redis (TTL-based cart) | Низкая |
| Optimistic concurrency | EF Core RowVersion / ETags | Средняя |
| Idempotency (дублирование запросов) | Idempotency keys в заголовках | Средняя |
| Конечный автомат | Stateless (NuGet) | Средняя |

### BankManager (оплата)
| Навык | Технология | Сложность |
|-------|------------|-----------|
| Микросервис / модуль | gRPC или REST | Средняя |
| Distributed transaction | Saga Pattern (Choreography/Orchestration) | Высокая |
| Outbox Pattern | Надёжная публикация событий | Высокая |
| Circuit Breaker | Polly | Средняя |
| Двухфазный платёж | Hold → Capture / Release | Средняя |

### Доставка и курьеры
| Навык | Технология | Сложность |
|-------|------------|-----------|
| Геопространственные данные | PostGIS + NetTopologySuite | Средняя |
| Real-time позиция | SignalR (уже есть) | Низкая |
| GPS-симуляция | BackgroundService + интерполяция | Средняя |
| Routing (опционально) | OSRM (Docker) | Высокая |
| Геофенсинг (зона доставки) | PostGIS ST_Within | Средняя |

### Конкурентный доступ к данным
| Навык | Где применяется в маркетплейсе | Сложность |
|-------|-------------------------------|-----------|
| Optimistic concurrency (RowVersion) | Списание остатка товара (`Stock`) | Средняя |
| Pessimistic locking (`SELECT FOR UPDATE`) | Списание баланса в BankManager | Средняя |
| `FOR UPDATE SKIP LOCKED` | Атомарное назначение курьера на заказ | Высокая |
| Serializable isolation | Гарантия "последний товар не уйдёт дважды" | Высокая |
| Redis WATCH / Lua script | Атомарное уменьшение резерва в кэше | Средняя |
| Idempotent state transitions | Двойное нажатие "Оплатить" / "Начать сборку" | Средняя |
| Database CHECK constraints | Баланс никогда не уходит в минус на уровне БД | Низкая |

### Тестирование
| Навык | Технология | Сложность |
|-------|------------|-----------|
| Симуляция времени | ITimeProvider / FakeClock | Средняя |
| Симуляция GPS | Генератор маршрутов + BackgroundService | Средняя |
| Контрактные тесты | Pact.NET | Высокая |
| Property-based тесты | FsCheck / CsCheck | Высокая |
| Chaos testing | Polly + симуляция сбоев | Высокая |

### Бонусные навыки (можно добавить по желанию)
| Навык | Зачем |
|-------|-------|
| Redis Sorted Sets | Лидерборды (топ-товары, топ-продавцы) |
| Audit Log / Event Sourcing | История всех изменений заказа |
| Rate Limiting (ASP.NET Core 7+) | Защита API от перегрузки |
| API Versioning | Хорошая практика для публичного API |
| GraphQL (Hot Chocolate) | Гибкая выборка для каталога |
| Background Jobs | Hangfire: авто-отмена заказа, напоминания |
| Observability | OpenTelemetry трейсы через Saga |

---

## Архитектура системы

```
┌─────────────────────────────────────────────────────┐
│                   FitHub Frontend                    │
│  страница каталога │ корзина │ заказ │ трекинг       │
└───────────────────────────┬─────────────────────────┘
                            │ REST + SignalR
┌───────────────────────────▼─────────────────────────┐
│                  FitHub Backend (монолит)             │
│                                                      │
│  ┌──────────────┐  ┌──────────────┐  ┌───────────┐  │
│  │   Catalog    │  │    Orders    │  │  Delivery │  │
│  │  (поиск,     │  │  (корзина,   │  │ (курьеры, │  │
│  │   фильтры)   │  │   checkout)  │  │  трекинг) │  │
│  └──────┬───────┘  └──────┬───────┘  └─────┬─────┘  │
│         │                 │                │        │
│         └─────────────────┼────────────────┘        │
│                           │ RabbitMQ events          │
│  ┌────────────────────────▼──────────────────────┐  │
│  │              Order Saga Orchestrator           │  │
│  └────────────────────────┬──────────────────────┘  │
└───────────────────────────┼──────────────────────────┘
                            │ gRPC / HTTP
┌───────────────────────────▼──────────────────────────┐
│                   BankManager Service                  │
│                                                       │
│  Accounts │ Transactions │ Hold/Capture │ Refund      │
│                   (отдельная БД)                      │
└───────────────────────────────────────────────────────┘

Дополнительные сервисы:
  PostgreSQL + PostGIS  ← геопозиция курьеров
  Redis                 ← корзина, текущая позиция курьеров
  Meilisearch           ← поисковый индекс каталога
  RabbitMQ (уже есть)   ← события между модулями
```

---

## Роадмап по фазам

> Каждая фаза — завершённый рабочий срез. Не переходи к следующей, пока не написаны тесты для текущей.

---

### Фаза 0 — Принятие решений (до кода)

**Цель**: зафиксировать ответы на вопросы из раздела выше. Написать ADR (Architecture Decision Record) хотя бы в виде заметок.

Конкретные решения, которые нужно принять:
- [ ] Физические товары
- [ ] Один склад
- [ ] BankManager: отдельный процесс
- [ ] Симуляция курьеров: интерполяция
- [ ] Хранение адресов: строка (read model) vs нормализованная структура (в каталоге)

---

### Фаза 1 — Каталог товаров

**Роли**:
- `CmsAdmin` — полный CRUD товаров (название, описание, цена, категория, фото, остаток), публикация/архивация
- `GymAdmin` — только просмотр каталога: поиск, фильтры, страница товара, добавление в корзину

**Домен**:
```
Product
  Id, Name, Description
  Price (Money value object: Amount + Currency)
  CategoryId, Stock (остаток)
  Status: Draft | Published | Archived
  Photos: string[] (S3 ключи)
  SearchVector (tsvector — для PostgreSQL FTS как промежуточный шаг)

Category (иерархическая: Protein > Whey / Casein)
  Id, ParentId, Name, Slug
```

Нет поля `GymId` у `Product` — продавец единственный (сеть / `CmsAdmin`).

**Что реализовать**:
- [ ] CRUD товаров (только `CmsAdmin`)
- [ ] Загрузка фото через существующий S3-пайплайн
- [ ] Каталог для `GymAdmin`: базовый поиск через PostgreSQL FTS (`tsvector` + `tsquery`)
- [ ] Пагинация (сначала offset, потом переделать на cursor/keyset)
- [ ] Фильтры: категория, цена (min/max), в наличии

**Ключевые вопросы реализации**:
- `Money` как value object — как хранить в EF Core (owned entity или value converter)?
- Как обновить `tsvector` автоматически при изменении товара? (PostgreSQL trigger или EF interceptor)

**Скиллы фазы**: owned entities в EF Core, PostgreSQL FTS, keyset pagination, value objects, optimistic concurrency (xmin / RowVersion) при списании Stock

---

### Фаза 2 — Поисковый движок (Meilisearch)

**Цель**: заменить PostgreSQL FTS на выделенный поисковый движок. Это самостоятельная фаза, потому что паттерн синхронизации — отдельная большая тема.

**Что реализовать**:
- [ ] Поднять Meilisearch в docker-compose
- [ ] Настроить индекс: searchable attributes, filterable attributes, sortable attributes
- [ ] Outbox Pattern: при изменении Product → запись в `ProductSearchEvent` → фоновый воркер → Meilisearch
- [ ] Full-text поиск с подсветкой совпадений
- [ ] Фасетные фильтры через Meilisearch facets API
- [ ] Автодополнение (suggest / autocomplete endpoint)

**Outbox Pattern для синхронизации**:
```
1. CmsAdmin сохраняет/изменяет Product
2. В той же транзакции EF Core пишет запись в таблицу OutboxMessages
3. BackgroundService читает OutboxMessages, отправляет в Meilisearch, помечает как processed
4. Гарантия: Product и SearchEvent всегда консистентны (один коммит)
```

**Скиллы фазы**: Outbox Pattern, BackgroundService, Meilisearch API, eventual consistency

---

### Фаза 3 — Корзина

Корзина принадлежит `GymAdmin` — он выбирает товары для своего зала.

**Хранение в Redis** (не в PostgreSQL — корзина эфемерна):
```
Key: cart:{gymAdminId}
TTL: 7 дней (продлевается при каждом обращении)
Value: JSON { items: [{ productId, quantity, priceSnapshot }] }
```

**Почему снапшот цены важен**: `CmsAdmin` может изменить цену товара пока корзина `GymAdmin` активна. При оформлении заказа — сравниваем текущую цену с `priceSnapshot`, если изменилась — предупреждаем покупателя.

**Что реализовать**:
- [ ] Redis как хранилище корзины (StackExchange.Redis)
- [ ] Add / Update / Remove / Clear
- [ ] Проверка наличия товара при добавлении
- [ ] Истечение срока (TTL) и восстановление корзины из БД (опционально)
- [ ] Idempotency: повторный запрос Add не дублирует товар

**Скиллы фазы**: Redis, TTL-стратегии, idempotency

---

### Фаза 4 — BankManager

**Это отдельный проект** в Solution, со своей БД-схемой (или отдельным соединением).

**Концептуальная модель**:
```
BankAccount
  Id, OwnerId (userId или systemId), OwnerType (GymAdmin | CmsAdmin | Marketplace)
  Balance, HeldBalance
  Currency
  // GymAdmin — покупатель, пополняет баланс и платит
  // CmsAdmin — продавец, получает деньги после доставки
  // Marketplace — служебный счёт для комиссий (опционально)

Transaction
  Id, FromAccountId, ToAccountId, Amount, Type, Status, IdempotencyKey
  Type: Deposit | Hold | Capture | Release | Refund | Commission

PaymentSession
  Id, OrderId, Amount, Status: Pending | Held | Captured | Released
  ExpiresAt (hold истекает через N минут)
```

**Двухфазная оплата**:
```
1. Hold   — деньги заморожены на счету покупателя, но не переведены
2. Capture — после подтверждения доставки: перевод продавцу - комиссия маркетплейсу
   Release — при отмене заказа: разморозка холда
```

**API BankManager** (gRPC или REST):
```
POST /accounts            — создать счёт
POST /accounts/{id}/deposit  — пополнить (симуляция)
POST /payments/hold       — заморозить сумму
POST /payments/{id}/capture  — списать
POST /payments/{id}/release  — разморозить (отмена)
GET  /accounts/{id}/balance
GET  /accounts/{id}/transactions
```

**Что реализовать**:
- [ ] Проект `BankManager` (ASP.NET Core или minimal API)
- [ ] Счета и транзакции с полным аудит-логом
- [ ] Hold → Capture / Release flow
- [ ] Idempotency keys для всех мутирующих операций
- [ ] Симуляция задержки (Thread.Sleep или Task.Delay — "банк обрабатывает")
- [ ] Симуляция сбоев (настраиваемый failure rate для тестирования)

**Скиллы фазы**: gRPC (.NET), domain modeling финансовых операций, idempotency, симуляция сбоев, pessimistic locking (`SELECT FOR UPDATE`) + CHECK constraints для защиты баланса

---

### Фаза 5 — Оформление заказа (Order Saga)

**Самая сложная фаза**. Оформление заказа — это распределённая транзакция:

```
Шаг 1: Резервирование товаров (уменьшить Stock)
Шаг 2: Создание PaymentSession в BankManager (Hold со счёта GymAdmin)
Шаг 3: Создание Order в БД
Шаг 4: Уведомление CmsAdmin о новом заказе (SignalR / Email)

При сбое на любом шаге — компенсирующие транзакции:
- Шаг 2 упал → освободить Stock
- Шаг 3 упал → Release холда в BankManager + освободить Stock
```

**Saga Choreography vs Orchestration**:
- **Choreography**: каждый сервис реагирует на события и публикует свои → слабая связность, но сложно отлаживать
- **Orchestration**: один `OrderSagaOrchestrator` знает весь сценарий и вызывает сервисы → легче читать и тестировать
- Рекомендация для пет-проекта: **Orchestration** — код saga в одном месте, легче понять

**Конечный автомат заказа** (библиотека Stateless):
```csharp
// Состояния
enum OrderState { New, PaymentPending, PaymentHeld, Assembling,
                  PickedUp, InTransit, Delivered, Cancelled, Refunded }

// Триггеры
enum OrderTrigger { PlaceOrder, PaymentHeld, PaymentFailed,
                    StartAssembly, HandedToCourier, Delivered,
                    Cancel, RefundIssued }
```

**Что реализовать**:
- [ ] `Order` aggregate с конечным автоматом (Stateless)
- [ ] `OrderSagaOrchestrator` — координирует Catalog (stock) + BankManager (hold)
- [ ] Компенсирующие транзакции при сбоях
- [ ] Таймаут оплаты: заказ не оплачен за 15 мин → автоматическая отмена (Hangfire)
- [ ] События:
  - `OrderPlaced` → SignalR push `CmsAdmin` ("новый заказ от зала X")
  - `OrderStatusChanged` → SignalR push `GymAdmin` ("ваш заказ собирается / передан курьеру")
  - `OrderCancelled` → Email обеим сторонам

**Скиллы фазы**: Saga Pattern, конечные автоматы (Stateless), компенсирующие транзакции, Hangfire

---

### Фаза 6 — Доставка и курьеры

**Модель данных**:
```
Courier
  Id, Name, Phone, Status: Available | Busy | Offline
  CurrentLocation: geometry(Point, 4326)  ← PostGIS
  LastLocationAt

Delivery
  Id, OrderId, CourierId
  PickupLocation  (координаты склада сети — из конфига или отдельной сущности Warehouse)
  DropoffLocation (координаты зала GymAdmin — берётся из существующей сущности Gym)
  Status: Assigned | PickedUp | InTransit | Delivered | Failed
  EstimatedArrivalAt

LocationHistory (опционально, для аналитики)
  CourierId, Location, RecordedAt
```

**PostGIS queries**:
```sql
-- Ближайшие свободные курьеры
SELECT id, name,
  ST_Distance(current_location, ST_MakePoint(:lng, :lat)::geography) AS distance_meters
FROM couriers
WHERE status = 'Available'
ORDER BY distance_meters
LIMIT 5;

-- Курьеры в радиусе 5 км
SELECT id FROM couriers
WHERE ST_DWithin(
  current_location::geography,
  ST_MakePoint(:lng, :lat)::geography,
  5000
);
```

**Что реализовать**:
- [ ] PostGIS в docker-compose (расширение для PostgreSQL)
- [ ] NetTopologySuite в EF Core для работы с геометрией
- [ ] Добавить `Location: geometry(Point, 4326)` к существующей сущности `Gym`
- [ ] CRUD курьеров (`CmsAdmin`): создание, просмотр всех курьеров на карте
- [ ] Назначение курьера на заказ (ближайший свободный к складу)
- [ ] Обновление позиции курьера → Redis (текущая позиция) + PostgreSQL (история)
- [ ] SignalR хаб `CourierTrackingHub`:
  - `CmsAdmin` подписывается на всех курьеров (карта диспетчера)
  - `GymAdmin` подписывается только на курьера своего текущего заказа (Яндекс Еда view)

**Скиллы фазы**: PostGIS, NetTopologySuite, геопространственные запросы, SignalR (расширение существующего), `SELECT FOR UPDATE SKIP LOCKED` при назначении курьера

---

### Фаза 7 — Симуляция курьеров

> Это отдельный, очень интересный модуль.

**Проблема**: как тестировать систему доставки без реальных людей на улице?

**Решение**: `CourierSimulatorService` — `BackgroundService`, который симулирует движение курьеров.

**Как работает симуляция**:

```
1. Симулятор знает: текущую позицию курьера, точку назначения
2. Каждые N секунд (настраивается) рассчитывает следующую позицию:
   nextLat = currentLat + (targetLat - currentLat) * stepFraction
   nextLng = currentLng + (targetLng - currentLng) * stepFraction
3. Обновляет позицию в Redis + публикует событие LocationUpdated
4. SignalR доставляет обновление на фронт
5. Когда курьер достиг точки назначения → событие CourierArrived
```

**Настройки симуляции**:
```json
{
  "CourierSimulation": {
    "Enabled": true,
    "UpdateIntervalSeconds": 3,
    "SpeedMultiplier": 10,
    "FailureRate": 0.02,
    "SimulatedCouriersCount": 5
  }
}
```

---

## Конкурентный доступ к данным

Маркетплейс — это классическая конкурентная среда: несколько `GymAdmin` одновременно покупают один товар, два заказа претендуют на одного курьера, пользователь нажимает "Оплатить" дважды. Ниже — все конкретные сценарии, которые возникнут в этом проекте, и способы их решения.

---

### Сценарий 1 — "Последний товар" (Stock списание)

**Проблема**: два `GymAdmin` одновременно оформляют заказ на последний экземпляр товара. Оба читают `Stock = 1`, оба проходят проверку, оба сохраняют `Stock = 0`. В итоге продано 2 штуки при наличии 1.

```
GymAdmin A: READ stock=1 → ok → ... → WRITE stock=0  ✓
GymAdmin B: READ stock=1 → ok → ... → WRITE stock=0  ✓  ← двойная продажа!
```

#### Решение A: Optimistic Concurrency (RowVersion)

Подходит когда конфликты редки. EF Core добавляет `WHERE row_version = @original` к UPDATE — если строка изменилась, выбрасывается `DbUpdateConcurrencyException`.

```csharp
// Конфигурация EF Core
builder.Property(p => p.RowVersion).IsRowVersion(); // автоматический timestamp в PostgreSQL → xmin

// Применение
public async Task ReserveStockAsync(ProductId productId, int quantity)
{
    var product = await _db.Products.FindAsync(productId);

    if (product.Stock < quantity)
        throw new InsufficientStockException();

    product.Stock -= quantity;

    try
    {
        await _db.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        // Другой поток успел раньше — перезагружаем и повторяем
        await _db.Entry(product).ReloadAsync();
        throw new StockReservationConflictException("Остаток изменился, повторите попытку");
    }
}
```

PostgreSQL-специфика: вместо отдельной колонки `rowversion` можно использовать системный столбец `xmin`:
```csharp
builder.Property<uint>("xmin")
    .HasColumnName("xmin")
    .HasColumnType("xid")
    .ValueGeneratedOnAddOrUpdate()
    .IsConcurrencyToken();
```

#### Решение B: Pessimistic Locking (`SELECT FOR UPDATE`)

Подходит когда конфликты часты или цена ошибки высока. Строка блокируется на уровне БД — второй поток ждёт.

```csharp
public async Task ReserveStockAsync(ProductId productId, int quantity)
{
    await using var tx = await _db.Database.BeginTransactionAsync(
        IsolationLevel.ReadCommitted);

    // Блокируем строку до конца транзакции
    var product = await _db.Products
        .FromSqlInterpolated(
            $"SELECT * FROM products WHERE id = {productId} FOR UPDATE")
        .SingleAsync();

    if (product.Stock < quantity)
    {
        await tx.RollbackAsync();
        throw new InsufficientStockException();
    }

    product.Stock -= quantity;
    await _db.SaveChangesAsync();
    await tx.CommitAsync();
}
```

#### Когда что использовать

| Ситуация | Выбор | Почему |
|----------|-------|--------|
| Конфликты редки (большой каталог) | Optimistic | Нет блокировок, выше throughput |
| Конфликты часты (популярный товар, 1 шт) | Pessimistic | Нет retry-логики, предсказуемо |
| Финансовые операции (BankManager) | Pessimistic | Нельзя допустить overdraft |
| Назначение курьера | `SKIP LOCKED` | Нужна атомарная очередь |

---

### Сценарий 2 — Назначение курьера (`SELECT FOR UPDATE SKIP LOCKED`)

**Проблема**: два заказа одновременно ищут ближайшего свободного курьера. Оба находят одного и того же → двойное назначение.

**Решение**: `FOR UPDATE SKIP LOCKED` — атомарно захватывает первую незаблокированную строку. Второй запрос автоматически получает следующего курьера в очереди, не ждёт первого.

```csharp
public async Task<CourierId?> AssignNearestCourierAsync(
    OrderId orderId,
    Point pickupLocation)
{
    await using var tx = await _db.Database.BeginTransactionAsync();

    // Атомарно берём ближайшего свободного курьера и сразу блокируем строку.
    // SKIP LOCKED: если курьера уже захватил другой поток — пропускаем его,
    // берём следующего. Нет ожидания, нет дедлоков.
    var courier = await _db.Couriers
        .FromSqlInterpolated($"""
            SELECT * FROM couriers
            WHERE status = 'Available'
            ORDER BY ST_Distance(current_location, {pickupLocation})
            LIMIT 1
            FOR UPDATE SKIP LOCKED
            """)
        .FirstOrDefaultAsync();

    if (courier is null)
    {
        await tx.RollbackAsync();
        return null; // нет свободных курьеров
    }

    courier.Status = CourierStatus.Busy;
    courier.CurrentOrderId = orderId;
    await _db.SaveChangesAsync();
    await tx.CommitAsync();

    return courier.Id;
}
```

`SKIP LOCKED` также полезен для воркеров, которые обрабатывают очередь заданий — каждый воркер атомарно забирает задание, не пересекаясь с другими.

---

### Сценарий 3 — Баланс в BankManager (не уйти в минус)

**Проблема**: `GymAdmin` одновременно оплачивает два заказа. Оба читают баланс 1000р, оба проходят проверку, оба списывают 800р. Итог: -600р.

**Решение**: три уровня защиты одновременно.

```csharp
// Уровень 1: CHECK constraint в PostgreSQL — последний рубеж, ломает транзакцию
migrationBuilder.Sql("""
    ALTER TABLE bank_accounts
    ADD CONSTRAINT chk_balance_non_negative
    CHECK (balance >= 0 AND held_balance >= 0);
    """);

// Уровень 2: SELECT FOR UPDATE при любой финансовой операции
public async Task<HoldResult> HoldAsync(HoldRequest request)
{
    await using var tx = await _db.Database.BeginTransactionAsync(
        IsolationLevel.ReadCommitted);

    var account = await _db.BankAccounts
        .FromSqlInterpolated(
            $"SELECT * FROM bank_accounts WHERE id = {request.AccountId} FOR UPDATE")
        .SingleAsync();

    var available = account.Balance - account.HeldBalance;
    if (available < request.Amount)
        return HoldResult.InsufficientFunds(available);

    account.HeldBalance += request.Amount;
    await _db.SaveChangesAsync();
    await tx.CommitAsync();

    return HoldResult.Success();
}

// Уровень 3: Idempotency key — повторный запрос с тем же ключом
// возвращает кэшированный результат, не создаёт вторую транзакцию
public async Task<HoldResult> HoldAsync(HoldRequest request)
{
    var existing = await _db.Transactions
        .FirstOrDefaultAsync(t => t.IdempotencyKey == request.IdempotencyKey);
    if (existing is not null)
        return HoldResult.FromTransaction(existing); // уже выполнено

    // ... основная логика
}
```

---

### Сценарий 4 — Двойное нажатие кнопки (Idempotent state transitions)

**Проблема**: `CmsAdmin` нажимает "Начать сборку" дважды. Или `GymAdmin` нажимает "Отменить" дважды. Второй запрос не должен сломать систему.

**Решение**: конечный автомат (`Stateless`) делает переходы идемпотентными — повторный переход в то же состояние либо игнорируется, либо возвращает текущее состояние без ошибки.

```csharp
public async Task<Order> StartAssemblyAsync(OrderId orderId)
{
    var order = await _db.Orders.FindAsync(orderId);

    // Если заказ уже собирается — просто возвращаем, не кидаем исключение
    if (order.Status == OrderStatus.Assembling)
        return order;

    // Если статус не позволяет переход — это реальная ошибка
    if (!order.StateMachine.CanFire(OrderTrigger.StartAssembly))
        throw new InvalidOrderTransitionException(order.Status, OrderTrigger.StartAssembly);

    order.StateMachine.Fire(OrderTrigger.StartAssembly);
    await _db.SaveChangesAsync();
    return order;
}
```

Для защиты на уровне БД — уникальный индекс или optimistic concurrency на поле `Status`:
```csharp
// Если два потока одновременно пытаются изменить статус — один получит
// DbUpdateConcurrencyException и будет отклонён
builder.Property(o => o.StatusVersion).IsConcurrencyToken();
```

---

### Сценарий 5 — Serializable isolation (строгая гарантия)

**Когда нужен**: когда Optimistic/Pessimistic недостаточно и нужна полная гарантия "как будто запросы выполняются последовательно". Пример: "продать товар, только если и Stock > 0, и курьер есть — оба условия в одной атомарной проверке".

```csharp
await using var tx = await _db.Database.BeginTransactionAsync(
    IsolationLevel.Serializable); // PostgreSQL: SSI (Serializable Snapshot Isolation)

// Оба READ выполнятся как часть одного serializable snapshot
var stock = await _db.Products...
var courier = await _db.Couriers...

if (stock.Available > 0 && courier is not null)
{
    // ... резервируем оба ресурса
    await _db.SaveChangesAsync();
    await tx.CommitAsync();
}
// При конфликте PostgreSQL выбросит serialization failure (код 40001) →
// нужно поймать и повторить транзакцию
```

Serializable дорогой по производительности — применять точечно, не глобально.

---

### Где какой паттерн применять в этом проекте

| Место в коде | Паттерн | Уровень изоляции |
|---|---|---|
| `Product.Stock` при оформлении заказа | `FOR UPDATE` или Optimistic (xmin) | ReadCommitted |
| `BankAccount.Balance` при любой операции | `FOR UPDATE` + CHECK constraint | ReadCommitted |
| Назначение курьера на заказ | `FOR UPDATE SKIP LOCKED` | ReadCommitted |
| Изменение статуса заказа | Idempotent check + Optimistic | ReadCommitted |
| Проверка "товар + курьер" как единица | Serializable | Serializable |
| Корзина в Redis | `WATCH` / Lua script | — (Redis single-threaded) |

---

## Тестирование без реальных курьеров

Это отдельная тема, заслуживающая глубокого изучения.

### 1. Абстракция времени (ITimeProvider)

Любой код, зависящий от времени (`DateTime.Now`, `Task.Delay`) — сложно тестировать. Решение:

```csharp
public interface ITimeProvider
{
    DateTimeOffset UtcNow { get; }
}

// Продакшн
public class SystemTimeProvider : ITimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

// В тестах
public class FakeTimeProvider : ITimeProvider
{
    private DateTimeOffset _current;
    public FakeTimeProvider(DateTimeOffset start) => _current = start;
    public DateTimeOffset UtcNow => _current;
    public void Advance(TimeSpan delta) => _current += delta;
}
```

Тест истечения заказа (без ожидания 15 минут реального времени):

```csharp
[Fact]
public async Task Order_ShouldBeCancelled_WhenPaymentTimeoutExpires()
{
    var clock = new FakeTimeProvider(DateTimeOffset.UtcNow);
    // ... регистрируем clock в DI
    var order = await PlaceOrderAsync();

    clock.Advance(TimeSpan.FromMinutes(16)); // телепортируем время
    await _sagaOrchestrator.ProcessTimeoutsAsync();

    order.Status.ShouldBe(OrderStatus.Cancelled);
}
```

### 2. Симулятор маршрутов (RouteGenerator)

```csharp
public class RouteGenerator
{
    // Генерирует список координат от A до B с N шагами
    // Можно добавить случайный шум для реализма
    public IReadOnlyList<GeoPoint> GenerateRoute(
        GeoPoint from,
        GeoPoint to,
        int steps,
        double noiseMeters = 0)
    {
        var points = new List<GeoPoint>();
        for (int i = 0; i <= steps; i++)
        {
            var fraction = (double)i / steps;
            var lat = from.Lat + (to.Lat - from.Lat) * fraction;
            var lng = from.Lng + (to.Lng - from.Lng) * fraction;

            if (noiseMeters > 0)
            {
                lat += (Random.Shared.NextDouble() - 0.5) * noiseMeters / 111000;
                lng += (Random.Shared.NextDouble() - 0.5) * noiseMeters / 111000;
            }

            points.Add(new GeoPoint(lat, lng));
        }
        return points;
    }
}
```

### 3. Управляемый симулятор для интеграционных тестов

```csharp
// В тестах подменяем реальный BackgroundService на управляемый симулятор
public class ManualCourierSimulator : ICourierSimulator
{
    private readonly Dictionary<Guid, Queue<GeoPoint>> _routes = new();

    public void EnqueueRoute(Guid courierId, IEnumerable<GeoPoint> points)
        => _routes[courierId] = new Queue<GeoPoint>(points);

    // Вызывается явно в тесте — полный контроль
    public async Task<bool> AdvanceOnceAsync(Guid courierId)
    {
        if (!_routes.TryGetValue(courierId, out var queue) || queue.Count == 0)
            return false;

        var nextPoint = queue.Dequeue();
        await UpdateCourierLocationAsync(courierId, nextPoint);
        return queue.Count > 0;
    }
}

// Тест
[Fact]
public async Task Customer_ShouldReceiveLocationUpdate_WhenCourierMoves()
{
    var simulator = new ManualCourierSimulator();
    var route = _routeGenerator.GenerateRoute(gymLocation, customerLocation, steps: 10);
    simulator.EnqueueRoute(courierId, route);

    var receivedUpdates = new List<GeoPoint>();
    // подписаться на SignalR хаб через тестовый клиент

    await simulator.AdvanceOnceAsync(courierId);

    receivedUpdates.Count.ShouldBe(1);
    receivedUpdates[0].ShouldBeNear(route[1], toleranceMeters: 1);
}
```

### 4. Property-based тестирование (FsCheck / CsCheck)

Вместо написания конкретных примеров — проверяем **инварианты**:

```csharp
// Инвариант: баланс аккаунта никогда не уходит в минус
[Property]
public Property Account_BalanceShouldNeverGoBelowZero(
    decimal initialBalance,
    decimal[] withdrawals)
{
    // Arrange: аккаунт с начальным балансом
    // Act: применить все withdrawals (игнорировать ошибки)
    // Assert: итоговый баланс >= 0
}

// Инвариант: сумма всех Hold + Available = Total всегда
[Property]
public Property Account_HeldPlusAvailableEqualsTotalAlways(...) { }
```

### 5. Chaos Testing (симуляция сбоев BankManager)

```csharp
// В тестах или Development-окружении
public class ChaosBankManagerClient : IBankManagerClient
{
    private readonly IBankManagerClient _inner;
    private readonly double _failureRate;

    public async Task<HoldResult> HoldAsync(HoldRequest request)
    {
        if (Random.Shared.NextDouble() < _failureRate)
            throw new BankManagerUnavailableException("Simulated failure");

        return await _inner.HoldAsync(request);
    }
}

// appsettings.Development.json
{
  "BankManager": {
    "SimulatedFailureRate": 0.1,  // 10% сбоев
    "SimulatedDelayMs": 500
  }
}
```

### 6. Сценарные тесты (Scenario Testing)

Полный сценарий от добавления в корзину до "Доставлено":

```csharp
[Fact]
public async Task FullDeliveryScenario_HappyPath()
{
    // 1. CmsAdmin (продавец) публикует товар
    using var cmsAdminClient = _fixture.CreateCmsAdminClient();
    var product = await cmsAdminClient.PublishProductAsync(price: 1000m, stock: 5);

    // 2. GymAdmin (покупатель) добавляет в корзину
    using var gymAdminClient = _fixture.CreateGymAdminClient(gymId: _fixture.GymId);
    await gymAdminClient.AddToCartAsync(product.Id, quantity: 2);

    // 3. GymAdmin оформляет заказ → BankManager холдирует 2000р со счёта GymAdmin
    var order = await gymAdminClient.CheckoutAsync();
    order.Status.ShouldBe(OrderStatus.PaymentHeld);
    (await GetBankBalance(gymAdminId)).Held.ShouldBe(2000m);

    // 4. CmsAdmin видит новый заказ и начинает сборку
    await cmsAdminClient.StartAssemblyAsync(order.Id);
    order.Status.ShouldBe(OrderStatus.Assembling);

    // 5. CmsAdmin назначает курьера; симулятор проигрывает маршрут склад → зал
    var courier = await cmsAdminClient.AssignCourierAsync(order.Id);
    await _simulator.RunFullRouteAsync(courier.Id); // склад → координаты зала GymAdmin

    // 6. Курьер доставил → заказ закрыт, деньги переведены на счёт CmsAdmin
    await cmsAdminClient.MarkDeliveredAsync(order.Id);
    order.Status.ShouldBe(OrderStatus.Delivered);
    (await GetBankBalance(gymAdminId)).Held.ShouldBe(0m);
    (await GetBankBalance(cmsAdminId)).Available.ShouldBeGreaterThan(0m);

    // 7. Stock уменьшился
    (await GetProduct(product.Id)).Stock.ShouldBe(3);
}
```