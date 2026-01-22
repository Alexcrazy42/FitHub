# grpc

Google Remote Procedure Call

Ключевая идея: вместо того, чтобы думать о HTTP запросах, endpoints, JSON-сериализации - просто вызов функции

```csharp
var response = await httpClient.PostAsync("/api/users", jsonContent);
var user = JsonSerializer.Deserialize<User>(await response.Content.ReadAsStringAsync());
```

```csharp
// gRPC - вызов как обычной функции!
var user = await grpcClient.GetUserAsync(new GetUserRequest { Id = 123 });
```

## Технологический стек

### HTTP/2 в качестве транспорта

gRPC требует HTTP/2 и использует его преимущества:
Multiplexing (мультиплексирование) — множество запросов в одном TCP-соединении без блокировки
- В REST/HTTP/1.1: 1 соединение = 1 запрос в момент времени
- В gRPC/HTTP/2: 1 соединение = N параллельных запросов

Header compression — сжатие заголовков (HPACK), экономия трафика при повторяющихся заголовках

Server Push — сервер может отправлять данные без явного запроса клиента

Binary protocol — бинарный протокол вместо текстового (меньше парсинга, быстрее)

Bidirectional streaming — двунаправленная потоковая передача данных

### Protocol Buffers (Protobuf) для сериализации

Бинарный формат вместо JSON:

JSON: {"name":"John","age":30} → ~23 байта текста

Protobuf: то же самое → ~7 байт бинарных данных

Строгая типизация — схема данных описывается заранее в .proto файлах

Автогенерация кода — из .proto автоматически генерируются классы для C#, Java, Python, Go и др.

Обратная совместимость — можно добавлять новые поля без поломки старых клиентов

### Сгенерированный клиент и сервер

Из одного .proto файла генерируется:
- Серверная заглушка (server stub) — интерфейс который ты реализуешь
- Клиентская заглушка (client stub) — готовый клиент для вызовов


## 4 типа взаимодействия в gRPC

1. Unary (простой запрос-ответ)

Классический запрос-ответ как в REST
Используется для простых CRUD операций, точечных запросов

```protobuf
service UserService {
    rpc GetUser(GetUserRequest) returns (UserResponse);
}
```

```csharp
// Клиент
var response = await client.GetUserAsync(new GetUserRequest { Id = 123 });

// Сервер
public override async Task<UserResponse> GetUser(GetUserRequest request, ServerCallContext context)
{
    var user = await _db.Users.FindAsync(request.Id);
    return new UserResponse { Name = user.Name, Email = user.Email };
}
```

2. Server Streaming (стриминг с сервера)

Клиент отправляет один запрос, сервер возвращает поток данных:
Используем для: Real-Time обновления (цены, котировки, статусы); постраничная загрузка больших данных; Live логи, метрики

```protobuf
service HotelService {
    rpc GetHotelPrices(HotelRequest) returns (stream PriceUpdate);
}
```

```csharp
// Клиент
var call = client.GetHotelPrices(new HotelRequest { HotelId = "hotel-1" });

await foreach (var priceUpdate in call.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"New price: {priceUpdate.Amount}");
}

// Сервер
public override async Task GetHotelPrices(HotelRequest request, IServerStreamWriter<PriceUpdate> responseStream, ServerCallContext context)
{
    while (!context.CancellationToken.IsCancellationRequested)
    {
        var price = await GetCurrentPrice(request.HotelId);
        await responseStream.WriteAsync(new PriceUpdate { Amount = price });
        await Task.Delay(1000);
    }
}
```

3. Client Streaming (стриминг с клиента)

Клиент отправляет поток запросов, сервер возвращает один ответ

Когда использовать:
Bulk операции (массовая загрузка данных)
Агрегация данных с клиента
Телеметрия, метрики

```protobuf
service OrderService {
    rpc BulkCreateOrders(stream CreateOrderRequest) returns (BulkOrderResponse);
}
```

```csharp
// Клиент
var call = client.BulkCreateOrders();

foreach (var order in orders)
{
    await call.RequestStream.WriteAsync(new CreateOrderRequest { Order = order });
}

await call.RequestStream.CompleteAsync();
var response = await call;

// Сервер
public override async Task<BulkOrderResponse> BulkCreateOrders(IAsyncStreamReader<CreateOrderRequest> requestStream, ServerCallContext context)
{
    int count = 0;
    
    await foreach (var request in requestStream.ReadAllAsync())
    {
        await _db.Orders.AddAsync(request.Order);
        count++;
    }
    
    await _db.SaveChangesAsync();
    return new BulkOrderResponse { ProcessedCount = count };
}
```

