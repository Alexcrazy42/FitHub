# Roadmap

## Цель

Система маркетплейса взаимоотношения CMS Admin - Gym Admin по поставке товаров в конкретные залы с доставкой и оплатой

## Роли

CmsAdmin - ведение каталога, добавление курьеров
GymAdmin - просмотр каталога, покупка, отслеживание статуса
Courier - доставка товаров (новая роль без интерфейса)
Фон системы - назначение/переназначение курьеров, пересмотр каталога (цены, наличие, read models и тд)

## Особенности

1. Платежного шлюза не будет, хочется видеть исключительно мок в виде микросервиса BankManager, который будет инкапсулировать действия от банков. По факту это будет являться стабом, который может применяться в dev/regress окружении
2. Курьеры виртуальные - их действия должны имитироваться (взял заказ, в пути за заказом, отказался от заказа и тд)
3. Под поиск в каталоге нужен какой нибудь OpenSearch/ElasticSearch для поиска. Также возможно что само наполнение каталога будет машинным, а не вручную (тоже подумать)
4. Под поиск в каталоге хочется видеть что-то похожее на https://www.sportmaster.ru/catalog/muzhskaya_obuv/krossovki

---

## Роадмап задачки

> Каждая фаза — завершённый рабочий срез. Не переходить к следующей до написания тестов на текущую.
> Детальное описание каждой фазы — в [README.md](./README.md).

### Фаза 0 — Подготовка инфраструктуры и принятие решений

- [ ] Добавить в `docker-compose`: PostGIS (расширение над существующим PostgreSQL-контейнером), Redis, Meilisearch
- [ ] Добавить `NetTopologySuite` в EF Core конфигурацию
- [ ] Добавить поле `Location: geometry(Point, 4326)` к существующей сущности `Gym`
- [ ] Создать проект `BankManager` в Solution (`src/BankManager`) — ASP.NET Core Minimal API, отдельная БД-схема
- [ ] Зафиксировать ADR: поисковый движок (Meilisearch vs PostgreSQL FTS), routing курьеров (прямолинейная интерполяция vs OSRM)

---

### Фаза 1 — Каталог товаров (Catalog)

**Домен:** `Product`, `Category`, `ProductPhoto`

- [ ] Миграция: таблицы `products`, `categories`, `product_photos`, `outbox_messages`
- [ ] CRUD товаров — только `CmsAdmin` (create, update, publish, archive)
- [ ] Иерархия категорий (`ParentId`): дерево категорий для фильтрации
- [ ] Загрузка фото через существующий S3-пайплайн (presigned URL)
- [ ] Каталог для `GymAdmin`: список с пагинацией, фильтры по категории / цене / наличию
- [ ] PostgreSQL FTS (`tsvector`) как первый шаг поиска — заменяется в фазе 2
- [ ] Optimistic concurrency (xmin) на `Product.Stock`
- [ ] Unit-тесты: валидация доменной модели, value object `Money`
- [ ] Интеграционные тесты: CRUD + проверка конкурентного списания Stock

---

### Фаза 2 — Поиск (Meilisearch)

- [ ] Настроить индекс Meilisearch: searchable / filterable / sortable / facets атрибуты
- [ ] Outbox Pattern: при `SaveChanges` на `Product` → запись `ProductIndexEvent` в `outbox_messages`
- [ ] `ProductIndexWorker` (BackgroundService): читает outbox → пушит в Meilisearch → помечает processed
- [ ] Endpoint автодополнения (`/catalog/products/suggest`)
- [ ] Фасетные агрегации: категория + ценовой диапазон + наличие (как на Sportmaster)
- [ ] Тесты: Outbox-консистентность (что в Meilisearch попадает то, что в БД)

---

### Фаза 3 — Корзина (Cart)

