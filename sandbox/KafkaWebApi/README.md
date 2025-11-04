# Kafka WebApi

Prod Ready условия:
    Множество консюмеров и продюсеров
    Продюсер не задумывается кто и как это будет обрабатывать
    Консюмеры хотят обрабатывать одни и те же события, но чтобы не было дублей (Consumer Group Id)
    Dead Letter Queue с возможностью переобработать сообщения из нее
    кластер кафки


# Главные термины
Брокер
Топик
Партиция
Консюмер группа
member in consumer group
__consumer_offsets - где хранятся оффсеты для консюмер групп
Кластер
Лидер
Фолловер
InSyncReplica (ISR)
Idempotency Producer

Log End Offset (LEO)
HW (High Watermark) - то, что лидер считает безопасным offset
Sequence на продюсере


# Главные проблемы
    потеря данных -> на продюсере использовать outbox, на консюмере EnableAutoCommit = false + ручной коммит после handle
    дублирование на продюсере/консюмере -> идемпотентные handler-ы
    большая проблема с эффективностью Outbox на тысячах RPS (на DotNext есть исчерпывающий доклад про это "Outbox: сложно, казалось бы о простом")
    

## Основные принципы работы
1. Group Coordinator - отвечает за консюмера. он срабатывает при consumer.Consume(cancellationToken);
2. Rebalance - это процесс перераспределения партиций между консюмерами в группе
3. assignment:
```csharp
{
  "member_id": "consumer-1",
  "topic": "orders",
  "partitions": [0, 1, 2, 3, 4]
}
```
4. при добавлении нового консюмера (в рамках group.id) group coordinator запускает rebalance
consumer-1 → P0, P1, P2
consumer-2 → P3, P4
5. group coordinator (один из брокеров) различает консюмеров внутри group.id по member.id (клиент сам генерит Confluent.Kafka), либо кафка сама)
    этапы для GroupCoordinator в рамках JoinGroupProtocol:
        - FindCoordinator: новый консюмер узнает, где находится координатор для группы
        - JoinGroup: новый консьюмер отправляет на координатор запрос с возможностью вступления в группу
        - координатор: регистрирует нового участника, ждет пока все консюмер отправят JoinGroup, выбирает лидера группового из консюмеров
        - SyncGroup: лидер предлагает какие консюмеры какие партиции будет читать, координатор отправляет всем назначения
6. падение консюмера при потере связи между координатором и консюмером
   кафка проверяет что консюмер жив благодаря тому, что он постоянно отправляет запрос, что он жив
   session.timeout.ms - Сколько времени Kafka ждёт, пока консюмер отправит heartbeat (по умолч. 45 сек)
   heartbeat.interval.ms - как часто консюмер отправляет heartbeat (например каждые 3-5 сек)
7. 
client.id - id для нас (для логов, мониторинга) - для prod лучше задать
member.id — для Kafka (внутренняя идентификация в группе) (лучше пусть кафка за этим следит) (Confluent.Kafka сама обработает)
```csharp
var config = new ConsumerConfig
{
    BootstrapServers = "kafka:9092",
    GroupId = "order-processor",
    ClientId = "order-service-prod-01",  // ← твой ID, для логов и мониторинга
    SessionTimeoutMs = 45000,
    HeartbeatIntervalMs = 3000,
    AutoOffsetReset = AutoOffsetReset.Earliest
};
```
8. offsets.retention.minutes - это настройка Kafka, которая определяет, сколько времени хранятся offset'ы потребителей (позиции чтения) для неактивных consumer group'ов.
   Если consumer group не читал сообщения дольше этого времени — его offset'ы удаляются.
    По умолчанию 7 дней
9. кластер кафки - решает проблему падения ноды
   один лидер
   остальные фоллверы