4. Bidirectional Streaming (двунаправленный стриминг)

Клиент и сервер одновременно отправляют друг другу потоки данных: 

Когда использовать:
Чаты, live collaboration
Игровые сервера
IoT (двусторонний обмен данных с устройствами)

```protobuf
service ChatService {
    rpc Chat(stream ChatMessage) returns (stream ChatMessage);
}
```

```csharp
// Клиент
var call = client.Chat();

// Читаем ответы в фоне
var readTask = Task.Run(async () =>
{
    await foreach (var message in call.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"Received: {message.Text}");
    }
});

// Отправляем сообщения
await call.RequestStream.WriteAsync(new ChatMessage { Text = "Hello" });
await call.RequestStream.WriteAsync(new ChatMessage { Text = "How are you?" });
await call.RequestStream.CompleteAsync();

await readTask;

// Сервер
public override async Task Chat(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
{
    await foreach (var message in requestStream.ReadAllAsync())
    {
        // Echo back
        await responseStream.WriteAsync(new ChatMessage { Text = $"Echo: {message.Text}" });
    }
}
```

## gRPC vs REST: когда что использовать

| Критерий           | gRPC                                    | REST                      |
| ------------------ | --------------------------------------- | ------------------------- |
| Производительность | 🔥 В 5-10 раз быстрее (бинарный формат) | Медленнее (JSON парсинг)  |
| Размер данных      | 🔥 Меньше в 3-5 раз                     | Больше (текстовый формат) |
| Строгая типизация  | 🔥 Да (protobuf схема)                  | Нет (любой JSON)          |
| Streaming          | 🔥 Нативная поддержка 4 типов           | Сложно (SSE, WebSocket)   |
| Browser support    | ⚠️ Нужен gRPC-Web (ограничения)         | ✅ Нативная поддержка      |
| Human-readable     | ❌ Бинарный формат                       | ✅ Текст, легко дебажить   |
| Кэширование        | ❌ Сложнее (POST запросы)                | ✅ HTTP кэши работают      |
| Firewall/Proxy     | ⚠️ Могут блокировать HTTP/2             | ✅ Везде работает          |


## Обработка ошибок в gRPC

Status Codes
gRPC использует стандартизированные коды ошибок:

| Code               | Значение          | HTTP эквивалент | Когда использовать    |
| ------------------ | ----------------- | --------------- | --------------------- |
| OK                 | Успех             | 200             | Все ок                |
| CANCELLED          | Отменено          | 499             | Клиент отменил запрос |
| INVALID_ARGUMENT   | Невалидные данные | 400             | Плохой input          |
| NOT_FOUND          | Не найдено        | 404             | Ресурс не существует  |
| ALREADY_EXISTS     | Уже существует    | 409             | Дубликат              |
| PERMISSION_DENIED  | Нет прав          | 403             | Недостаточно прав     |
| UNAUTHENTICATED    | Не авторизован    | 401             | Нужна авторизация     |
| RESOURCE_EXHAUSTED | Лимит превышен    | 429             | Rate limit            |
| UNAVAILABLE        | Недоступно        | 503             | Сервис упал           |
| INTERNAL           | Внутренняя ошибка | 500             | Что-то сломалось      |
| DEADLINE_EXCEEDED  | Таймаут           | 504             | Запрос слишком долгий |

Примеры обработки ошибок

Сервер:

```csharp
public override async Task<UserResponse> GetUser(GetUserRequest request, ServerCallContext context)
{
    if (string.IsNullOrEmpty(request.Id))
    {
        throw new RpcException(new Status(StatusCode.InvalidArgument, "User ID is required"));
    }
    
    var user = await _db.Users.FindAsync(request.Id);
    
    if (user == null)
    {
        throw new RpcException(new Status(StatusCode.NotFound, $"User {request.Id} not found"));
    }
    
    return new UserResponse { Name = user.Name };
}
```

Клиент:
```csharp
try
{
    var response = await client.GetUserAsync(new GetUserRequest { Id = "123" });
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
{
    Console.WriteLine("User not found");
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
{
    Console.WriteLine($"Invalid input: {ex.Status.Detail}");
}
catch (RpcException ex)
{
    Console.WriteLine($"gRPC error: {ex.Status}");
}
```

