# Marketplace Delivery: доставка и live-трекинг как в Яндекс Доставке

Документ описывает примерный дизайн доставки для marketplace FitHub. Доставка включается после того, как товар найден, зарезервирован, оплачен через `BankManager`, и на стороне `PlatformService` создан реальный `Order`.

Цель UX: для `GymAdmin` это должно выглядеть как привычная доставка на карте: заказ собирается, курьер назначен, курьер едет к складу, забрал заказ, едет к залу, ETA обновляется, точка курьера двигается в реальном времени.

Основа из `CourierSimulation.md`:

- доставка своя, не сторонний провайдер;
- у курьера браузерный интерфейс, не мобильное приложение;
- геопозиция курьера приходит в backend через HTTP polling;
- `GymAdmin` видит курьера на карте в реальном времени;
- финальное действие курьера - кнопка "Доставлено";
- курьер назначается автоматически системой;
- staging пока нет, поэтому нужен симулятор курьеров.

## Где доставка начинается в общем flow

Доставка не участвует в поиске, резерве и оплате. Она начинается только после успешной оплаты:

```text
Catalog search
  -> stock_reservation
  -> card payment
  -> PaymentSucceeded
  -> Order created with status Paid
  -> CmsAdmin starts assembly
  -> Delivery module starts
```

Рекомендуемый order lifecycle:

```text
Paid
  -> Assembling
  -> ReadyForPickup
  -> CourierAssigned
  -> CourierArrivingToPickup
  -> PickedUp
  -> InTransit
  -> Arriving
  -> Delivered
```

Ошибочные ветки:

```text
CourierAssigned -> CourierCancelled -> ReassigningCourier
PickedUp        -> DeliveryFailed
InTransit       -> DeliveryFailed
InTransit       -> LostContact
LostContact     -> InTransit
```

Для MVP можно сократить:

```text
Paid -> Assembling -> ReadyForPickup -> CourierAssigned -> PickedUp -> InTransit -> Delivered
```

## Роли и экраны

### GymAdmin: покупатель

Видит только свою доставку.

Экран "Мой заказ":

- статус заказа;
- статус доставки;
- карта с маршрутом и точкой курьера;
- ETA;
- адрес склада и адрес зала;
- имя/телефон курьера, если хотим симулировать связь;
- события таймлайна: "заказ собирается", "курьер назначен", "курьер забрал заказ", "курьер в пути", "доставлено".

Это "Яндекс Доставка view": пользователь не управляет процессом, он наблюдает за движением и статусами.

### CmsAdmin: диспетчер/продавец

Видит операционный экран.

Экран "Заказы":

- оплаченные заказы;
- заказы в сборке;
- заказы, ожидающие курьера;
- активные доставки;
- проблемные доставки.

Экран "Курьеры":

- список курьеров;
- статусы: `Available`, `Assigned`, `PickingUp`, `Delivering`, `Offline`;
- карта всех курьеров;
- текущий заказ курьера;
- last seen / last location time;
- ручное переназначение как опциональная админская функция.

### Courier: браузерный интерфейс

Курьер не является полноценным `User` системы в первой версии. Это отдельная сущность с выданным access token или одноразовой ссылкой.

Экран курьера:

- текущий заказ;
- адрес склада;
- адрес зала;
- кнопки:
  - "Принять заказ";
  - "Я у склада";
  - "Забрал заказ";
  - "Еду к залу";
  - "Доставлено";
  - "Проблема", опционально;
- browser geolocation permission;
- отправка координат в backend каждые N секунд.

Если браузер не дал геолокацию, courier UI показывает ошибку и не даёт стартовать доставку.

## Сущности

### Courier

```text
Courier
  Id
  Name
  Phone
  Status: Available | Assigned | PickingUp | Delivering | Offline | Suspended
  CurrentOrderId null
  CurrentDeliveryId null
  CurrentLocation geometry(Point, 4326) null
  LastLocationAt null
  CreatedAt
  UpdatedAt
```

