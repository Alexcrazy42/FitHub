# BankManager: оплата резервации перед созданием заказа

Документ фиксирует границу отдельного сервиса `BankManager` для маркетплейса FitHub. Это не реальный банк, а сервис-заглушка, который имитирует банковское поведение: принимает запрос на списание, асинхронно обрабатывает платёж и отправляет результат обратно в `PlatformService` через RabbitMQ.

Ключевое правило: `stock_reservation` не становится реальным заказом сразу. Реальный `Order` в `PlatformService` создаётся только после успешной оплаты, подтверждённой событием от `BankManager`.

## Роли сервисов

`PlatformService` остаётся владельцем каталога, остатков, резерваций и заказов:

- ищет товары и показывает каталог;
- создаёт `stock_reservation` с TTL;
- создаёт платёжную попытку для резервации;
- ждёт результат оплаты;
- создаёт `Order` только после `PaymentSucceeded`;
- освобождает резерв после `PaymentFailed`, `PaymentExpired` или истечения TTL;
- после доставки инициирует финальный capture, если используется двухфазная модель `Authorize -> Capture`.

`BankManager` владеет платёжной моделью:

- хранит счета покупателей и продавца;
- принимает команды на оплату;
- имитирует задержку, отказ, недостаток средств и успешную оплату;
- публикует платёжные события в RabbitMQ;
- хранит идемпотентный аудит всех операций;
- предоставляет dev/admin API для ручной симуляции банковских вебхуков.

## Сущности BankManager

### BankAccount

Счёт участника симуляции.

```text
BankAccount
  Id
  OwnerId
  OwnerType: GymAdmin | CmsAdmin | Marketplace | System
  Currency
  Balance
  HeldBalance
  Status: Active | Frozen | Closed
  CreatedAt
  UpdatedAt
```

Инварианты:

- `Balance >= 0`;
- `HeldBalance >= 0`;
- `HeldBalance <= Balance`;
- финансовые операции по счёту выполняются через блокировку строки (`SELECT FOR UPDATE`);
- на уровне БД нужен `CHECK` для неотрицательных значений.

### PaymentIntent

Намерение оплатить конкретную резервацию в `PlatformService`. Это основная сущность взаимодействия между платформой и банком.

```text
PaymentIntent
  Id
  PlatformReservationId
  PlatformOrderDraftId null
  BuyerAccountId
  SellerAccountId
  Amount
  Currency
  Status: Created | Processing | Authorized | Succeeded | Failed | Cancelled | Expired
  IdempotencyKey
  ExpiresAt
  CreatedAt
  UpdatedAt
```

`PlatformReservationId` связывает банковскую оплату с `stock_reservation`. На этом этапе реального заказа ещё нет, поэтому `OrderId` не должен быть обязательным внешним ключом в платёжной модели.

### PaymentOperation

Аудитная запись конкретного банковского действия.

```text
PaymentOperation
  Id
  PaymentIntentId
  Type: CreateIntent | Authorize | Capture | Release | Refund | Fail | Expire
  Status: Pending | Succeeded | Failed
  Amount
  Currency
  IdempotencyKey
  FailureCode null
  FailureMessage null
  CreatedAt
  CompletedAt null
```

Идемпотентность должна быть на уровне `(Type, IdempotencyKey)`: повтор той же команды возвращает уже записанный результат и не двигает деньги второй раз.

### BankWebhookEvent

Внутренняя имитация вебхука от банка. В реальной интеграции банк сам прислал бы HTTP webhook, но в пет-проекте `BankManager` сам генерирует событие и публикует его в RabbitMQ.

```text
BankWebhookEvent
  Id
  PaymentIntentId
  EventType: PaymentSucceeded | PaymentFailed | PaymentExpired | PaymentCancelled | CaptureSucceeded | CaptureFailed | RefundSucceeded
  PayloadJson
  Status: Pending | Published | PublishFailed
  OccurredAt
  PublishedAt null
```