Rich Error Model (детальные ошибки)

```protobuf
import "google/rpc/error_details.proto";

// В коде
var status = new Status(StatusCode.InvalidArgument, "Validation failed");
var metadata = new Metadata
{
    { "validation-errors", JsonSerializer.Serialize(new { field = "email", message = "Invalid format" }) }
};
throw new RpcException(status, metadata);
```

## Метаданные (Metadata) в gRPC

Аналог HTTP headers:

Отправка метаданных (клиент):
```csharp
var metadata = new Metadata
{
    { "authorization", "Bearer token123" },
    { "user-id", "123" },
    { "correlation-id", Guid.NewGuid().ToString() }
};
var response = await client.GetUserAsync(new GetUserRequest { Id = "123" }, headers: metadata);
```

Чтение метаданных (сервер):

```csharp
public override async Task<UserResponse> GetUser(GetUserRequest request, ServerCallContext context)
{
    var authHeader = context.RequestHeaders.GetValue("authorization");
    var userId = context.RequestHeaders.GetValue("user-id");
    
    // Отправка метаданных в ответе
    await context.WriteResponseHeadersAsync(new Metadata
    {
        { "server-version", "1.0" },
        { "request-id", context.RequestHeaders.GetValue("correlation-id") }
    });
    
    return new UserResponse { Name = "John" };
}
```

## Interceptors (Middleware в gRPC)

Серверный Interceptor:

```csharp
public class LoggingInterceptor : Interceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation("Starting {Method}", context.Method);
        
        try
        {
            var response = await continuation(request, context);
            _logger.LogInformation("Completed {Method} in {ElapsedMs}ms", context.Method, stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in {Method}", context.Method);
            throw;
        }
    }
}

// Регистрация
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<LoggingInterceptor>();
});
```

Клиентский Interceptor:
```csharp
public class AuthInterceptor : Interceptor
{
    private readonly ITokenProvider _tokenProvider;

    public AuthInterceptor(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var token = _tokenProvider.GetToken();
        
        var metadata = new Metadata
        {
            { "authorization", $"Bearer {token}" }
        };
        
        var options = context.Options.WithHeaders(metadata);
        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method, context.Host, options);
        
        return continuation(request, newContext);
    }
}

// Использование
var channel = GrpcChannel.ForAddress("https://localhost:5001");
var invoker = channel.Intercept(new AuthInterceptor(tokenProvider));
var client = new UserService.UserServiceClient(invoker);
```

## Deadline и Timeout

```csharp
// Клиент - установка deadline (таймаут)
var response = await client.GetUserAsync(
    new GetUserRequest { Id = "123" },
    deadline: DateTime.UtcNow.AddSeconds(5)
);

// Или через CallOptions
var response = await client.GetUserAsync(
    new GetUserRequest { Id = "123" },
    new CallOptions(deadline: DateTime.UtcNow.AddSeconds(5))
);

// Сервер - проверка deadline
public override async Task<UserResponse> GetUser(GetUserRequest request, ServerCallContext context)
{
    // Проверяем не истек ли deadline
    if (context.CancellationToken.IsCancellationRequested)
    {
        throw new RpcException(new Status(StatusCode.DeadlineExceeded, "Request timeout"));
    }
    
    // Передаем CancellationToken дальше
    var user = await _db.Users.FindAsync(request.Id, context.CancellationToken);
    
    return new UserResponse { Name = user.Name };
}

```

Health Checking
```csharp
// Установка пакета
// dotnet add package Grpc.AspNetCore.HealthChecks

// Program.cs
builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());

app.MapGrpcHealthChecksService();

// Клиент может проверить здоровье сервиса
var healthClient = new Health.HealthClient(channel);
var response = await healthClient.CheckAsync(new HealthCheckRequest { Service = "" });
Console.WriteLine(response.Status); // SERVING, NOT_SERVING, UNKNOWN
```


Reflection (для отладки)
```csharp
// Установка
// dotnet add package Grpc.AspNetCore.Server.Reflection

// Program.cs (только в Development!)
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

// Теперь можно использовать grpcurl или postman для отладки
// grpcurl -plaintext localhost:5001 list
// grpcurl -plaintext localhost:5001 describe UserService
```


## Best Practices

1. Организация proto-контрактов

