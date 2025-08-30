# Технологии для Sandbox


Роадмап:
    + S3: done
    + Kafka: done
    + ELK. цель: я вижу логи в кибане, пишу их из C# через Serilog, они сохраняются в течении недели в volume
    Jobs. Цель: я запускаю задачи на разных нодах с помощью TickerQ. они конкурируют за выполнение задач, но выигрывает только один
    Упаковка ASP.NET + Postgres + S3 + Kafka + ELK в докеркомпос
    Написание логики приложения целиком в монолите (бэк + фронт)

    


PostgreSql

S3 +

Брокеры
    Kafka +
    Rabbit MQ


Логи: 
    ELK (Elasticsearch, Logstash, Kibana) +
    Loki (Grafana Labs) — легковесная альтернатива ELK.
    

Jobs
    TickerQ
    Hangfire
    Quartz


OpenTelemetry — стандарт для сбора метрик, логов, трейсов.
    Jaeger / Zipkin — распределённый трейсинг (tracing), особенно важно при микросервисах.


Prometheus + Grafana

Consul
Kubernetes + IAC (Helm, Terraform, Ansible)
Vault
Keycloak/Okta/Auth0
CI/CD: Gitlab CI, Github Actions / Jenkins / Argo CD - пайплайны CI/CD
    Argo CD / Flux - GitOps для K8S
    Nexus / Frog Artifactory -  хранение артефактов (Docker-образов, бинарников).

Service mesh, Ingress-контроллер, cert-manage

когда что то перестает работать мы узнаем это постфактум, нужно прийти к тому, чтобы мы это узнавали из алертов в тг