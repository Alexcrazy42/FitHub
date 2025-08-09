# DDX клон

Хайповые штуки: OpenTelemetry (Activity, TraceId, SpanId, Jaeger/Zipkin), Sentry, SignalR with Redis Backplane


Бэкенд

ASP.NET Core
DB: Postgres, EF Core, Views, Stored Procedures, Function
Broker: Kafka
S3: Minio
Авторизация на JWT: Access + Refresh
SignalR with Redis Backplane (https://learn.microsoft.com/en-us/aspnet/signalr/overview/performance/scaleout-with-redis)
Background Tasks: Hangfire with Postgres
Logging: ELK, Serilog
Build: Nuke

Метрики: Prometheus + Grafana
Трейсинг: OpenTelemetry + Jaeger/Zipkin

Tests: Unit (xUnit, Moq, Autofixture), Integrations (TestContainers)


Локальный проксинг продакшн-API
Ngrok: Пробрасывает локальный localhost:5000 в публичный URL.
ngrok http 5000
→ Тестируют вебхуки (платежи, уведомления).
WireMock.NET

1. Mock серверы
WireMock.NET (для .NET)
Postman Mock Servers
JSON Server (для простых REST API)

2. Contract Testing (Pact)
Pact.NET (для .NET)


3.  VCR-тестирование (запись/воспроизведение)
Инструменты:

Betamax (Java)
VCR.py (Python)
Nock (Node.js)

Как работает:
При первом запуске тест записывает реальные ответы API в файл.
При следующих запусках — использует сохраненные ответы.



Фронтенд
React
TypeScript
Ant Design
Vite
Redux / Zustand
React-Hook-Form
Yup
Axios (RTK Query)
в рамках одного фронтенда с разграничиванием страниц и ролей. мб потом попилить на несколько фронтендов

Фичи: темная тема, мультидоступы (одна платформа для всех)