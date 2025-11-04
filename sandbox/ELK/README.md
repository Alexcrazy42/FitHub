📁 Logstash
logstash.yml - Основные настройки (память, порты, пути)

pipelines.yml - Управление пайплайнами (параллельная обработка)

*.conf - Конфиги пайплайнов (input/filter/output)

jvm.options - Настройки JVM для Logstash

log4j2.properties - Логирование Logstash

startup.options - Опции запуска (только для .deb/.rpm)

📁 Elasticsearch
elasticsearch.yml - Главный конфиг (кластер, сеть, безопасность)

jvm.options - Настройки памяти JVM (Xmx, Xms, GC)

log4j2.properties - Логирование ES

role_mapping.yml - Маппинг ролей (LDAP/AD интеграция)

roles.yml - Статические роли (устарело, лучше через API)

users/users_roles - Локальные пользователи (устарело)

📁 Kibana
kibana.yml - Основной конфиг (порты, подключение к ES, индексы)

node.options - Настройки Node.js (редко используется)

🔐 Безопасность (дополнительно)
Сертификаты - .crt, .key, .p12 файлы для SSL/TLS

elasticsearch.keystore - Хранилище секретов ES

kibana.keystore - Хранилище секретов Kibana

📊 Мониторинг (опционально)
metricbeat.yml - Для сбора метрик ELK стека

filebeat.yml - Для сбора логов ELK компонентов




http://localhost:9200/_cat/indices

http://localhost:9200/app-logs-2025.08.29/_search?pretty
{ "size": 10 }