```text
proto/
├── common/
│   ├── types.proto          # Общие типы
│   └── pagination.proto     # Пагинация
├── users/
│   ├── v1/
│   │   └── users.proto
│   └── v2/
│       └── users.proto
└── hotels/
    └── v1/
        └── hotels.proto
```

2. Shared contracts в отдельном NuGet
```text
MyCompany.Grpc.Contracts/
├── Proto/
│   ├── users.proto
│   └── hotels.proto
└── MyCompany.Grpc.Contracts.csproj

// Публикуй в private NuGet
// Все сервисы reference этот пакет
```

3. API Gateway pattern
```csharp
// API Gateway переводит REST → gRPC
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService.UserServiceClient _grpcClient;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var response = await _grpcClient.GetUserAsync(new GetUserRequest { Id = id });
        return Ok(response);
    }
}
```

4. Retry policies с Polly
```csharp
builder.Services
    .AddGrpcClient<UserService.UserServiceClient>(options =>
    {
        options.Address = new Uri("https://users-service:5001");
    })
    .AddPolicyHandler(GetRetryPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
```

5. Observability (логи, метрики, трейсы)
```csharp
// OpenTelemetry для gRPC
builder.Services.AddOpenTelemetry()
    .WithTracing(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddGrpcClientInstrumentation()  // Трейсинг gRPC клиентов
        .AddSource("MyApp.*")
        .AddJaegerExporter());

// Prometheus metrics
app.UseGrpcMetrics();
```

## Типы данных

| Proto Type | C# Type    | Java Type  | Go Type | Default | Размер   | Когда использовать              |
| ---------- | ---------- | ---------- | ------- | ------- | -------- | ------------------------------- |
| double     | double     | double     | float64 | 0.0     | 8 bytes  | Высокая точность дробных чисел  |
| float      | float      | float      | float32 | 0.0f    | 4 bytes  | Дробные числа (меньше точность) |
| int32      | int        | int        | int32   | 0       | Variable | Положительные числа < 2³¹       |
| int64      | long       | long       | int64   | 0L      | Variable | Положительные числа < 2⁶³       |
| uint32     | uint       | int        | uint32  | 0U      | Variable | Только положительные < 2³²      |
| uint64     | ulong      | long       | uint64  | 0UL     | Variable | Только положительные < 2⁶⁴      |
| sint32     | int        | int        | int32   | 0       | Variable | Часто отрицательные < 2³¹       |
| sint64     | long       | long       | int64   | 0L      | Variable | Часто отрицательные < 2⁶³       |
| fixed32    | uint       | int        | uint32  | 0U      | 4 bytes  | Числа часто > 2²⁸               |
| fixed64    | ulong      | long       | uint64  | 0UL     | 8 bytes  | Числа часто > 2⁵⁶               |
| sfixed32   | int        | int        | int32   | 0       | 4 bytes  | Знаковые числа > 2²⁸            |
| sfixed64   | long       | long       | int64   | 0L      | 8 bytes  | Знаковые числа > 2⁵⁶            |
| bool       | bool       | boolean    | bool    | false   | 1 byte   | Логические значения             |
| string     | string     | String     | string  | ""      | Variable | UTF-8 текст                     |
| bytes      | ByteString | ByteString | []byte  | empty   | Variable | Бинарные данные                 |

Выбор числовых типов: шпаргалка

```text
// Положительные маленькие числа (ID, счетчики)
int32 user_id = 1;        // ✅ Обычно < 1M

// Большие положительные числа (timestamp в ms)
int64 timestamp_ms = 2;   // ✅ До 2^63

// ЧАСТО отрицательные числа (координаты, дельты)
sint32 temperature = 3;   // ✅ -50°C .. +50°C
sint64 balance_delta = 4; // ✅ Может быть отрицательной

// Очень большие числа (> 2^28)
fixed32 ip_address = 5;   // ✅ Всегда 4 байта
fixed64 hash_value = 6;   // ✅ Всегда 8 байт

// Только положительные (возраст, количество)
uint32 age = 7;           // ✅ 0..4 млрд
uint64 file_size = 8;     // ✅ 0..18 квинтиллионов
```

## Well-Known Types (google.protobuf.*)
Время и длительность

