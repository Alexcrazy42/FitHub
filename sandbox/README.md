# Технологии для Sandbox


Роадмап:
    подготовка:
        + S3: done
        + Kafka: done
        + ELK. цель: я вижу логи в кибане, пишу их из C# через Serilog, они сохраняются в течении недели в volume
        + Jobs. Цель: я запускаю задачи на разных нодах с помощью Hangfire. они конкурируют за выполнение задач, но выигрывает только один
        + Упаковка ASP.NET + Postgres + S3 + Kafka + ELK в докеркомпос
        + Добавить Redis в Docker-compose
        + OpenTelemetry: по всем микросервисам формируются trace, я вижу span по: бд, http запросы, kafka, фоновые задачи (?)
        + Prometheus + Grafana в docker-compose
        + RabbitMQ
    
    основная часть:
        планирование модулей (бэк, тесты, фронт)


    дальнейшая часть:
        Quartz
        Kubernetes + IAC (Helm, Terraform, Ansible)
        Keycloak/Okta/Auth0
        Consul
        Vault
        
        
        



## БАЗА

PostgreSql +
    Redis
    Mongo
    ClickHouse
    Doris или другой Column Store

S3 +

Брокеры
    Kafka +
    RabbitMQ +


Логи: 
    ELK (Elasticsearch, Logstash, Kibana) +
    Loki (Grafana Labs) — легковесная альтернатива ELK.
    

Jobs
    TickerQ +- не зашел совсем + уж слишком новый и сырой, хоть и перспективный
    Hangfire +
    Quartz


OpenTelemetry — стандарт для сбора метрик, логов, трейсов. +
    Jaeger +
    Zipkin


Prometheus + Grafana +
    AlertManager

## DevOps
Consul
Kubernetes + IAC (Helm, Terraform, Ansible)
Vault

CI/CD: Gitlab CI, Github Actions / Jenkins / Argo CD - пайплайны CI/CD
    Argo CD / Flux - GitOps для K8S
    Nexus / Frog Artifactory -  хранение артефактов (Docker-образов, бинарников).

Service mesh, Ingress-контроллер, cert-manage


## ИДЕИ ЕЩЕ ЧТО ДОБАВИТЬ

1. signalR
2. видеоплатформа
3. общение голосом
4. общение по камере
5. показывание экрана
6. грид с фильтрами, можно выбирать колонки, можно выбрать разные колонки для фильтров.
7. Backend Driven UI
8. динамические usecases в системе
{
  "event": "order_created",
  "conditions": ["amount > 1000", "customer.type = 'VIP'"],
  "actions": ["send_email_to_manager", "create_task_for_sales"]
}

Используется id > last_id или (created_at, id) > (last_time, last_id).
Быстро на больших таблицах, нет проблем с OFFSET


## ТЕСТЫ

xUnit / NUnit / MSTest → юнит-тесты (обычно xUnit).
FluentAssertions → удобные ассерты.
WireMock.Net → моки HTTP-сервисов.
Testcontainers for .NET → запуск тестов с реальными контейнерами (Postgres, Kafka, Redis).
SonarQube → анализ качества кода и метрик.
netarchitect.rules


## .NET глубина

.NET (CQRS, DDD, паттерны интеграции)
Polly, Serilog, MediatR, MassTransit