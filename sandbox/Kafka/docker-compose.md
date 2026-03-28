# S3 WebApi



# Собрать и запустить в фоне
docker compose up --build -d

# Посмотреть логи
docker compose logs -f

# Остановить
docker compose down

# Полная очистка (с данными)
docker compose down --volumes

# Если ты изменил код .NET или Dockerfile, выполнить:
docker compose up --build -d


docker build -t filestorageapi:latest .

docker run -d \
    --name fileapi \
    -p 5242:80 \
    -e ASPNETCORE_ENVIRONMENT=Development \
    -e AWS__Region=us-east-1 \
    -e AWS__ServiceURL=http://host.docker.internal:9000 \
    -e AWS__AccessKey=minioadmin \
    -e AWS__SecretKey=minioadmin \
    -e AWS__BucketName=myfiles \
    filestorageapi:latest



docker compose down --volumes --remove-orphans
docker compose up --build -d


# Контейнеры

Контейнер — это запущенный (или остановленный) экземпляр образа.
Он как "виртуальная машина", но легковесная — содержит приложение, его зависимости и среду выполнения.
Пример: контейнер с твоим .NET API или MinIO.
🔹 Где живёт?
Запущен в памяти (если работает)
Или остановлен, но сохранён в списке (можно запустить снова)


# Удалить один контейнер
docker rm имя_контейнера
# Удалить остановленный контейнер принудительно
docker rm -f имя_контейнера
# Удалить все остановленные контейнеры
docker container prune
# Удалить ВСЕ контейнеры (даже запущенные — остановит и удалит)
docker rm -f $(docker ps -aq)


# Образы

Образ — это шаблон для контейнера.
Как "установочный ISO", из которого запускаются контейнеры.
Примеры:
mcr.microsoft.com/dotnet/aspnet:9.0
minio/minio:latest
filestorageapi:latest (твой собранный образ)
🔹 Где живёт?
На диске, занимает место
Не исчезает сам по себе, даже если контейнер удалён

# Удалить один образ
docker rmi имя_образа:тег
# Пример:
docker rmi filestorageapi:latest
# Удалить все образы с определённым именем
docker rmi $(docker images 'filestorageapi' -q)
# Удалить ВСЕ неиспользуемые образы
docker image prune -a

# Тома
Том — это персистентное хранилище данных, которое не исчезает при удалении контейнера.
Пример: данные MinIO (файлы, которые ты загрузил) хранятся в volume, а не в контейнере.
🔹 Зачем?
Сохранить данные после пересоздания контейнера
Поделиться данными между контейнерами
Быстрый доступ к файлам с хоста


docker volume ls
# Удалить один том
docker volume rm имя_тома
# Пример:
docker volume rm minio_data
# Удалить ВСЕ неиспользуемые тома
docker volume prune
# Удалить ВСЕ тома (включая используемые — нужно остановить контейнеры)
docker volume prune -a


docker container prune
docker image prune
docker volume prune

docker image prune -a


# Почисти кэш сборки
docker builder prune -f


docker compose down --rmi local --volumes --remove-orphans

