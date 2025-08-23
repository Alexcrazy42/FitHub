# Технологии для Sandbox



PostgreSql
Jobs (Ticker, Quartz, Hangfire)
Kafka
Rabbit MQ
S3
Prometheus + Grafana
ELK (Elasticsearch, Logstash, Kibana)
    Loki (Grafana Labs) — легковесная альтернатива ELK.
    Jaeger / Zipkin — распределённый трейсинг (tracing), особенно важно при микросервисах.
    OpenTelemetry — стандарт для сбора метрик, логов, трейсов.

Consul
Kubernetes + IAC (Helm, Terraform, Ansible)
Vault
Keycloak/Okta/Auth0
CI/CD: Gitlab CI, Github Actions / Jenkins / Argo CD - пайплайны CI/CD
    Argo CD / Flux - GitOps для K8S
    Nexus / Frog Artifactory -  хранение артефактов (Docker-образов, бинарников).

Service mesh, Ingress-контроллер, cert-manage


когда что то перестает работать мы узнаем это постфактум, нужно прийти к тому, чтобы мы это узнавали из алертов в тг