- [ ] `StackExchange.Redis` в DI
- [ ] Корзина в Redis с ключом `cart:{gymAdminId}`, TTL 7 дней
- [ ] Add / Update / Remove / Clear; снапшот цены (`priceSnapshot`) при добавлении
- [ ] Предупреждение покупателю если цена изменилась с момента добавления в корзину
- [ ] Idempotency: повторный `Add` обновляет quantity, не дублирует позицию
- [ ] Тесты: TTL-логика (FakeClock), конкурентные добавления

---

### Фаза 4 — BankManager

- [ ] Проект `src/BankManager`: счета (`BankAccount`), транзакции (`Transaction`), сессии оплаты (`PaymentSession`)
- [ ] Flow: `Deposit` → `Hold` → `Capture` / `Release`
- [ ] CHECK constraint в БД: `balance >= 0 AND held_balance >= 0`
- [ ] `SELECT FOR UPDATE` при любой финансовой операции
- [ ] Idempotency keys на всех мутирующих операциях
- [ ] Настраиваемый `FailureRate` и `DelayMs` для dev/тестовых окружений
- [ ] Тесты: property-based (баланс никогда не уходит в минус), сценарий двойного Hold

---

### Фаза 5 — Оформление заказа (Order Saga)

- [ ] `Order` aggregate + конечный автомат (Stateless): состояния и триггеры из README
- [ ] `OrderSagaOrchestrator`: резерв Stock → Hold в BankManager → создание Order
- [ ] Компенсирующие транзакции при сбое на любом шаге
- [ ] Таймаут оплаты 15 мин (Hangfire / BackgroundService + FakeClock в тестах)
- [ ] SignalR push: `CmsAdmin` получает "новый заказ", `GymAdmin` — смену статуса
- [ ] Idempotent state transitions: повторный запрос на смену статуса не ломает систему
- [ ] Сценарный тест полного happy path (см. README)

---

### Фаза 6 — Доставка и курьеры (Delivery)

- [ ] Миграция: таблицы `couriers`, `deliveries`, `courier_location_history`
- [ ] CRUD курьеров (`CmsAdmin`)
- [ ] Назначение курьера: `SELECT FOR UPDATE SKIP LOCKED` — ближайший свободный к складу
- [ ] `CourierTrackingHub` (SignalR):
  - `CmsAdmin` → видит всех курьеров на карте (диспетчерская)
  - `GymAdmin` → видит только курьера своего заказа + ETA (Яндекс Еда view)
- [ ] Обновление позиции: Redis (текущая) + PostgreSQL (история)
- [ ] Тесты: конкурентное назначение двух заказов на одного курьера

---

### Фаза 7 — Симулятор курьеров (CourierSimulator)

- [ ] `CourierSimulatorService` (BackgroundService): интерполяция координат склад → зал с настраиваемой скоростью
- [ ] Сценарии симуляции: happy path, отказ курьера, пропадание с GPS, опоздание
- [ ] `ManualCourierSimulator` для интеграционных тестов (пошаговое управление)
- [ ] Настройки через `appsettings`: `Enabled`, `UpdateIntervalSeconds`, `SpeedMultiplier`, `FailureRate`
- [ ] Chaos-тест: курьер пропадает на 2 минуты в середине маршрута

---

### Фаза 8 — Фронтенд

- [ ] Каталог с фильтрами (фасеты, поиск, пагинация) — `GymAdmin`
- [ ] Страница товара + добавление в корзину
- [ ] Checkout flow: корзина → оформление → статус оплаты
- [ ] Страница заказов `GymAdmin` (список + детали)
- [ ] Страница заказов `CmsAdmin` (все заказы, кнопки: начать сборку, назначить курьера, доставлено)
- [ ] Карта трекинга курьера (Leaflet.js или Yandex Maps) — `GymAdmin`
- [ ] Диспетчерская карта всех курьеров — `CmsAdmin`
- [ ] Управление каталогом — `CmsAdmin` (CRUD товаров, категории)

---

## Предполагаемые апи ручки и консюмеры

### Каталог (`/api/v1/catalog`)

