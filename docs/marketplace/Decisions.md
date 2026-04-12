# Marketplace Stage 0 Decisions

Этот файл закрывает Этап 0 из [TODO.md](TODO.md): решения до кода. Решения можно пересмотреть позже, но следующий этап разработки должен стартовать именно с этих допущений.

## Backend

### Product and delivery assumptions

MVP работает с физическими товарами.

- Цифровые товары, подписки, аренда и услуги не входят в marketplace MVP.
- Склад один: склад сети/продавца, откуда курьер забирает заказ.
- Адрес доставки берется из существующей сущности зала Gym и на первом этапе хранится в delivery read model строкой плюс координаты, если они доступны.
- Нормализованную адресную модель можно добавить позже, если понадобится расчет маршрутов, валидация адресов или несколько точек доставки.
- Симуляция курьеров строится через интерполяцию маршрута между pickup и dropoff точками; реальный GPS/мобильное приложение не входят в MVP.

### Bounded context

Marketplace реализуется внутри `PlatformService`, а не как отдельный сервис.

Код раскладывается отдельным вертикальным срезом:

- `backend/Platform/src/Domain/Marketplace`
- `backend/Platform/src/Application/Marketplace`
- `backend/Platform/src/Data/Marketplace`
- `backend/Platform/src/Contracts/V1/Marketplace`
- `backend/Platform/src/Host` или `backend/Platform/src/Web` для HTTP endpoints по текущему паттерну проекта

Причина: каталог, остатки, checkout, order и delivery сильно завязаны на существующие роли, gyms, файлы, SignalR, JWT и инфраструктуру Platform. Для demo SaaS это проще и быстрее, чем разносить каталог/заказы/доставку в отдельные сервисы.

### BankManager

BankManager остается отдельным backend-процессом/проектом в solution, а не модулем внутри Platform.

Граница ответственности:

- Platform владеет каталогом, остатками, резервациями, заказами и доставкой.
- BankManager владеет счетами, платежными намерениями, операциями, балансами, webhook/outbox событиями и финансовой идемпотентностью.
- Platform не списывает деньги напрямую и не хранит финансовую ledger-модель.

### Транспорт PlatformService <-> BankManager

Используется гибрид `HTTP + RabbitMQ`.

- HTTP: синхронные команды, где UI ждет понятный результат, например создать `PaymentIntent`, получить статус оплаты, запросить баланс для debug/admin сценария.
- RabbitMQ: асинхронные события состояния, например `PaymentSucceeded`, `PaymentFailed`, `PaymentExpired`, `PaymentRefunded`.
- Outbox обязателен на стороне BankManager для публикации событий в RabbitMQ.
- Inbox/idempotent handling обязателен на стороне Platform для обработки событий BankManager.

Для MVP можно начать с HTTP-команды создания оплаты и RabbitMQ-события результата. Полностью синхронный payment flow не фиксировать как целевую архитектуру.

### Базовый flow заказа

Целевой flow:

```text
Catalog product -> Product variant -> Checkout reservation -> Payment intent -> Payment result -> Order -> Delivery
```

Состояния уровня Platform:

```text
ReservationCreated
ReservationExpired
PaymentPending
PaymentSucceeded
PaymentFailed
OrderCreated
DeliveryPending
DeliveryInProgress
Delivered
Cancelled
```

Правила:

- Резервация создается до оплаты и держит остаток ограниченное время.
- Заказ создается только после подтвержденной успешной оплаты.
- Если оплата неуспешна, резерв освобождается или остается до истечения срока в зависимости от отдельного бизнес-правила checkout.
- Если резерв истек до результата оплаты, Platform не создает заказ и запускает компенсацию через BankManager.
- Доставка создается после `OrderCreated`, но до назначения курьера может находиться в `DeliveryPending`.

### Конкурентность остатков

Для MVP стратегия остатков - optimistic concurrency.

Практическая реализация:

- Для административного редактирования товара, варианта и остатка использовать `RowVersion`/`version` concurrency token.
- Для резервирования последней единицы использовать атомарное условное обновление остатка с проверкой доступного количества и уникальным `idempotencyKey` резервации. Это сохраняет optimistic-подход без долгих блокировок на чтение.
- При конфликте backend возвращает бизнес-ошибку "остаток изменился" или "товар закончился", а frontend предлагает обновить состояние товара.

Pessimistic locking для товарного остатка не является MVP-стратегией. Его можно вернуть точечно для горячих SKU, если optimistic retries станут проблемой.

### Контроль зависших доставок

PlatformService обязан контролировать каждую активную доставку и не оставлять ее в подвешенном состоянии, даже если тестовое courier-приложение или будущий courier browser перестал присылать сигналы.

Решение:

- Для каждой активной доставки хранить `LastCourierSignalAt`, `LastLocationAt`, `LastStateChangedAt`, `WatchdogCheckedAt` и причину последнего автоматического решения.
- Сигналами считаются координата, heartbeat, accept/reject, picked up, delivered, failed и любые courier status events.
- Background job/watchdog в Platform периодически проверяет активные доставки и переводит их в следующий безопасный статус.
- Первое авто-решение - degraded state и уведомление GymAdmin: "Мы заметили, что курьер временно не обновляет геопозицию. Мы проверяем доставку и обновим статус автоматически."
- Если связи нет дольше допустимого порога до pickup, Platform пытается переназначить курьера или переводит доставку в `Failed/Cancelled` с уведомлением.
- Если связи нет дольше допустимого порога после pickup, Platform эскалирует доставку в `LostContact`, уведомляет CmsAdmin и GymAdmin, затем принимает конечное решение по таймауту: `DeliveryFailed` + компенсация/возврат денег через BankManager или ручное вмешательство CmsAdmin, если товар уже физически у курьера.
- Автоматический refund/release/capture reversal всегда идет через BankManager и должен быть идемпотентным.
- Каждый auto-decision пишет `DeliveryEvent`, чтобы GymAdmin и CmsAdmin видели, что система не молчит, а контролирует ситуацию.