```text
import "google/protobuf/timestamp.proto";
import "google/protobuf/duration.proto";

message Event {
    google.protobuf.Timestamp created_at = 1;    // DateTime (UTC)
    google.protobuf.Timestamp updated_at = 2;    // DateTime (UTC)
    google.protobuf.Duration processing_time = 3; // TimeSpan
    google.protobuf.Duration ttl = 4;             // TimeSpan
}

```
C# маппинг:
Timestamp → DateTime (UTC only!)
Duration → TimeSpan

Методы:
```text
// Timestamp
var ts = Timestamp.FromDateTime(DateTime.UtcNow);
var ts2 = Timestamp.FromDateTimeOffset(DateTimeOffset.Now);
DateTime dt = ts.ToDateTime();
DateTimeOffset dto = ts.ToDateTimeOffset();

// Duration
var dur = Duration.FromTimeSpan(TimeSpan.FromMinutes(5));
TimeSpan span = dur.ToTimeSpan();
```

Nullable типы (Wrappers)

```text
import "google/protobuf/wrappers.proto";

message User {
    string name = 1;                              // Обязательное (default: "")
    google.protobuf.Int32Value age = 2;           // int? (nullable)
    google.protobuf.StringValue nickname = 3;     // string? (nullable)
    google.protobuf.BoolValue is_verified = 4;    // bool? (nullable)
    google.protobuf.DoubleValue balance = 5;      // double? (nullable)
}
```

Доступные wrappers:

| Wrapper Type | C# Type     | Когда использовать                 |
| ------------ | ----------- | ---------------------------------- |
| DoubleValue  | double?     | Nullable дробные числа             |
| FloatValue   | float?      | Nullable дробные (меньше точность) |
| Int64Value   | long?       | Nullable большие числа             |
| Int32Value   | int?        | Nullable числа                     |
| UInt64Value  | ulong?      | Nullable беззнаковые большие       |
| UInt32Value  | uint?       | Nullable беззнаковые               |
| BoolValue    | bool?       | Отличить false от null             |
| StringValue  | string?     | Отличить "" от null                |
| BytesValue   | ByteString? | Nullable бинарные данные           |

Когда использовать:
✅ Partial Update (PATCH) - обновляем только указанные поля
✅ Опциональные фильтры в поиске
✅ Отличить "не установлено" от "default значение"

## Другие Well-Known Types

```text
import "google/protobuf/empty.proto";
import "google/protobuf/any.proto";
import "google/protobuf/struct.proto";
import "google/protobuf/field_mask.proto";

// Empty - для методов без параметров
rpc DeleteUser(google.protobuf.Empty) returns (google.protobuf.Empty);

// Any - динамический тип (любое сообщение)
message Notification {
    string id = 1;
    google.protobuf.Any payload = 2;  // Может быть что угодно
}

// Struct - JSON-подобная динамическая структура
message Config {
    google.protobuf.Struct settings = 1;  // Произвольный JSON
}

// FieldMask - указание какие поля обновлять
message UpdateRequest {
    User user = 1;
    google.protobuf.FieldMask update_mask = 2;  // Например: "name,email"
}
```

# Сложные типы

## Enum

```text
enum Status {
    STATUS_UNSPECIFIED = 0;  // ⚠️ ОБЯЗАТЕЛЬНО! Первое значение = 0
    STATUS_PENDING = 1;
    STATUS_ACTIVE = 2;
    STATUS_INACTIVE = 3;
    STATUS_DELETED = 4;
}

enum Priority {
    PRIORITY_UNSPECIFIED = 0;
    PRIORITY_LOW = 1;
    PRIORITY_MEDIUM = 2;
    PRIORITY_HIGH = 3;
    
    // Алиасы (одинаковые значения)
    option allow_alias = true;
    PRIORITY_URGENT = 3;  // Тот же код что HIGH
}
```

Best Practices:

✅ Первое значение всегда 0 с суффиксом _UNSPECIFIED
✅ Префикс для избежания коллизий (STATUS_, PRIORITY_)
✅ Не удаляй значения - делай reserved

## Repeated (массивы/списки)

```text
message Hotel {
    string id = 1;
    string name = 2;
    repeated string amenities = 3;           // List<string>
    repeated Room rooms = 4;                 // List<Room>
    repeated int32 prices = 5 [packed=true]; // Упакованный массив (по умолчанию)
}
```

C# маппинг:
repeated → RepeatedField<T> (implements IList<T>)
Всегда инициализирован (не бывает null)

Операции:

```csharp
var hotel = new Hotel();

// Add
hotel.Amenities.Add("WiFi");

// AddRange
hotel.Amenities.AddRange(new[] { "Pool", "Gym" });

// Инициализатор
var hotel2 = new Hotel
{
    Amenities = { "WiFi", "Pool", "Gym" }
};

// Count, индексация, LINQ работают
Console.WriteLine(hotel.Amenities.Count);
var first = hotel.Amenities[0];
var filtered = hotel.Amenities.Where(a => a.StartsWith("W")).ToList();
```

Map (словари)

```text
message Product {
    string id = 1;
    map<string, string> labels = 2;          // Dictionary<string, string>
    map<int32, Product> related = 3;         // Dictionary<int, Product>
    map<string, double> prices_by_currency = 4; // Dictionary<string, double>
}
```

Ограничения:
❌ Ключ НЕ может быть: float, double, bytes, message, enum
✅ Ключ может быть: любой int, uint, sint, fixed, string, bool
✅ Значение может быть любым типом (включая message)

C# использование:
```csharp
var product = new Product();

// Добавление
product.Labels.Add("color", "red");
product.Labels["size"] = "XL";

// Инициализатор
var product2 = new Product
{
    Labels = 
    {
        ["color"] = "red",
        ["size"] = "XL"
    }
};

// Dictionary операции работают
if (product.Labels.ContainsKey("color"))
{
    string color = product.Labels["color"];
}

```

Oneof (дискриминированный union)

```text
message SearchFilter {
    oneof filter {
        string name = 1;
        int32 id = 2;
        GeoLocation location = 3;
        DateRange date_range = 4;
    }
}
```

Поведение:

Можно установить только одно поле из oneof
При установке нового поля старое сбрасывается
Генерируется enum FilterCase для проверки

```text
var filter = new SearchFilter { Name = "hotel" };

// Проверка какое поле установлено
switch (filter.FilterCase)
{
    case SearchFilter.FilterOneofCase.Name:
        Console.WriteLine($"Search by name: {filter.Name}");
        break;
    case SearchFilter.FilterOneofCase.Id:
        Console.WriteLine($"Search by ID: {filter.Id}");
        break;
    case SearchFilter.FilterOneofCase.None:
        Console.WriteLine("No filter set");
        break;
}

// При смене поля старое обнуляется
filter.Id = 123;
Console.WriteLine(filter.Name); // "" (сброшено!)
```

Вложенные сообщения (Nested Types)

```text
message Hotel {
    string id = 1;
    string name = 2;
    
    message Address {
        string street = 1;
        string city = 2;
        string country = 3;
        int32 zip_code = 4;
    }
    
    Address address = 3;
    repeated Address branches = 4;
    
    enum Status {
        STATUS_UNSPECIFIED = 0;
        STATUS_ACTIVE = 1;
        STATUS_INACTIVE = 2;
    }
    
    Status status = 5;
}

```

C# маппинг:
```csharp
var hotel = new Hotel
{
    Id = "hotel-1",
    Name = "Grand Hotel",
    Address = new Hotel.Types.Address  // ⚠️ Hotel.Types.Address!
    {
        Street = "Main St",
        City = "Moscow",
        Country = "Russia"
    },
    Status = Hotel.Types.Status.Active // ⚠️ Hotel.Types.Status!
};
```

Нумерация полей (Field Numbers)

```text
message User {
    // 1-15: 1 байт для номера (используй для частых полей)
    string id = 1;
    string name = 2;
    string email = 3;
    // ... до 15
    
    // 16-2047: 2 байта для номера
    string description = 16;
    repeated string tags = 17;
    
    // 2048+: 3+ байта (редко используемые поля)
    string internal_notes = 2048;
    
    // Зарезервированные номера и имена
    reserved 100, 101, 102;           // Номера нельзя использовать
    reserved "old_field", "deprecated_field"; // Имена нельзя использовать
}
```

Правила:
- ✅ Диапазон: 1 до 536,870,911 (2²⁹ - 1)
- ❌ Нельзя использовать: 19000-19999 (зарезервировано protobuf)
- 🔥 1-15: используй для самых частых полей (1 байт overhead)
- ⚠️ 16-2047: нормальные поля (2 байта overhead)
- 📦 2048+: редкие/внутренние поля (3+ байта)

Optional поля (proto3)
```text
syntax = "proto3";

message User {
    string name = 1;             // Implicit presence (default = "")
    optional string nickname = 2; // Explicit presence (nullable)
    optional int32 age = 3;       // Explicit presence (nullable)
    optional bool is_active = 4;  // Explicit presence (nullable)
}
```