| Метод | URL | Роль | Описание |
|---|---|---|---|
| `GET` | `/products` | GymAdmin | Список с фильтрами, поиском, пагинацией |
| `GET` | `/products/{id}` | GymAdmin | Детальная страница товара |
| `GET` | `/products/suggest` | GymAdmin | Автодополнение поиска |
| `GET` | `/categories` | GymAdmin | Дерево категорий |
| `POST` | `/products` | CmsAdmin | Создать товар (Draft) |
| `PUT` | `/products/{id}` | CmsAdmin | Обновить товар |
| `POST` | `/products/{id}/publish` | CmsAdmin | Опубликовать |
| `POST` | `/products/{id}/archive` | CmsAdmin | Архивировать |
| `POST` | `/products/{id}/photos` | CmsAdmin | Загрузить фото (presigned URL flow) |
| `DELETE` | `/products/{id}/photos/{photoId}` | CmsAdmin | Удалить фото |

Query-параметры `GET /products`: `q`, `categoryId`, `minPrice`, `maxPrice`, `inStock`, `sortBy`, `cursor`, `limit`

---

### Корзина (`/api/v1/cart`)

| Метод | URL | Роль | Описание |
|---|---|---|---|
| `GET` | `/` | GymAdmin | Получить корзину |
| `POST` | `/items` | GymAdmin | Добавить позицию |
| `PUT` | `/items/{productId}` | GymAdmin | Изменить количество |
| `DELETE` | `/items/{productId}` | GymAdmin | Удалить позицию |
| `DELETE` | `/` | GymAdmin | Очистить корзину |

---

### Заказы (`/api/v1/orders`)

| Метод | URL | Роль | Описание |
|---|---|---|---|
| `POST` | `/` | GymAdmin | Оформить заказ (checkout из корзины) |
| `GET` | `/my` | GymAdmin | Мои заказы |
| `GET` | `/{id}` | GymAdmin, CmsAdmin | Детали заказа |
| `GET` | `/` | CmsAdmin | Все заказы (с фильтрами по статусу) |
| `POST` | `/{id}/start-assembly` | CmsAdmin | Начать сборку |
| `POST` | `/{id}/cancel` | GymAdmin, CmsAdmin | Отменить заказ |

---

### Доставка (`/api/v1/deliveries`)

| Метод | URL | Роль | Описание |
|---|---|---|---|
| `POST` | `/{orderId}/assign` | CmsAdmin | Назначить ближайшего курьера |
| `POST` | `/{orderId}/delivered` | CmsAdmin | Подтвердить доставку |
| `GET` | `/{orderId}/tracking` | GymAdmin | Текущая позиция курьера + ETA |

---

### Курьеры (`/api/v1/couriers`)

| Метод | URL | Роль | Описание |
|---|---|---|---|
| `GET` | `/` | CmsAdmin | Все курьеры с текущими позициями |
| `POST` | `/` | CmsAdmin | Создать курьера |
| `PUT` | `/{id}` | CmsAdmin | Обновить данные |
| `DELETE` | `/{id}` | CmsAdmin | Удалить курьера |
| `GET` | `/{id}/history` | CmsAdmin | История маршрута |

---

### BankManager (`/api/bank`) — отдельный сервис

| Метод | URL | Описание |
|---|---|---|
| `POST` | `/accounts` | Создать счёт |
| `POST` | `/accounts/{id}/deposit` | Пополнить баланс (симуляция) |
| `GET` | `/accounts/{id}/balance` | Текущий баланс |
| `GET` | `/accounts/{id}/transactions` | История операций |
| `POST` | `/payments/hold` | Заморозить сумму (Hold) |
| `POST` | `/payments/{id}/capture` | Списать после доставки (Capture) |
| `POST` | `/payments/{id}/release` | Разморозить при отмене (Release) |

Все мутирующие операции принимают заголовок `Idempotency-Key`.

---

### SignalR хабы

#### `CourierTrackingHub` (`/hubs/courier-tracking`)