## Шаги, которые происходят при поднятии консюмера
продюсер, шаги:
    1. Продюсер (именно клиентский код) определяет, в какую партицию писать partition = hash("order-123") % 3 → например, P1
    ```
    await producer.ProduceAsync("orders", new Message<string, string>
    {
        Key = "order-123",
        Value = "{...}"
    });
    ```
    2. продюсер находит лидера партиции P1, спрашивая любого брокера ("Кто лидер для orders/P1?" ). Брокер отвечает и эта инфа кэшируется в клиента
    3. Продюсер отправляет сообщение лидеру (broker-2)
        брокер:
        Записывает сообщение в свой локальный лог (на диск).
        Отправляет это сообщение фолловерам (broker-1 и broker-3) — они тоже записывают.
        Ждёт подтверждения от минимум min.insync.replicas брокеров (например, 2).
    


## Шаги, которые происходят при поднятии консюмер
консюмер, шаги:
    1. Консюмер узнаёт, кто лидер каждой партиции, которую он получил из assignment
    2. Консюмер читает только с лидера партиции



## Падения, ISR, HW, LEO (log end offset)
если лидер упал:
1. Координатор группы (Group Coordinator) замечает, что broker-2 не отвечает.
2. Запускается выбор нового лидера (leader election).
3. Например, broker-3 становится лидером P1.
4. Консюмер получает ошибку или зависает на poll().
5. Через некоторое время он переподключается и узнаёт нового лидера.
6. продолжает читать.

Фолловеры синхронизируются с лидером, выступая в роли обычных консюмеров.
Они опрашивают лидера (как консюмер), читают сообщения из его лога и записывают в свой локальный лог.
Этот процесс называется follower fetching или replica synchronization.
👉 Это не push, а pull — фолловер сам "забирает" данные у лидера.

Как часто фолловеры "опрашивают"?
Через настройку:
replica.fetch.wait.max.ms=500  # как часто фолловер спрашивает лидера
replica.fetch.min.bytes=1      # минимальный объём данных

ISR = In-Sync Replicas — это множество реплик, которые:
    Активны.
    Не отстают от лидера больше чем на replica.lag.time.max.ms (по умолч. 30 сек).
    Успевают за лидером.

Почему ISR важен?
Потому что:
При acks=all — сообщение считается записанным, только если его подтвердили все реплики из ISR.
При сбое лидера — новый лидер выбирается ТОЛЬКО из ISR.

acks=all и min.insync.replicas=2
Тогда новое сообщение вообще не будет записано, если ISR < min.insync.replicas.

Если лидер упал, а в ISR нет ни одного брокера — Kafka не выберет нового лидера, и партиция станет недоступной для записи и чтения.



и почему в ISR брокер попадает если не отстает от лидера больше чем на replica.lag.time.max.ms (по умолч. 30 сек). а если за это 
время было сообщение, брокер его не засинхрил еще но при этом считается ISR. если в этот момент лидер упадет, то мы потеряем навсегда 
это сообщение если выберем этого брокера как лидера

Как кафка это решает:
Нет, сообщение не будет потеряно, потому что:
    ISR — это не только про время, а про последний offset, до которого фолловер успел скопировать.
    Новое сообщение не считается "успешно записанным", пока его не подтвердят реплики из ISR.
    Если фолловер его ещё не получил — он не в ISR, или acks=all не даст подтвердить запись.

HW (High Watermark) — это последний offset, который гарантированно записан во всех ISR-репликах.


Да, когда продюсер отправляет сообщение с acks=all, брокер-лидер НЕ отправляет подтверждение (ack) до тех пор, пока:
    Сообщение будет записано локально на диске лидера.
    Оно будет реплицировано на все реплики, входящие в ISR (In-Sync Replicas).
    Их high watermark (HW) будет продвинут.


Лидер и фолловеры не используют "отдельный канал подтверждений" — вместо этого:

Фолловеры сами опрашивают лидера (pull-модель), запрашивая новые сообщения.
При ответе лидер включает текущий high watermark (HW).
Когда фолловер видит, что его LEO ≥ HW, он считает сообщение зафиксированным.
Лидер отслеживает HW каждой реплики и сам решает, когда продвинуть общий HW.
    👉 Это не push-подтверждение, а синхронизация через метаданные в обычных fetch-запросах.