Если `BankManager` хранит outbox, `BankWebhookEvent` может быть частью outbox-таблицы или отдельной доменной таблицей, из которой публикуется сообщение.

### BankOutboxMessage

Техническая таблица надёжной публикации событий в RabbitMQ.

```text
BankOutboxMessage
  Id
  Type
  PayloadJson
  CorrelationId
  IdempotencyKey
  Status: Pending | Published | Failed
  CreatedAt
  PublishedAt null
  RetryCount
  LastError null
```

Все события, которые должны уйти в `PlatformService`, пишутся в outbox в той же транзакции, что и изменение `PaymentIntent`/`PaymentOperation`.

## Сущности на стороне PlatformService

В `PlatformService` нужны не банковские счета, а проекция платёжного процесса для резервации.

```text
CheckoutReservation
  Id
  StockReservationId
  GymAdminId
  Amount
  Currency
  Status: Reserved | PaymentPending | Paid | PaymentFailed | Expired | Released
  ExpiresAt
  CreatedAt
  UpdatedAt
```

```text
MarketplacePayment
  Id
  CheckoutReservationId
  BankPaymentIntentId
  Status: Pending | Processing | Succeeded | Failed | Expired | Cancelled
  FailureCode null
  FailureMessage null
  IdempotencyKey
  CreatedAt
  UpdatedAt
```

```text
Order
  Id
  CheckoutReservationId
  PaymentId
  GymAdminId
  Status: Paid | Assembling | PickedUp | InTransit | Delivered | Cancelled | Refunded
  CreatedAt
  UpdatedAt
```

`Order` создаётся только когда `MarketplacePayment.Status = Succeeded`. До этого UI может показывать пользователю состояние резервации и оплаты, но не должен считать это полноценным заказом.

## UX оплаты на платформе

Для пользователя оплата должна выглядеть как обычная оплата картой на сайте, а не как техническая симуляция BankManager.

Frontend flow:

1. После резерва товара пользователь попадает на checkout-экран.
2. UI показывает состав заказа, итоговую сумму, TTL резерва и форму оплаты картой.
3. Пользователь вводит:
   - номер карты;
   - срок действия;
   - CVC/CVV;
   - имя держателя, если решим показывать поле;
   - email/телефон для чека, если это понадобится для сценария.
4. Frontend отправляет данные карты в `PlatformService` через endpoint checkout-оплаты.
5. UI переходит в состояние "обрабатываем оплату" и ждёт результат по polling endpoint или SignalR-событию.
6. После успеха UI открывает страницу созданного заказа.
7. После отказа UI остаётся на checkout, показывает причину и даёт повторить оплату, если резерв ещё жив.

Важное ограничение: это учебная симуляция, поэтому нельзя проектировать так, будто платформа реально хранит карточные данные. Для MVP допустимо принять карточные поля в request и сразу передать их в BankManager, но не сохранять PAN/CVV в БД и не писать их в логи. В `PlatformService` можно хранить только безопасный snapshot:

```text
CardPaymentSnapshot
  PaymentId
  CardLast4
  CardBrand: Visa | Mastercard | Mir | Unknown
  ExpMonth
  ExpYear
  CardholderName null
```

Для более реалистичной модели можно сделать псевдо-токенизацию:

```text
POST /api/v1/marketplace/checkout/reservations/{id}/payment-methods
  -> BankManager возвращает paymentMethodToken

POST /api/v1/marketplace/checkout/reservations/{id}/pay
  -> PlatformService отправляет PaymentIntentRequested с paymentMethodToken
```

Для первого релиза проще один endpoint:

```text
POST /api/v1/marketplace/checkout/reservations/{id}/pay
```

Request:

```json
{
  "idempotencyKey": "reservation-019ad1a0-0000-7000-9000-000000000020-pay",
  "card": {
    "number": "4111111111111111",
    "expiryMonth": 12,
    "expiryYear": 2030,
    "cvv": "123",
    "cardholderName": "Alex Customer"
  }
}
```