| Событие | Направление | Получатель | Данные |
|---|---|---|---|
| `JoinOrderTracking` | Client → Server | GymAdmin | `orderId` |
| `JoinDispatcherRoom` | Client → Server | CmsAdmin | — |
| `CourierLocationUpdated` | Server → Client | GymAdmin / CmsAdmin | `{ courierId, lat, lng, etaMinutes }` |
| `OrderStatusChanged` | Server → Client | GymAdmin | `{ orderId, newStatus, message }` |
| `NewOrderReceived` | Server → Client | CmsAdmin | `{ orderId, gymName, totalAmount }` |

---

### RabbitMQ: события и консюмеры

#### События, публикуемые основным приложением

| Событие | Публикует | Когда |
|---|---|---|
| `OrderPlacedEvent` | Orders | GymAdmin оформил заказ |
| `StockReservedEvent` | Catalog | Stock успешно уменьшен |
| `StockReservationFailedEvent` | Catalog | Нет нужного количества |
| `CourierAssignedEvent` | Delivery | Курьер назначен на заказ |
| `CourierLocationUpdatedEvent` | Simulator | Каждый тик симулятора |
| `DeliveryCompletedEvent` | Delivery | Курьер доставил заказ |
| `DeliveryFailedEvent` | Delivery | Курьер не смог доставить |
| `OrderCancelledEvent` | Orders | Заказ отменён (любой стороной) |

#### События, публикуемые BankManager

| Событие | Когда |
|---|---|
| `PaymentHeldEvent` | Hold прошёл успешно |
| `PaymentFailedEvent` | Hold не прошёл (недостаток средств / сбой) |
| `PaymentCapturedEvent` | Деньги переведены продавцу |
| `PaymentReleasedEvent` | Холд разморожен при отмене |

#### Консюмеры в основном приложении (Saga Orchestrator)

| Консюмер | Реагирует на | Действие |
|---|---|---|
| `OrderPlacedConsumer` | `OrderPlacedEvent` | Резервирует Stock → отправляет Hold в BankManager |
| `PaymentHeldConsumer` | `PaymentHeldEvent` | Переводит заказ в `PaymentHeld`, уведомляет CmsAdmin |
| `PaymentFailedConsumer` | `PaymentFailedEvent` | Освобождает Stock, отменяет заказ |
| `DeliveryCompletedConsumer` | `DeliveryCompletedEvent` | Запускает Capture в BankManager, закрывает заказ |
| `DeliveryFailedConsumer` | `DeliveryFailedEvent` | Release холда, возможно переназначение курьера |
| `OrderCancelledConsumer` | `OrderCancelledEvent` | Release холда, возврат Stock, уведомления |
| `CourierLocationConsumer` | `CourierLocationUpdatedEvent` | Обновляет Redis + SignalR broadcast |

---

## Схема БД

### Основная БД (PostgreSQL + PostGIS)