```csharp
var user = new User { Name = "John" };

// Есть Has* и Clear* методы
if (user.HasNickname)
{
    Console.WriteLine(user.Nickname);
}

user.Age = 25;
Console.WriteLine(user.HasAge);  // true

user.ClearAge();
Console.WriteLine(user.HasAge);  // false
Console.WriteLine(user.Age);     // 0 (default)
```

Optional vs Wrappers:

| Критерий               | optional           | google.protobuf.*Value   |
| ---------------------- | ------------------ | ------------------------ |
| Proto версия           | proto3 v3.15+      | proto3 любая             |
| Синтаксис              | ✅ Проще            | Многословнее             |
| C# маппинг             | T + HasX/ClearX    | T?                       |
| Обратная совместимость | ⚠️ С proto3 v3.15+ | ✅ Везде                  |
| Рекомендация           | Для новых проектов | Для старой совместимости |

Service определения

```text
service HotelService {
    // Unary: один запрос → один ответ
    rpc GetHotel(GetHotelRequest) returns (Hotel);
    
    // Server streaming: один запрос → поток ответов
    rpc ListHotels(ListHotelsRequest) returns (stream Hotel);
    
    // Client streaming: поток запросов → один ответ
    rpc BulkCreateHotels(stream CreateHotelRequest) returns (BulkResponse);
    
    // Bidirectional streaming: поток запросов ↔ поток ответов
    rpc ChatWithSupport(stream ChatMessage) returns (stream ChatMessage);
    
    // Empty параметры/ответы
    rpc Ping(google.protobuf.Empty) returns (google.protobuf.Empty);
}
```

Опции файла (.proto)

```text
syntax = "proto3";

// Package (для других языков, в C# не влияет)
package hotels.v1;

// C# namespace
option csharp_namespace = "MyCompany.Hotels.V1";

// Java package
option java_package = "com.mycompany.hotels.v1";
option java_outer_classname = "HotelsProto";
option java_multiple_files = true;

// Go package
option go_package = "github.com/mycompany/hotels/v1;hotelsv1";

// Оптимизация
option optimize_for = SPEED; // SPEED | CODE_SIZE | LITE_RUNTIME

// Deprecated файл
option deprecated = true;
```

Опции полей

```csharp
message User {
    string id = 1;
    
    // Deprecated поле
    string old_field = 2 [deprecated = true];
    
    // Packed массив (по умолчанию в proto3)
    repeated int32 numbers = 3 [packed = true];
    
    // Unpacked массив
    repeated int32 old_numbers = 4 [packed = false];
    
    // JSON name (для Web)
    string email_address = 5 [json_name = "emailAddr"];
}
```

Импорты

```text
// Публичный импорт (реэкспорт)
import public "common/types.proto";

// Обычный импорт
import "google/protobuf/timestamp.proto";
import "google/protobuf/wrappers.proto";

// Слабый импорт (опциональный, может отсутствовать)
import weak "optional_types.proto";
```

Паттерны именования

Файлы

```csharp
// snake_case с версией
user_service.proto
hotel_booking_v1.proto
common_types.proto
```

Messages

```csharp
// PascalCase
message UserProfile { }
message HotelSearchRequest { }
message DataListResponse { }
```

Fields

```csharp
// snake_case
string user_name = 1;
int32 total_count = 2;
google.protobuf.Timestamp created_at = 3;
```

Enums

```csharp
// SCREAMING_SNAKE_CASE с префиксом
enum OrderStatus {
    ORDER_STATUS_UNSPECIFIED = 0;
    ORDER_STATUS_PENDING = 1;
    ORDER_STATUS_CONFIRMED = 2;
}
```

Services

```csharp
// PascalCase
service UserService { }
service HotelBookingService { }
```

RPC методы

```csharp
// PascalCase, глагол + существительное
rpc GetUser(...) returns (...);
rpc CreateHotel(...) returns (...);
rpc ListOrders(...) returns (...);
rpc DeleteBooking(...) returns (...);
```

Версионирование
Способ 1: Директории

```csharp
proto/
├── v1/
│   ├── user_service.proto
│   └── hotel_service.proto
└── v2/
    ├── user_service.proto
    └── hotel_service.proto
```