`PlatformService` валидирует только базовую форму: номер похож на карту, срок не истёк, CVV нужной длины. Бизнес-решение "принять или отклонить платёж" принимает `BankManager`.

## События RabbitMQ

### PlatformService -> BankManager

`PaymentIntentRequested`

```json
{
  "eventId": "019ad1a0-0000-7000-9000-000000000001",
  "correlationId": "checkout-019ad1a0-0000-7000-9000-000000000010",
  "idempotencyKey": "reservation-019ad1a0-0000-7000-9000-000000000020-pay",
  "reservationId": "019ad1a0-0000-7000-9000-000000000020",
  "buyerAccountId": "019ad1a0-0000-7000-9000-000000000030",
  "sellerAccountId": "019ad1a0-0000-7000-9000-000000000040",
  "paymentMethod": {
    "type": "Card",
    "token": "pm_019ad1a0_0000_7000_9000_000000000070",
    "cardLast4": "1111",
    "cardBrand": "Visa"
  },
  "amount": 149.00,
  "currency": "USD",
  "expiresAt": "2026-04-12T12:20:00Z"
}
```

`PaymentCancelRequested`

```json
{
  "eventId": "019ad1a0-0000-7000-9000-000000000002",
  "correlationId": "checkout-019ad1a0-0000-7000-9000-000000000010",
  "idempotencyKey": "reservation-019ad1a0-0000-7000-9000-000000000020-cancel",
  "paymentIntentId": "019ad1a0-0000-7000-9000-000000000050",
  "reason": "ReservationExpired"
}
```

`PaymentCaptureRequested` нужен только если выбран двухфазный flow `Authorize -> Capture`, где деньги холдируются при оплате и переводятся продавцу после доставки.

```json
{
  "eventId": "019ad1a0-0000-7000-9000-000000000003",
  "correlationId": "order-019ad1a0-0000-7000-9000-000000000060",
  "idempotencyKey": "order-019ad1a0-0000-7000-9000-000000000060-capture",
  "paymentIntentId": "019ad1a0-0000-7000-9000-000000000050",
  "orderId": "019ad1a0-0000-7000-9000-000000000060"
}
```

### BankManager -> PlatformService

`PaymentSucceeded`

```json
{
  "eventId": "019ad1a0-0000-7000-9000-000000000101",
  "correlationId": "checkout-019ad1a0-0000-7000-9000-000000000010",
  "paymentIntentId": "019ad1a0-0000-7000-9000-000000000050",
  "reservationId": "019ad1a0-0000-7000-9000-000000000020",
  "amount": 149.00,
  "currency": "USD",
  "occurredAt": "2026-04-12T12:05:10Z"
}
```

`PaymentFailed`

```json
{
  "eventId": "019ad1a0-0000-7000-9000-000000000102",
  "correlationId": "checkout-019ad1a0-0000-7000-9000-000000000010",
  "paymentIntentId": "019ad1a0-0000-7000-9000-000000000050",
  "reservationId": "019ad1a0-0000-7000-9000-000000000020",
  "failureCode": "InsufficientFunds",
  "failureMessage": "Not enough available balance",
  "occurredAt": "2026-04-12T12:05:10Z"
}
```

`PaymentExpired`, `PaymentCancelled`, `CaptureSucceeded`, `CaptureFailed` и `RefundSucceeded` имеют тот же envelope: `eventId`, `correlationId`, `paymentIntentId`, `reservationId` или `orderId`, `occurredAt`, payload с деталями результата.

## Основной flow

### 1. Поиск и резерв товара

1. `GymAdmin` выбирает конкретный `productVariantId`.
2. `PlatformService` создаёт `stock_reservation` атомарным update остатков.
3. Платформа создаёт `CheckoutReservation` в статусе `Reserved`.
4. Frontend получает `reservationId` и `expiresAt`.

На этом шаге заказа ещё нет.

### 2. Запуск оплаты