сценарий: лидер медленно получает данные
t=0       | Фолловер: fetch(offset=100)
t=100ms   | Лидер: нет новых данных → ждёт до 500 мс (wait.max.ms)
t=500ms   | Лидер: отвечает "пусто" (timeout)
t=501ms   | Фолловер: fetch(offset=100) снова


request.timeout.ms - продюсер сколько ждет. ожидание происходит на стороне продюсера, а не на брокере.

возможна ситуация, когда продюсер отвалился по таймауту, а лидер все таки записал сообщение

Может ли сообщение быть зафиксировано после таймаута? -> ✅ Да , если фолловеры догонят

как исправить:
Разделять "инициирование" и "результат"

// Плохо: "если ошибка → считаем, что не запущено"
await producer.ProduceAsync("payments", event);
// Ошибка → fallback


await producer.ProduceAsync("payments", new PaymentInitiated { OrderId = 123 });

Используй event sourcing:
PaymentInitiated — факт попытки.
PaymentProcessed / PaymentFailed — результат.

Saga: никогда не принимай решения на основе "таймаута"
Как решить? → Разделяй ответ пользователю и судьбу события
❌ Плохо:
"Если ProduceAsync упал → показываем ошибку."

✅ Хорошо:
"Мы приняли запрос на оплату.
Результат будет чуть позже.
Проверяйте статус."

Правильный паттерн: "Fire and Forget" → нет. "Fire and Monitor" → да.


Подробно: что происходит при ProduceAsync

await producer.ProduceAsync("orders", message);

Когда EnableIdempotence = true, Confluent.Kafka делает следующее:

Получает producer id (PID) от Kafka
При первом вызове — библиотека отправляет InitProducerIdRequest.
Брокер выдаёт уникальный producer id (например, 12345).
Этот PID сохраняется внутри объекта IProducer.


dirty records или uncommitted messages сообщения в логе кафки, по которым не было получено acks и HW не сдвинут

Ты абсолютно прав в своей логике — и это один из самых опасных сценариев в Kafka!
То, что ты описал, называется "распухание uncommitted лога" или "отставание репликации при блокировке коммита", и если не контролировать, может привести к краху кластера.

Лидер сначала проверяет, достаточно ли реплик в ISR:
    Если ISR.size < min.insync.replicas → отклоняет запрос сразу.

ISR (In-Sync Replicas) — это набор реплик (включая лидера), которые:
    Активны (отвечают),
    Не сильно отстают по логу от лидера,
    Считаются достаточно синхронизированными, чтобы быть кандидатами на новое лидерство.
    💡 Только реплики из ISR участвуют в кворуме при acks=all.




# Confluent.Kafka Produce для максимальной производительности

Критерии:
    производительность
    пропускная способность
    high-throughput
    использует буферизацию
    Риск блокировки потока


Не используй await в цикле для отправки большого потока — лучше Produce + callback.

1. await producer.ProduceAsync
Хорошо работает в асинхронных веб-приложениях (ASP.NET Core).
Не блокирует поток.
Одно сообщение за раз — если использовать в цикле, будет низкая пропускная способность.
Не подходит для высокоскоростной пакетной отправки.
Каждый await — переключение контекста → накладные расходы.
Не используй в цикле для high-throughput сценариев — будет "последовательная задержка".

2. producer.ProduceAsync().ContinueWith(...)
Не блокирует выполнение.
Подходит для высокой пропускной способности.Можно отправлять много сообщений подряд без ожидания.
Лучше использует буферизацию Kafka (producer batch).
Нет автоматического контекстного переключения (например, в UI-поток).