```csharp
// v1/user_service.proto
syntax = "proto3";
package users.v1;
option csharp_namespace = "MyCompany.Users.V1";

// v2/user_service.proto
syntax = "proto3";
package users.v2;
option csharp_namespace = "MyCompany.Users.V2";
```

Способ 2: Обратная совместимость

```csharp
message UserV1 {
    string id = 1;
    string name = 2;
}

// Расширение (обратная совместимость)
message UserV2 {
    string id = 1;
    string name = 2;
    string email = 3;      // Новое поле (старые клиенты игнорируют)
    string phone = 4;      // Новое поле
}
```

Правила обратной совместимости:

- ✅ Можно добавлять новые поля
- ✅ Можно добавлять новые RPC методы
- ✅ Можно добавлять новые enum значения
- ❌ Нельзя удалять поля (используй reserved)
- ❌ Нельзя менять типы полей
- ❌ Нельзя менять номера полей

Конфигурация .csproj

```csharp
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <!-- Server -->
    <Protobuf Include="Protos\**\*_service.proto" GrpcServices="Server" />
    
    <!-- Client -->
    <Protobuf Include="Protos\external\*.proto" GrpcServices="Client" />
    
    <!-- Both -->
    <Protobuf Include="Protos\shared\*.proto" GrpcServices="Both" />
    
    <!-- None (только DTO) -->
    <Protobuf Include="Protos\models\*.proto" GrpcServices="None" />
    
    <!-- С дополнительными опциями -->
    <Protobuf Include="Protos\internal.proto" 
              GrpcServices="Server" 
              Access="Internal" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Grpc.AspNetCore" Version="2.60.0" />
    <PackageReference Include="Grpc.Tools" Version="2.60.0" PrivateAssets="All" />
    <PackageReference Include="Google.Protobuf" Version="3.25.0" />
  </ItemGroup>
</Project>
```

## Полный пример: REST-like API

```protobuf
syntax = "proto3";

package hotels.v1;

option csharp_namespace = "MyCompany.Hotels.V1";

import "google/protobuf/timestamp.proto";
import "google/protobuf/wrappers.proto";
import "google/protobuf/empty.proto";
import "google/protobuf/field_mask.proto";

// ============ Enums ============

enum HotelStatus {
    HOTEL_STATUS_UNSPECIFIED = 0;
    HOTEL_STATUS_ACTIVE = 1;
    HOTEL_STATUS_INACTIVE = 2;
    HOTEL_STATUS_MAINTENANCE = 3;
}

// ============ Models ============

message Hotel {
    string id = 1;
    string name = 2;
    optional string description = 3;
    HotelStatus status = 4;
    double rating = 5;
    
    Address address = 6;
    repeated string amenities = 7;
    map<string, string> metadata = 8;
    
    google.protobuf.Timestamp created_at = 9;
    google.protobuf.Timestamp updated_at = 10;
    
    message Address {
        string street = 1;
        string city = 2;
        string country = 3;
        google.protobuf.StringValue postal_code = 4;
    }
}

// ============ Requests/Responses ============

message GetHotelRequest {
    string id = 1;
}

message ListHotelsRequest {
    int32 page_size = 1;
    string page_token = 2;
    optional HotelStatus status = 3;
    optional string city = 4;
}

message ListHotelsResponse {
    repeated Hotel hotels = 1;
    string next_page_token = 2;
    int32 total_count = 3;
}

message CreateHotelRequest {
    string name = 1;
    Hotel.Address address = 2;
    repeated string amenities = 3;
}

message UpdateHotelRequest {
    string id = 1;
    google.protobuf.StringValue name = 2;
    google.protobuf.StringValue description = 3;
    google.protobuf.FieldMask update_mask = 4;
}

message DeleteHotelRequest {
    string id = 1;
}

// ============ Service ============

service HotelService {
    // CRUD
    rpc GetHotel(GetHotelRequest) returns (Hotel);
    rpc ListHotels(ListHotelsRequest) returns (ListHotelsResponse);
    rpc CreateHotel(CreateHotelRequest) returns (Hotel);
    rpc UpdateHotel(UpdateHotelRequest) returns (Hotel);
    rpc DeleteHotel(DeleteHotelRequest) returns (google.protobuf.Empty);
    
    // Streaming
    rpc StreamHotels(ListHotelsRequest) returns (stream Hotel);
    rpc BulkCreateHotels(stream CreateHotelRequest) returns (ListHotelsResponse);
}
```