1. `GymAdmin` вводит данные карты на checkout-экране и нажимает "Оплатить".
2. Frontend отправляет карточные данные в `PlatformService`.
3. `PlatformService` проверяет, что `CheckoutReservation` ещё активна, сумма не изменилась, TTL не истёк.
4. `PlatformService` формирует безопасный snapshot карты (`last4`, brand, expiry) и не сохраняет CVV/PAN.
5. `PlatformService` переводит `CheckoutReservation` в `PaymentPending`.
6. `PlatformService` публикует `PaymentIntentRequested` в RabbitMQ через свой outbox. В событие уходит `paymentMethodToken` или безопасный card snapshot, в зависимости от выбранного уровня симуляции.
7. `BankManager` принимает событие, создаёт `PaymentIntent` в статусе `Processing`.
8. `BankManager` имитирует задержку и результат:
   - успех;
   - недостаточно средств;
   - неверная карта;
   - истёкшая карта;
   - 3DS/подтверждение банка не пройдено, если решим добавить такой сценарий;
   - случайный отказ;
   - timeout/expired.

Для UX это всё равно один обычный checkout: пользователь не видит RabbitMQ, outbox, PaymentIntent и банковские события. Он видит состояния "проверяем карту", "оплата прошла", "оплата отклонена".

### 3. Успешная оплата

1. `BankManager` блокирует счёт покупателя.
2. Если средств достаточно, выполняет один из вариантов:
   - для MVP: сразу списывает покупателя и зачисляет продавцу;
   - для более реалистичного flow: увеличивает `HeldBalance` и ставит `PaymentIntent.Status = Authorized/Succeeded`.
3. `BankManager` пишет `PaymentSucceeded` в outbox.
4. `PlatformService` получает `PaymentSucceeded`.
5. `PlatformService` в одной транзакции:
   - проверяет, что `stock_reservation` ещё active и не истёк;
   - переводит `MarketplacePayment` в `Succeeded`;
   - переводит `CheckoutReservation` в `Paid`;
   - commit'ит `stock_reservation` в sold/reserved committed;
   - создаёт реальный `Order` в статусе `Paid`.
6. Дальше заказ идёт в сборку и доставку.

### 4. Неуспешная оплата

1. `BankManager` публикует `PaymentFailed` или `PaymentExpired`.
2. `PlatformService` получает событие.
3. `PlatformService` в одной транзакции:
   - переводит `MarketplacePayment` в `Failed` или `Expired`;
   - переводит `CheckoutReservation` в `PaymentFailed` или `Expired`;
   - release'ит `stock_reservation`;
   - не создаёт `Order`.
4. Frontend показывает, что оплата не прошла, и предлагает повторить покупку при наличии товара.

### 5. Истечение резерва раньше ответа банка

Если TTL резерва истёк раньше, чем пришёл успешный ответ от банка:

1. `PlatformService` release'ит `stock_reservation`.
2. `PlatformService` публикует `PaymentCancelRequested`.
3. Если потом приходит поздний `PaymentSucceeded`, обработчик не создаёт `Order`, а переводит платёж в конфликтное состояние `SucceededAfterReservationExpired` или инициирует refund.
4. Для MVP лучше избегать этого состоянием через одинаковый deadline: `PaymentIntent.ExpiresAt <= stock_reservation.ExpiresAt`.

## Состояния

### CheckoutReservation в PlatformService

```text
Reserved
  -> PaymentPending
  -> Paid
  -> PaymentFailed
  -> Expired
  -> Released
```

Разрешённые переходы:

- `Reserved -> PaymentPending`;
- `PaymentPending -> Paid`;
- `PaymentPending -> PaymentFailed`;
- `PaymentPending -> Expired`;
- `Reserved -> Expired`;
- `PaymentFailed -> Released`;
- `Expired -> Released`.

Запрещено:

- создавать `Order` из `Reserved`;
- создавать `Order` из `PaymentPending`;
- создавать `Order` из `PaymentFailed` или `Expired`;
- повторно release'ить уже committed/paid reservation без отдельного refund flow.