Курьер - отдельная доменная сущность. Он не обязан быть `User`, потому что в MVP у него простой браузерный cockpit.

### Delivery

```text
Delivery
  Id
  OrderId
  CourierId null
  Status: PendingAssignment | Assigned | CourierToPickup | AtPickup | PickedUp | InTransit | Arriving | Delivered | Failed | Cancelled
  PickupLocation geometry(Point, 4326)
  DropoffLocation geometry(Point, 4326)
  PickupAddressSnapshot
  DropoffAddressSnapshot
  EstimatedPickupAt null
  EstimatedArrivalAt null
  AssignedAt null
  PickedUpAt null
  DeliveredAt null
  FailureReason null
  Version
  CreatedAt
  UpdatedAt
```

`PickupLocation` - координаты склада сети. `DropoffLocation` - координаты зала `GymAdmin`.

Для marketplace нужен snapshot адресов: если адрес зала изменится после создания заказа, старая доставка должна остаться понятной.

### DeliveryTrackingPoint

```text
DeliveryTrackingPoint
  Id
  DeliveryId
  CourierId
  Location geometry(Point, 4326)
  AccuracyMeters null
  SpeedMetersPerSecond null
  Heading null
  RecordedAt
  ReceivedAt
  Source: Browser | Simulator
```

Для hot path текущую позицию лучше хранить в Redis, а историю писать в PostgreSQL/PostGIS пачками или с downsampling.

Redis:

```text
delivery:{deliveryId}:location
  courierId
  lat
  lng
  accuracy
  recordedAt
  eta
```

PostgreSQL:

```text
delivery_tracking_point
  история для аудита, аналитики и воспроизведения маршрута
```

### DeliveryEvent

```text
DeliveryEvent
  Id
  DeliveryId
  Type: Assigned | CourierLocationUpdated | AtPickup | PickedUp | InTransit | Arriving | Delivered | Failed | CourierOffline | CourierReassigned
  PayloadJson
  OccurredAt
```

События нужны для таймлайна заказа и для SignalR push.

## Основной flow

### 1. Заказ оплачен

1. `BankManager` присылает `PaymentSucceeded`.
2. `PlatformService` создаёт `Order` в статусе `Paid`.
3. `CmsAdmin` видит новый заказ.
4. Доставка ещё не стартует: сначала заказ должен быть собран.

### 2. Сборка заказа

1. `CmsAdmin` нажимает "Начать сборку".
2. `Order.Status = Assembling`.
3. Когда товары собраны, `CmsAdmin` нажимает "Готов к выдаче курьеру".
4. `Order.Status = ReadyForPickup`.
5. `Delivery.Status = PendingAssignment`.

На этом шаге система может автоматически начать поиск курьера.

### 3. Автоматическое назначение курьера

Система ищет ближайшего свободного курьера к складу:

```sql
select *
from couriers
where status = 'Available'
order by current_location <-> :pickupLocation
limit 1
for update skip locked;
```

После выбора:

```text
Courier.Status = Assigned
Courier.CurrentDeliveryId = deliveryId
Delivery.CourierId = courierId
Delivery.Status = Assigned
Order.Status = CourierAssigned
```

Если свободных курьеров нет:

```text
Delivery.Status = PendingAssignment
Order.Status = ReadyForPickup
```

Или отдельный статус:

```text
WaitingForCourier
```

Для MVP можно запускать retry job каждые 10-30 секунд.

### 4. Курьер принимает заказ в браузере

Курьер открывает cockpit:

```text
GET /api/v1/courier/me/current-delivery
```

Нажимает "Принять заказ":

```text
POST /api/v1/courier/deliveries/{deliveryId}/accept
```

Backend:

```text
Delivery.Status = CourierToPickup
Courier.Status = PickingUp
Order.Status = CourierArrivingToPickup
```

Курьерский браузер начинает слать координаты:

```text
POST /api/v1/courier/location
```

Пример:

```json
{
  "deliveryId": "019ad1a0-0000-7000-9000-000000000060",
  "lat": 55.7558,
  "lng": 37.6173,
  "accuracyMeters": 12,
  "speedMetersPerSecond": 7.5,
  "heading": 120,
  "recordedAt": "2026-04-12T12:15:10Z"
}
```

Backend:

- валидирует, что курьер назначен на эту доставку;
- фильтрует аномальные координаты;
- обновляет текущую позицию в Redis;
- пишет историю в PostgreSQL/PostGIS;
- пересчитывает ETA;
- публикует `CourierLocationUpdated` через SignalR.

### 5. Курьер забирает заказ

Когда курьер приехал к складу, он нажимает:

```text
POST /api/v1/courier/deliveries/{deliveryId}/at-pickup
```

Потом:

```text
POST /api/v1/courier/deliveries/{deliveryId}/picked-up
```

Backend:

```text
Delivery.Status = PickedUp
Courier.Status = Delivering
Order.Status = PickedUp
```

После `PickedUp` покупатель видит основной маршрут "склад -> зал" и ETA до зала.

### 6. Курьер едет к залу

Каждые N секунд courier browser отправляет координаты через HTTP polling.

`PlatformService` отдаёт live updates покупателю:

```text
CourierTrackingHub
  group: delivery:{deliveryId}
  event: CourierLocationUpdated
```

Payload:

```json
{
  "deliveryId": "019ad1a0-0000-7000-9000-000000000060",
  "courierId": "019ad1a0-0000-7000-9000-000000000070",
  "lat": 55.7562,
  "lng": 37.6201,
  "accuracyMeters": 10,
  "eta": "2026-04-12T12:34:00Z",
  "status": "InTransit",
  "recordedAt": "2026-04-12T12:20:10Z"
}
```

Если расстояние до зала меньше порога, например 300 метров:

```text
Delivery.Status = Arriving
Order.Status = Arriving
```

Frontend показывает "Курьер рядом".

### 7. Доставка завершена

Курьер нажимает:

```text
POST /api/v1/courier/deliveries/{deliveryId}/delivered
```

Backend:

```text
Delivery.Status = Delivered
Order.Status = Delivered
Courier.Status = Available
Courier.CurrentDeliveryId = null
Delivery.DeliveredAt = now()
```

Если в BankManager выбран двухфазный flow `Authorize -> Capture`, то после `Delivered` платформа публикует:

```text
PaymentCaptureRequested
```

Если для MVP деньги списаны сразу при оплате, финансового действия после доставки нет.

## UX как в Яндекс Доставке

Что важно для ощущения "живой доставки":

- карта открывается прямо на странице заказа;
- курьерская точка двигается без перезагрузки страницы;
- есть линия маршрута от склада до зала;
- ETA обновляется при каждом значимом изменении;
- статусы человеческие, не технические;
- при потере связи показывается "Курьер временно не обновляет геопозицию";
- когда курьер близко, статус меняется на "Курьер рядом";
- после доставки карта заменяется на финальный статус и таймлайн.

Пример текстов для `GymAdmin`:

```text
Заказ собирается
Ищем курьера
Курьер назначен
Курьер едет на склад
Курьер забрал заказ
Курьер в пути
Курьер рядом
Заказ доставлен
```

Не показывать пользователю:

```text
PendingAssignment
CourierToPickup
DeliveryTrackingPoint
RabbitMQ
Redis
PostGIS
```

## ETA

Для MVP ETA можно считать просто:

```text
remainingDistanceMeters / assumedSpeedMetersPerSecond
```

Где скорость:

```text
если есть speed из browser geolocation -> использовать её, но ограничить min/max
иначе использовать дефолт, например 25-35 км/ч
```

Более реалистичный вариант:

- построить маршрут через локальный OSRM;
- хранить polyline маршрута;
- считать оставшееся расстояние по polyline, а не прямую линию;
- обновлять ETA по текущей точке на маршруте.