Точные пороги не фиксируются в Stage 0. Их нужно вынести в конфигурацию delivery module на этапе реализации.

### Идемпотентность

Идемпотентность обязательна для:

- `POST /api/v1/marketplace/checkout/reservations`
- команды старта оплаты по резервации
- обработки `PaymentSucceeded`/`PaymentFailed` от BankManager
- создания `Order` из успешной оплаты
- переходов статуса доставки, которые могут повториться из-за retry или reconnect

Правила:

- Idempotency key приходит от клиента или создается на backend для внутренних переходов.
- Повтор того же ключа возвращает уже созданный результат, а не выполняет операцию второй раз.
- Для событий BankManager хранить `eventId`/`messageId` в inbox-таблице Platform.
- Для создания заказа держать уникальность по `paymentIntentId` или `reservationId`, чтобы повторное событие оплаты не создало второй заказ.

## Frontend

### Роли

Целевые роли marketplace:

- `GymAdmin`: покупатель. Видит каталог, деталку товара, checkout, оплату, заказ и tracking доставки.
- `CmsAdmin`: продавец/оператор. Управляет каталогом, заказами, сборкой, доставками и может видеть диспетчерский экран.
- `Courier`: курьерский browser UI. Целевая роль для доставки, но ее еще нет в текущем `frontend/src/types/auth.ts`.

Решение для MVP до добавления auth-роли `Courier`: курьерский UI делать как dev-only/CmsAdmin-инструмент или симулятор. Отдельную роль `Courier` добавлять в этапе доставки/permissions, когда понадобится настоящий защищенный маршрут.

### Маршруты

Используем существующие базовые зоны:

- `GymAdmin`: `/gym-admin/marketplace`
- `CmsAdmin`: `/admin/marketplace`
- временный/dev courier UI: `/admin/marketplace/courier-simulator`
- целевой courier UI после добавления роли: `/courier/marketplace`

Минимальные маршруты GymAdmin:

- `/gym-admin/marketplace/catalog`
- `/gym-admin/marketplace/products/:productId`
- `/gym-admin/marketplace/checkout/:reservationId`
- `/gym-admin/marketplace/orders/:orderId`
- `/gym-admin/marketplace/orders/:orderId/tracking`

Минимальные маршруты CmsAdmin:

- `/admin/marketplace/products`
- `/admin/marketplace/products/:productId`
- `/admin/marketplace/orders`
- `/admin/marketplace/deliveries`
- `/admin/marketplace/deliveries/:deliveryId`

### Навигация

Marketplace появляется:

- в `GymAdmin` sidebar как "Marketplace" или "Магазин" с первым экраном каталога;
- в `CmsAdmin` sidebar как "Marketplace" или "Магазин" с управлением товарами и доставками;
- courier navigation не добавляется в основной sidebar до появления роли `Courier`.

Если marketplace скрывается feature flag/capability, frontend не должен считать это авторизацией. Backend endpoints все равно должны проверять роль/permission.

### Минимальные MVP экраны

GymAdmin:

- каталог;
- деталка товара;
- checkout с резервацией и оплатой;
- результат заказа;
- статус заказа;
- tracking доставки (с указанием где курьер и через сколько приедет)

CmsAdmin:

- список товаров;
- форма товара/вариантов;
- список заказов;
- список доставок;
- деталка доставки.

Courier/dev:

- экран принятия доставки;
- экран смены статуса доставки;
- экран отправки/симуляции координат

### Формат пользовательских ошибок

Frontend должен уметь различать минимум эти ошибки:

- `Marketplace.ProductNotFound`
- `Marketplace.VariantUnavailable`
- `Marketplace.StockChanged`
- `Marketplace.OutOfStock`
- `Marketplace.ReservationExpired`
- `Marketplace.PaymentDeclined`
- `Marketplace.PaymentPending`
- `Marketplace.OrderAlreadyCreated`
- `Marketplace.DeliveryNotAssigned`
- `Marketplace.DeliveryTrackingUnavailable`

Backend Response - как и сейчас на проекте - ProblemDetails (с возможностью детализированных ошибок по форме с FluentValidations и доп полем errors в ответе)

Frontend copy должен быть пользовательским и коротким, например:

- "Товар закончился. Обновите страницу товара."
- "Резерв истек. Создайте резерв заново."
- "Оплата отклонена. Попробуйте другой способ или повторите позже."
- "Курьер еще не назначен. Мы обновим статус автоматически."

## Контракты для Этапа 1

На первом этапе нужны только базовые контракты каталога и резервации. Полная оплата и доставка остаются заглушками до своих этапов.

Минимальный backend contract set:

- `MarketplaceProductListItem`
- `MarketplaceProductDetails`
- `MarketplaceProductVariant`
- `MarketplaceMoney`
- `MarketplaceStock`
- `MarketplaceFacet`
- `CreateCheckoutReservationRequest`
- `CheckoutReservationResponse`
- `MarketplaceErrorResponse`

Минимальный frontend contract set:

- `Product`
- `ProductVariant`
- `Money`
- `Stock`
- `Facet`
- `Reservation`
- `MarketplaceError`

Контракты должны использовать строковые id, как в существующих API с файлами.
