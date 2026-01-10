# FitHub

Демонстрационный проект: вертикальная SaaS-платформа для фитнес-индустрии в виде монолита
- Прототип системы, который объединяет логику ERP (управление ресурсами компании и конкретных залов) и CRM (работа с посетителями с помощью чата)
- Полигон (sandbox) для проверки гипотез и предположений насчет конкретных технологий

## Содержание

- [Стек](#Tech-stack)
- [Дальнейшие планы](#дальнейшие-планы)
- [Демонстрация работы](#демонстрация-работы)
- [Достойно отдельного упоминания](#достойно-отдельного-упоминания)
    - [Валидация форм](#formvalidation)
    - [Read Model денормализация для производительности](#read-model-денормалидация)
    - [JSONB для динамических данных, и как это связано с BDUI](#jsonb-для-динамических-данных-и-как-это-связано-с-bdui)
    - [SignalR для real-time чата и не только](#signalr)


## Tech stack
- Монолит (пока что)
- Clean Architecture (Presentation, Infrastucture, Core)
- в домене прослеживается DDD (Аггрегаты, Value Objects; инварианты внутри Entity, а не размазаны по сервисам)
- .NET 9
    - Nullable reference types
    - Central package management
- ASP.NET Core with SignalR (WebSockets)
    - JWT Authentication (для апи ручек и SignalR)
    - Policy based Authorization на основе JWT Claimы
- EF Core 9
    - либа: базовые репозитории, UnitOfWork, конвеншны (enum as string, ValueType Identifiers, Interceptors для аудитабл полей и тд)
    - PostgreSQL: реляционная модель, индексы, jsonb, read-models денормализация для perfomance
- RabbitMQ (🌱 на будущее)
    - Базовая библиотека production-readiness: producer, consumer, DLQ, routing
- Minio S3
    - upload via presigned urls
    - отказоустойчивый upload flow для фотографий благодаря компенсирующей cleanup job (запись в бд не остается без файла в s3, файл в s3 не остается без записи в бд)
- Unit And Integration Testing
    - xUnit, Moq, Autofixture
    - TestContainers
    - WebApplicationFactory с подменой части IServiceCollection (auth и внешние апи клиенты)
- React
    - TypeScript, Vite
    - React Hook Form
    - Ant Design, Tailwind
    - Redux with Redux Toolkit

## Дальнейшие планы
- IdentityServer with SSO (KeyCloak)
    - OAuth, OIDC
- OpenTelemetry for Ditributed logs and traces
- GRPC
- BDUI
- Геопозиционирование
    - точки, полигоны на карте
- Полноценная система доставки: delivery, orders, warehouse
    - Логистические вопросы: поиск наилучших путей, оптимизация работы курьеров
    - Антифрод, подозрительная активность


## Демонстрация работы

Модуль CMS администратора: [README с демонстрацией](./docs/cmsAdminModule/README.md)

Модуль администратора зала: [README а с демонстрацией](./docs/gymAdminModule/README.md)

## Достойно отдельного упоминания

### FormValidation 
[Readme](./docs/formValidation/README.md)

### Read Model денормалидация
[Readme](./docs/readModels/README.md)

### JSONB для динамических данных, и как это связано с BDUI
[Readme](./docs/jsonb/README.md)


### SignalR
[Readme](./docs/signalR/README.md)