```sql
-- Категории товаров (иерархия)
categories
  id            uuid PK
  parent_id     uuid FK → categories(id) NULL   -- NULL = корневая
  name          varchar(100) NOT NULL
  slug          varchar(100) NOT NULL UNIQUE

-- Товары
products
  id             uuid PK
  name           varchar(200) NOT NULL
  description    text
  price_amount   numeric(18,2) NOT NULL
  price_currency char(3) NOT NULL DEFAULT 'RUB'
  category_id    uuid FK → categories(id)
  stock          int NOT NULL DEFAULT 0 CHECK (stock >= 0)
  status         varchar(20) NOT NULL            -- Draft | Published | Archived
  search_vector  tsvector                         -- обновляется триггером
  xmin           xid                              -- optimistic concurrency (системный)
  created_at     timestamptz NOT NULL
  updated_at     timestamptz NOT NULL

-- Фотографии товаров
product_photos
  id          uuid PK
  product_id  uuid FK → products(id) ON DELETE CASCADE
  s3_key      varchar NOT NULL
  sort_order  int NOT NULL DEFAULT 0

-- Заказы
orders
  id              uuid PK
  gym_admin_id    uuid NOT NULL                  -- FK → users(id)
  status          varchar(30) NOT NULL           -- конечный автомат
  status_version  int NOT NULL DEFAULT 0         -- optimistic concurrency на статус
  total_amount    numeric(18,2) NOT NULL
  total_currency  char(3) NOT NULL DEFAULT 'RUB'
  payment_session_id uuid NULL                   -- FK → BankManager.payment_sessions
  created_at      timestamptz NOT NULL
  updated_at      timestamptz NOT NULL

-- Позиции заказа
order_items
  id               uuid PK
  order_id         uuid FK → orders(id) ON DELETE CASCADE
  product_id       uuid FK → products(id)
  quantity         int NOT NULL CHECK (quantity > 0)
  price_amount     numeric(18,2) NOT NULL        -- снапшот цены на момент заказа
  price_currency   char(3) NOT NULL

-- Курьеры
couriers
  id               uuid PK
  name             varchar(100) NOT NULL
  phone            varchar(20)
  status           varchar(20) NOT NULL           -- Available | Busy | Offline
  current_location geometry(Point, 4326) NULL     -- PostGIS
  current_order_id uuid NULL                      -- FK → orders(id)
  last_location_at timestamptz NULL
  created_at       timestamptz NOT NULL

-- Доставки
deliveries
  id                  uuid PK
  order_id            uuid NOT NULL UNIQUE FK → orders(id)
  courier_id          uuid FK → couriers(id)
  pickup_location     geometry(Point, 4326) NOT NULL   -- координаты склада
  dropoff_location    geometry(Point, 4326) NOT NULL   -- координаты зала
  status              varchar(20) NOT NULL    -- Assigned | PickedUp | InTransit | Delivered | Failed
  estimated_arrival_at timestamptz NULL
  started_at          timestamptz NULL
  completed_at        timestamptz NULL

-- История позиций курьеров (аналитика / аудит)
courier_location_history
  id           uuid PK
  courier_id   uuid FK → couriers(id)
  location     geometry(Point, 4326) NOT NULL
  recorded_at  timestamptz NOT NULL

CREATE INDEX idx_location_history_courier ON courier_location_history (courier_id, recorded_at DESC);
CREATE INDEX idx_couriers_location ON couriers USING GIST (current_location);
CREATE INDEX idx_products_search ON products USING GIN (search_vector);
CREATE INDEX idx_products_category ON products (category_id, status);

-- Outbox для синхронизации с Meilisearch и надёжной публикации событий
outbox_messages
  id           uuid PK
  type         varchar(100) NOT NULL
  payload      jsonb NOT NULL
  created_at   timestamptz NOT NULL
  processed_at timestamptz NULL

-- Расширение существующей таблицы залов
-- ALTER TABLE gyms ADD COLUMN location geometry(Point, 4326);
-- ALTER TABLE gyms ADD COLUMN address text;
```

### BankManager БД (отдельная схема/БД)