3. producer.Produce(topic, message, callback) — Callback-подход
Самый производительный способ.
Нулевые накладные расходы от TPL.
Сообщения буферизуются и отправляются пакетами — максимальная пропускная способность.
Минимальное потребление памяти и CPU на стороне .NET.
Используется нативный механизм librdkafka (C++).
Callback выполняется в потоке delivery-таймера librdkafka → нельзя делать тяжёлые операции!


```
producer.Produce("topic", message, async (deliveryReport) =>
{
    if (deliveryReport.Error.IsError)
    {
    // Логируем ошибку, можно повторить или упасть
        Console.WriteLine($"Failed to deliver: {deliveryReport.Error.Reason}");
    }
    else
    {
        // ✅ Только здесь сообщение гарантированно в Kafka
        await MarkAsSentInDb(deliveryReport.Message.Key, deliveryReport.Offset);
    }
});
```

```csharp
producer.Produce("topic", message, deliveryReport =>
{
    if (!deliveryReport.Error.IsError)
    {
        // Запускаем асинхронную задачу "в фоне"
        _ = Task.Run(async () => 
        {
            try
            {
                await MarkAsSentInDbAsync(deliveryReport.Message.Key);
            }
            catch (Exception ex)
            {
                // Обязательно логируй ошибки, иначе пропадут
                Log.Error(ex, "Failed to mark outbox message as sent");
            }
        });
    }
});
```

producer.Flush(TimeSpan.FromSeconds(10)); - важно заиспользовать в конце, чтобы сообщения из буфера точно отправились в кафку и получились acks


# Confluent.Kafka Producer

consumer.StoreOffset() - Помечает offset как обработанный — сохраняет его в памяти консьюмера.
consumer.Commit() - Отправляет последний stored offset в Kafka (в топик__consumer_offsets) — фиксирует прогресс навсегда.

at least once
Graceful shutdown


```csharp
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public class HighPerformanceKafkaConsumer : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly ILogger<HighPerformanceKafkaConsumer> _logger;
    private readonly string _topic = "your-input-topic";
    
    // Для батчинга: накапливаем offsets, коммитим пачками
    private readonly ConcurrentQueue<TopicPartitionOffset> _offsetsToCommit = new();
    private const int CommitBatchSize = 100;
    private int _processedCount = 0;

    public HighPerformanceKafkaConsumer(ILogger<HighPerformanceKafkaConsumer> logger)
    {
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "high-perf-group",
            AutoOffsetReset = AutoOffsetReset.Latest,
            
            // Отключаем авто-коммит — управляем вручную
            EnableAutoCommit = false,
            
            // Производительность
            EnablePartitionEof = false,        // отключаем обработку EOF
            AllowAutoCreateTopics = true,      // опционально
            SessionTimeoutMs = 45000,
            MaxPollIntervalMs = 300000,        // до 5 мин без poll — ok (для long-running обработки)
            FetchWaitMaxMs = 500,              // быстрее poll
            FetchMaxBytesPerPartition = 1024 * 1024, // 1MB/партиция
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.Range
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Быстрое чтение с коротким таймаутом
                var consumeResult = _consumer.Consume(100); // 100ms timeout

                if (consumeResult is null) continue;

                if (consumeResult.IsPartitionEOF)
                {
                    _logger.LogDebug("EOF reached at {Offset}", consumeResult.Offset);
                    continue;
                }

                // 1. Обработка сообщения (может быть async)
                await ProcessMessageAsync(consumeResult.Message, stoppingToken);

                // 2. Помечаем offset как обработанный
                _consumer.StoreOffset(consumeResult);
                _offsetsToCommit.Enqueue(consumeResult.TopicPartitionOffset);
                _processedCount++;

                // 3. Периодический коммит
                if (_processedCount % CommitBatchSize == 0)
                {
                    CommitOffsets();
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consume error: {Error}", ex.Error.Reason);
                // Не храним offset → при перезапуске повторим
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error processing message");
                // При фатальной ошибке можно перебросить, чтобы перезапустить сервис
                throw;
            }
        }

        // На случай graceful shutdown — коммитим остатки
        CommitOffsets();
    }

    private async Task ProcessMessageAsync(Message<Ignore, string> message, CancellationToken ct)
    {
        try
        {
            // 🔥 Тут твоя бизнес-логика
            // Пример: обновление БД, отправка в другой топик, outbox и т.п.
            await Task.Delay(1, ct); // ← заменить на реальную обработку

            _logger.LogDebug("Processed message: {Value}", message.Value);
        }
        catch
        {
            // ❌ Не лови и не подавляй все исключения!
            // Если обработка упала — offset не stored → будет повтор
            throw;
        }
    }

    private void CommitOffsets()
    {
        if (_offsetsToCommit.IsEmpty) return;

        try
        {
            _consumer.Commit();
            _logger.LogDebug("Committed batch of offsets");
        }
        catch (KafkaException e)
        {
            _logger.LogError(e, "Commit failed: {Error}", e.Error.Reason);
            // Оставляем offsets в очереди? Или сбрасываем?
            // Зависит от стратегии. Здесь — просто логируем.
        }
    }

    public override void Dispose()
    {
        _logger.LogInformation("Flushing consumer before shutdown...");
        _consumer?.Flush(TimeSpan.FromSeconds(10));
        _consumer?.Dispose();
        base.Dispose();
    }
}
```


