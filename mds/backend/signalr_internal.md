3. как SignalR поймет что этот же пользователь, если у него поменяется IP. SignalR увидит, если пользователь отключился, закроет его persistence connection. 

negogiate - доп http запрос, чтобы узнать доступные транспорты и получить connectionId. backplane - для pub-sub через redis, если разные пользователи одной группы находятся на разных серверах (через redis channels - push модель как rabbit)
примерный алгоритм:
- получить из channel сообщение 
- посмотреть есть ли подключенные клиенты к этой группе на этом сервере
- отправить всем кто есть

 keepalive probes (пакеты проверки живучести):
 SignalR использует свой PingPong, чтобы не ждать tcp_keepalive_time = 7200 sec (2 часа бездействия). и каждые 15 секунд отправляет пинги на клиентов



Group.Send сразу всем отправляет или асинхронно через Channel redis?

SignalR использует Kestrel WebSocket manager, который пишет в все сокеты одновременно:

csharp
// Псевдокод SignalR GroupDispatcher
public async Task SendToGroupAsync(string groupName, object[] args)
{
    var messageBytes = SerializeMessage(args); // 1ms
    
    // ✅ Параллельная отправка ВСЕМ сокетам!
    var writeTasks = _groupConnections[groupName]
        .Select(connectionId => 
            _webSocketManager.SendAsync(connectionId, messageBytes))
        .ToArray();
    
    // НЕ await каждую! Только ждем завершения ВСЕХ
    await Task.WhenAll(writeTasks); // 10ms max
}

// Псевдокод SignalR (упрощено)
public async Task SendToGroupAsync(string groupName, string method, params object[] args)
{
    // 1. Локальная доставка (мгновенно)
    await SendToLocalGroup(groupName, method, args);
    
    // 2. Асинхронная публикация в Redis (fire-and-forget)
    _ = Task.Run(async () =>
    {
        await _redis.PublishAsync($"signalr:group:{groupName}", message);
    }); // 🔥 НЕ await! Фоновая задача!
}




// Клиент reconnect после оффлайна
connection.onreconnected(async () => {
    // Загружаем пропущенные сообщения из API
    const missed = await fetch(`/api/messages?since=${lastMessageId}`);
    const history = await missed.json();
    displayHistory(history);
});


HTTP API = источник истины, SignalR = real-time уведомления.

Проблемы SignalR:
❌ Нет HTTP status codes (200/400/500)
❌ Нет транзакций (DB rollback)
❌ Network failure = потеря данных
❌ Сложно тестировать/мониторить
❌ Нет retry/idempotency
❌ Не кешируется CDN