```sql
-- Счета
bank_accounts
  id            uuid PK
  owner_id      uuid NOT NULL              -- userId из основного приложения
  owner_type    varchar(20) NOT NULL       -- GymAdmin | CmsAdmin | Marketplace
  balance       numeric(18,2) NOT NULL DEFAULT 0
  held_balance  numeric(18,2) NOT NULL DEFAULT 0
  currency      char(3) NOT NULL DEFAULT 'RUB'
  created_at    timestamptz NOT NULL

  CONSTRAINT chk_balance_non_negative CHECK (balance >= 0)
  CONSTRAINT chk_held_non_negative    CHECK (held_balance >= 0)

-- Транзакции (неизменяемый лог)
transactions
  id               uuid PK
  from_account_id  uuid FK → bank_accounts(id) NULL
  to_account_id    uuid FK → bank_accounts(id) NULL
  amount           numeric(18,2) NOT NULL
  currency         char(3) NOT NULL
  type             varchar(20) NOT NULL    -- Deposit | Hold | Capture | Release | Refund | Commission
  status           varchar(20) NOT NULL    -- Pending | Completed | Failed
  idempotency_key  varchar(100) NOT NULL UNIQUE
  created_at       timestamptz NOT NULL

-- Сессии оплаты (связывают заказ с холдом)
payment_sessions
  id               uuid PK
  order_id         uuid NOT NULL UNIQUE
  buyer_account_id uuid FK → bank_accounts(id)
  amount           numeric(18,2) NOT NULL
  currency         char(3) NOT NULL
  status           varchar(20) NOT NULL    -- Pending | Held | Captured | Released | Expired
  hold_transaction_id uuid NULL FK → transactions(id)
  expires_at       timestamptz NOT NULL
  created_at       timestamptz NOT NULL
```

---

## Вопросы — нужны ответы перед продолжением

**Q1: Поисковый движок**
В особенностях написано OpenSearch/Elasticsearch, в README обсуждается Meilisearch. Что выбираем? Meilisearch проще поднять и интегрировать, Elasticsearch/OpenSearch — более «боевой» опыт. Или всё-таки ограничиться PostgreSQL FTS на старте?

> A1: _[ответ]_

---

**Q2: BankManager — отдельный процесс или модуль**
README говорит «отдельный процесс/контейнер». Это значит отдельный `docker-compose` сервис с отдельной БД? Или можно сделать как изолированный модуль внутри монолита с отдельной схемой БД, чтобы не усложнять инфраструктуру на старте?

> A2: _[ответ]_

---

**Q3: Протокол общения с BankManager**
В README написано «асинхронно RabbitMQ + некоторые вызовы по gRPC». Какие операции синхронные (gRPC/REST), а какие асинхронные (RabbitMQ)? Например, Hold — это блокирующий вызов (ждём ответа перед созданием заказа) или асинхронный (создали заказ, ждём события)?

> A3: _[ответ]_

---

**Q4: Маршруты курьеров**
Прямолинейная интерполяция (просто: А → Б по прямой) или OSRM (реальные дороги, Docker-контейнер)? OSRM даёт красивую картинку на карте, но это дополнительная инфраструктура.

> A4: _[ответ]_

---

**Q5: Карта на фронтенде**
Какая библиотека для карты трекинга курьера? Leaflet.js (бесплатно, OpenStreetMap), Яндекс Карты (удобно для РФ-адресов, нужен ключ), Google Maps (нужен ключ + billing)?

> A5: _[ответ]_

---

**Q6: Наполнение каталога**
В особенностях написано «возможно наполнение каталога будет машинным». Что имеется в виду — сид-скрипт при запуске (faker + заранее подготовленные данные) или реально ИИ-генерация описаний товаров через API?

> A6: _[ответ]_

---

**Q7: Комиссия маркетплейса**
В README упомянут счёт `Marketplace` в BankManager. Нужна ли реально работающая комиссия (например, 5% с каждой сделки идут на счёт Marketplace)? Или это опциональный бонус?

> A7: _[ответ]_

---

**Q8: Переназначение курьера**
Если курьер отказался от заказа в пути — система должна автоматически найти нового или это ручное действие CmsAdmin? У нового курьера другая точка старта — он едет не со склада, а от текущей позиции.

> A8: _[ответ]_

---

**Q9: Уведомления о смене статуса**
SignalR push — понятно. Нужны ли ещё email-уведомления при ключевых событиях (заказ принят, заказ доставлен, заказ отменён)? В FitHub уже есть `EmailNotifications` модуль.

> A9: _[ответ]_

---

**Q10: Склад**
Откуда берутся координаты склада (точка отправки курьера)? Хардкод в `appsettings`? Или отдельная сущность `Warehouse`, которую `CmsAdmin` настраивает через UI?

> A10: _[ответ]_