### PaymentIntent в BankManager

```text
Created
  -> Processing
  -> Succeeded
  -> Failed
  -> Cancelled
  -> Expired
```

Для двухфазной оплаты:

```text
Created
  -> Processing
  -> Authorized
  -> Captured
  -> Released
  -> Refunded
```

Для первого релиза можно выбрать простой flow `Processing -> Succeeded/Failed`, чтобы быстрее собрать работающий сценарий. Но доменную модель лучше назвать `PaymentIntent`, а не `OrderPayment`, потому что на момент оплаты заказа ещё нет.

## API BankManager

HTTP API удобно оставить для ручного тестирования и dev-сценариев, а основной production-like обмен делать через RabbitMQ.

```text
POST /api/bank/accounts
POST /api/bank/accounts/{id}/deposit
GET  /api/bank/accounts/{id}/balance
GET  /api/bank/accounts/{id}/transactions

POST /api/bank/payment-methods/card-token
POST /api/bank/payments/intents
POST /api/bank/payments/intents/{id}/authorize
POST /api/bank/payments/intents/{id}/capture
POST /api/bank/payments/intents/{id}/release
POST /api/bank/payments/intents/{id}/refund
GET  /api/bank/payments/intents/{id}

POST /api/bank/dev/payments/{id}/simulate-success
POST /api/bank/dev/payments/{id}/simulate-failure
POST /api/bank/dev/payments/{id}/simulate-expired
```

`dev`-эндпоинты должны быть доступны только в Development/Staging или за отдельной админской авторизацией.

Публичный frontend не должен ходить напрямую в `BankManager`. Он общается только с `PlatformService`, а платформа уже публикует команды в банк. Это сохраняет естественную границу: для пользователя есть обычная страница оплаты внутри FitHub, а `BankManager` остаётся внутренним эквайринг-симулятором.

## Надёжность и идемпотентность

Обязательные правила:

- все входящие команды имеют `idempotencyKey`;
- все входящие события имеют `eventId`;
- `PlatformService` хранит inbox обработанных банковских событий;
- `BankManager` хранит inbox обработанных команд от платформы;
- оба сервиса публикуют события через outbox;
- повторное `PaymentSucceeded` не создаёт второй `Order`;
- повторное `PaymentFailed` не release'ит резерв второй раз;
- позднее событие не должно делать запрещённый переход состояния;
- `correlationId` протаскивается через весь flow для логов и трассировки.

Минимальные таблицы технической надёжности:

```text
InboxMessage
  MessageId
  ConsumerName
  ProcessedAt
  PayloadHash

OutboxMessage
  Id
  Type
  PayloadJson
  CorrelationId
  Status
  CreatedAt
  PublishedAt null
  RetryCount
```

## Что тестировать

- успешный flow: reserve -> payment pending -> `PaymentSucceeded` -> создан `Order`;
- failed flow: reserve -> payment pending -> `PaymentFailed` -> резерв освобождён, `Order` не создан;
- duplicate `PaymentSucceeded` не создаёт второй заказ;
- duplicate `PaymentIntentRequested` не создаёт второй платёж;
- истёкший `stock_reservation` не становится заказом даже при позднем `PaymentSucceeded`;
- недостаток средств не уводит баланс в минус;
- параллельные оплаты с одного счёта блокируются через `SELECT FOR UPDATE`;
- RabbitMQ consumer корректно десериализует контракт события;
- outbox публикует событие после коммита банковской транзакции;
- dev simulate endpoint вызывает тот же обработчик, что и реальный RabbitMQ/webhook flow.
- frontend checkout принимает карточные данные и показывает обычный платёжный сценарий;
- `PlatformService` не сохраняет полный номер карты и CVV;
- невалидная/истёкшая карта возвращает отказ оплаты без создания `Order`;
- повторная отправка формы оплаты с тем же `idempotencyKey` не создаёт второй `PaymentIntent`.
