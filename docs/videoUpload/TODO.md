# Video Upload — TODO

---

## 1. Poster generation не работает

`VideoService.ProcessAsync` содержит закомментированный код создания постера (`FFMpeg.SnapshotAsync`).
Постер не сканируется по неизвестной причине — надо разобраться и включить обратно.

Сейчас `video.MarkReady` вызывается с пустым `posterS3Key = ""`.

**Файлы:** `Application/Videos/VideoService.cs:227`

---

## 2. Пагинация `GET /api/v1/videos` +

Сейчас `GetAll` возвращает все видео без пагинации.
Нужно добавить курсорную или offset-пагинацию с условием: **видео в статусе `Pending`/`Processing` идут первыми**, затем остальные по дате убывания.

**Файлы:**
- `Web/V1/Videos/VideoController.cs:27`
- `Application/Videos/VideoService.cs` — `GetAllAsync`

---

## 3. Фоновая загрузка + multipart upload +

Загрузка сейчас блокирует модальное окно и прерывается при смене вкладки.

Нужно:
- Переключиться на **multipart upload** (несколько частей параллельно, возможность паузы/возобновления)
- Сделать загрузку **фоновой** — пользователь должен иметь возможность уйти на другую вкладку, не прерывая загрузку
- Рассмотреть использование `Worker` или persisted state в Redux

**Файлы:**
- `frontend/src/api/videoApi.ts` — `uploadToS3`
- `frontend/src/pages/admin/videos/VideosAdminPage.tsx` — upload modal

---

## 4. Presigned URL на отдачу видео — слишком короткий TTL

Сейчас `GetPresignedDownloadUrlAsync` вызывается с TTL `TimeSpan.FromHours(2)` при каждом запросе `GET /videos` и `GET /videos/{id}/resolutions`.

Варианты:
- **a)** Кэшировать URL и перегенерировать за 1 день до истечения
- **b)** Настроить bucket policy на публичное чтение (если контент не конфиденциальный) — тогда URL не нужны вообще
- **c)** Генерировать URL один раз и хранить в БД с timestamp истечения

**Файлы:**
- `Application/Videos/VideoService.cs:111`
- `Web/V1/Videos/VideoController.cs:33,47`

---

## 5. Poster URL генерируется в контроллере — не место бизнес-логике

`IS3FileService` инжектируется напрямую в `VideoController` для генерации poster URL.
Эту логику нужно перенести в `VideoService` (или в extension-метод `ToResponse`), чтобы контроллер не знал про S3.

**Файлы:** `Web/V1/Videos/VideoController.cs:16,33,47`

---

## 6. `POST /api/v1/videos/{id}/process` открыт анонимно

Эндпоинт помечен `[AllowAnonymous]` — это дыра в безопасности: любой может запустить обработку произвольного видео.

Нужно защитить его: выписать служебный JWT или shared secret для `HostJobs`, и проверять его при вызове `/process`.

**Файлы:** `Web/V1/Videos/VideoController.cs:69`

---

## 7. Кодирование только профилей ≤ оригинала +

Сейчас кодируются все три профиля (360p, 720p, 1080p) вне зависимости от разрешения оригинала.
Если оригинал 720p — нет смысла кодировать 1080p.
Нужно: перед кодированием через `FFProbe` получить разрешение оригинала и брать только профили с `Height <= оригинала`.
Также рассмотреть, нужно ли хранить оригинал в S3 после кодирования.

**Файлы:** `Application/Videos/VideoService.cs:17`

---

## 8. Нет Outbox для публикации в RabbitMQ

В `ConfirmUploadAsync` сначала сохраняется состояние файла в БД, потом публикуется сообщение в очередь.
Если сервис упадёт после `SaveChanges` но до `EnqueueAsync` — видео зависнет в статусе `Pending` навсегда.

Варианты:
- Реализовать Outbox (сохранять сообщение в БД в той же транзакции, потом worker публикует)
- Или добавить фоновый retry/sweep job по видео в `Pending` старше N минут

**Файлы:** `Application/Videos/VideoService.cs:75`

---

## 9. Временные папки при кодировании могут не удаляться

`Directory.Delete(workDir, recursive: true)` вызывается в `finally`, но ошибка проглатывается (`catch { /* best-effort */ }`).
Если FFmpeg или OS держит handle на файл — папка останется навсегда в `%TEMP%/fithub_videos/`.

Нужно логировать неудаление и/или добавить cleanup job.

**Файлы:** `Application/Videos/VideoService.cs:251`

---

---

## 10. RabbitMQ producer/consumer — независимый старт

Продюсер (в `Host`) и консюмер (в `HostJobs`) сейчас могут зависеть от порядка запуска: если очередей ещё нет — старт упадёт.

Варианты:
- Запускать `HostJobs` раньше `Host` (консюмер создаёт очереди при подключении)
- Создавать очереди заранее (в dev — через `NeedToPrepare: true`, в prod — через IaC)
- Добавить retry с backoff при подключении к RabbitMQ на старте

---