Почему нет ConsumeAsync:
    Consume() — это blocking poll, а не долгая операция
    Блокирует поток, пока не придет сообщение или не истечёт таймаут.
    Это не "работа", а ожидание (как queue.Take() или socket.Receive()).

Представим, что есть ConsumeAsync():
    Создаётся Task, который ждёт в фоновом потоке.
    Оригинальный поток освобождается.
    Когда приходит сообщение — Task завершается.

Но это не даёт выигрыша:
    Kafka-консьюмер всё равно должен где-то ждать (в потоке).
    Вместо одного потока — ты создаёшь Task + фоновый поток → накладные расходы.
    Нет реального параллелизма — Kafka не позволяет обрабатывать сообщения из одной партиции конкурентно.

Offset 100: { "id": 1 }           → IsPartitionEOF = false
Offset 101: { "id": 2 }           → IsPartitionEOF = false
Offset 102: null (EOF)            → IsPartitionEOF = true, Message = null
Offset 103: { "id": 3 } (новое)   → IsPartitionEOF = false


Советы по кафке:



Советы по различным настройкам:
min.insync.replicas
replication.factor=3, min.insync.replicas=2, acks=all - Сообщение не потеряется, если упадёт хотя бы 1 брокер
unclean.leader.election.enable=false - Никаких "неполных" лидеров, которые не входят в ISR → нет потери данных
enable.idempotence=true - Нет дублей при ретраях
max.in.flight.requests.per.connection=1 или idempotence=true - Упорядоченная доставка
replica.fetch.wait.max.ms - Максимальное время, которое лидер может ждать, пока наберётся replica.fetch.min.bytes
replica.fetch.min.bytes - Минимальный объём данных, который должен быть, чтобы лидер ответил (1 байт по умолчанию)
replica.fetch.max.bytes - макс. размер одного fetch запроса
num.replica.fetchers - количество фоновых потоков на фолловере, которые делают fetch (можно увеличить для параллелизма)
producer.id.expiration.ms



Надежная конфигура для кафки:
```csharp
var config = new ConsumerConfig
{
    BootstrapServers = "kafka:9092",
    GroupId = "email-service",              // ← имя группы

    // 🔽 Настройки offset
    EnableAutoCommit = false,               // ← отключаем авто-коммит
    AutoCommitIntervalMs = 5000,            // ← не используется, если auto-commit = false

    // 🔽 Надёжность
    AutoOffsetReset = AutoOffsetReset.Earliest, // Куда начать, если offset нет
    EnableAutoOffsetStore = false,          // Не сохранять offset до ручного Commit
};
```