Для ощущения "Яндекс Доставки" лучше хотя бы использовать маршрутную линию. Если OSRM пока не поднимать, можно начать с прямой линии и заменить реализацию за интерфейсом:

```csharp
public interface IDeliveryEtaService
{
    Task<DeliveryEta> CalculateAsync(Delivery delivery, GeoPoint courierLocation, CancellationToken ct);
}
```

## Карта и маршруты

Frontend может использовать:

- Yandex Maps, если хочется визуально приблизиться к Яндекс-подобному UX;
- Leaflet/OpenStreetMap, если нужен простой open-source вариант;
- любой текущий map stack проекта, если он уже есть.

Backend не должен зависеть от конкретной карты. Он отдаёт:

```text
pickup point
dropoff point
courier point
route polyline, если есть
eta
status
```

Frontend сам рисует:

- маркер склада;
- маркер зала;
- маркер курьера;
- линию маршрута;
- карточку статуса.

## API набросок

### CmsAdmin

```text
GET  /api/v1/marketplace/admin/deliveries
GET  /api/v1/marketplace/admin/deliveries/{id}
POST /api/v1/marketplace/admin/orders/{orderId}/start-assembly
POST /api/v1/marketplace/admin/orders/{orderId}/ready-for-pickup
POST /api/v1/marketplace/admin/deliveries/{id}/assign-courier
POST /api/v1/marketplace/admin/deliveries/{id}/reassign-courier

GET  /api/v1/marketplace/admin/couriers
POST /api/v1/marketplace/admin/couriers
PATCH /api/v1/marketplace/admin/couriers/{id}
```

### GymAdmin

```text
GET /api/v1/marketplace/orders/{orderId}
GET /api/v1/marketplace/orders/{orderId}/delivery
GET /api/v1/marketplace/deliveries/{deliveryId}/tracking-snapshot
```

`tracking-snapshot` нужен для первого открытия страницы: карта сразу получает текущую позицию, route, ETA и статус. Потом live updates приходят через SignalR.

### Courier browser

```text
GET  /api/v1/courier/me/current-delivery
POST /api/v1/courier/deliveries/{deliveryId}/accept
POST /api/v1/courier/deliveries/{deliveryId}/at-pickup
POST /api/v1/courier/deliveries/{deliveryId}/picked-up
POST /api/v1/courier/deliveries/{deliveryId}/delivered
POST /api/v1/courier/deliveries/{deliveryId}/problem
POST /api/v1/courier/location
```

## SignalR

Хаб:

```text
CourierTrackingHub
```

Группы:

```text
delivery:{deliveryId}
admin:marketplace:deliveries
```

Правила подписки:

- `GymAdmin` может подписаться только на delivery своего заказа;
- `CmsAdmin` может подписаться на все активные доставки;
- courier browser не обязан слушать SignalR в MVP, он может только отправлять HTTP polling.

События:

```text
DeliveryStatusChanged
CourierLocationUpdated
CourierAssigned
CourierOffline
EtaUpdated
DeliveryDelivered
DeliveryFailed
```

## Потеря связи и аномалии GPS

PlatformService обязан контролировать каждую доставку до финального статуса. Доставка не может зависнуть только потому, что тестовое courier-приложение, симулятор или будущий courier browser перестал присылать координаты/heartbeat.

Если `LastLocationAt` старше заданного порога:

```text
30-60 секунд -> degraded UI: "геопозиция временно не обновляется"
2-5 минут   -> Delivery.Status = LostContact или Courier.Status = Offline
```

Минимальная модель контроля:

- у `Delivery` хранить `LastCourierSignalAt`, `LastLocationAt`, `LastStateChangedAt`, `WatchdogCheckedAt`, `AutoDecisionReason`;
- сигналом считать location update, heartbeat, accept/reject, picked up, delivered, failed и другие courier events;
- `DeliveryWatchdogJob` периодически проверяет активные доставки;
- при первом пропуске сигнала писать `DeliveryEvent: CourierOffline` и отправлять `GymAdmin` уведомление: "Мы заметили, что курьер временно не обновляет геопозицию. Мы проверяем доставку и обновим статус автоматически.";
- до pickup: пытаться переназначить курьера, если текущий курьер молчит дольше порога;
- после pickup: переводить в `LostContact`, уведомлять `CmsAdmin` и `GymAdmin`, затем по финальному таймауту принимать решение `DeliveryFailed` или `ManualReviewRequired`;
- если доставка признана проваленной и товар не доставлен, запускать компенсацию через BankManager: release/refund/reversal по текущему состоянию оплаты;
- каждое автоматическое решение должно быть идемпотентным и писать `DeliveryEvent`, чтобы повторный запуск watchdog не делал возврат денег дважды.

Пример auto-decision flow:

```text
InTransit + no courier signal
  -> LostContact
  -> notify GymAdmin: "Мы заметили..."
  -> notify CmsAdmin
  -> wait configured timeout
  -> DeliveryFailed
  -> BankManager refund/reversal
  -> notify GymAdmin: "Доставка не завершилась, деньги возвращены"
```

Аномалии, которые стоит фильтровать:

- координата с невозможным скачком, например 5 км за 5 секунд;
- accuracy слишком плохой, например больше 500 метров;
- координата далеко от ожидаемого города/зоны доставки;
- recordedAt сильно в прошлом или будущем.

Для MVP плохие точки можно не сохранять как текущую позицию, но писать в лог диагностики.

## Связь с CourierSimulation

`CourierSimulation.md` описывает тестовый слой. Delivery module должен быть устроен так, чтобы симулятор и реальный courier browser шли через один и тот же backend flow:

```text
Реальный курьер:
  browser geolocation
  -> POST /api/v1/courier/location
  -> DeliveryLocationService
  -> Redis/PostGIS
  -> SignalR
  -> GymAdmin map

Симулятор:
  generated route points
  -> POST /api/v1/courier/location
  -> DeliveryLocationService
  -> Redis/PostGIS
  -> SignalR
  -> GymAdmin map
```

Симулятор не должен обходить обработчики доставки. Иначе тестируется не production-like flow.

Сценарии для симулятора:

- happy path: assigned -> picked up -> in transit -> delivered;
- курьер пропал на 2 минуты;
- курьер стоит на месте, ETA растёт;
- курьер отменил до pickup;
- GPS jitter;
- 20-50 активных доставок одновременно.

## MVP scope

Минимальный срез:

- `Courier` CRUD для `CmsAdmin`;
- координаты склада в конфиге или отдельной `Warehouse` сущности;
- координаты зала в `Gym`;
- `Delivery` создаётся после `ReadyForPickup`;
- автоматическое назначение ближайшего `Available` курьера;
- `DeliveryWatchdogJob`, который не оставляет доставку в подвешенном состоянии при отсутствии сигналов от курьера;
- courier browser с кнопками и browser geolocation;
- HTTP endpoint для координат курьера;
- Redis current location;
- PostgreSQL история location points;
- SignalR updates для `GymAdmin`;
- tracking page с картой, ETA и статусами;
- `CourierSimulatorService` для локального QA.

Что можно отложить:

- ручное переназначение курьера;
- фото доставки;
- код подтверждения;
- сложную поддержку проблемных доставок;
- настоящую мобильную app;
- OSRM, если для первого UI достаточно прямой линии;
- компенсации и поддержку.

## Тесты

- `GymAdmin` не может подписаться на чужую доставку;
- `CmsAdmin` видит все активные доставки;
- ближайший свободный курьер назначается атомарно через `FOR UPDATE SKIP LOCKED`;
- два заказа не получают одного курьера;
- координата курьера обновляет Redis snapshot;
- координата курьера публикует SignalR event;
- плохая GPS-точка не ломает delivery state;
- `delivered` переводит `Order` и `Delivery` в финальные статусы;
- повторный `delivered` идемпотентен;
- симулятор проходит happy path через те же endpoints, что и реальный courier browser.
