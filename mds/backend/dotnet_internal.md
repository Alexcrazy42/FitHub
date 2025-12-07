Что такое AsyncLocal<T>
AsyncLocal<T> — это специальный тип хранилища, который:
Привязан к ExecutionContext, а не к конкретному потоку
Копируется автоматически при каждом async/await
Изолирован между разными async 


```
class ExecutionContext
{
    // Каждый async flow имеет свою копию словаря
    Dictionary<AsyncLocal, object> asyncLocalValues;
    
    // При await Task создаётся копия контекста
    public ExecutionContext Clone()
    {
        return new ExecutionContext 
        {
            asyncLocalValues = new Dictionary(this.asyncLocalValues)
        };
    }
}

class AsyncLocal<T>
{
    public T Value
    {
        get 
        {
            var context = ExecutionContext.Current;
            return (T)context.asyncLocalValues[this];
        }
        set
        {
            var context = ExecutionContext.Current;
            context.asyncLocalValues[this] = value;
        }
    }
}
```

Сценарий 1. Один async flow

```
public async Task ProcessOrderA()
{
    using var activity = Activity.StartActivity("ProcessOrderA");
    activity.SetTag("order.id", "A");
    
    // Activity.Current = activity (записано в ExecutionContext потока #1)
    
    await Task.Delay(100); 
    // await сохраняет ExecutionContext и "прикрепляет" к continuation
    
    // Continuation может выполниться на потоке #5 из thread pool
    // НО ExecutionContext восстанавливается автоматически!
    // Activity.Current всё ещё = activity (тот же ExecutionContext)
    
    Console.WriteLine(Activity.Current.Tags["order.id"]); // "A"
}
```

Что происходит под капотом при await:
```
// Компилятор C# генерирует примерно такой код:
var awaiter = Task.Delay(100).GetAwaiter();
if (!awaiter.IsCompleted)   
{
    // Захватываем текущий ExecutionContext
    var capturedContext = ExecutionContext.Capture();
    
    awaiter.OnCompleted(() =>
    {
        // Восстанавливаем ExecutionContext перед continuation
        ExecutionContext.Run(capturedContext, _ =>
        {
            // Тут Activity.Current доступен!
            Console.WriteLine(Activity.Current.Tags["order.id"]);
        }, null);
    });
}
```


Сценарий 2: Параллельные async flow не путаются

```
public async Task ProcessOrders()
{
    // Запускаем параллельно два заказа
    var task1 = Task.Run(async () =>
    {
        using var activity = Activity.StartActivity("OrderA");
        activity.SetTag("order.id", "A");
        
        await Task.Delay(100);
        
        // Даже если выполняется на том же потоке что и task2,
        // Activity.Current будет правильный
        Console.WriteLine(Activity.Current.Tags["order.id"]); // "A"
    });
    
    var task2 = Task.Run(async () =>
    {
        using var activity = Activity.StartActivity("OrderB");
        activity.SetTag("order.id", "B");
        
        await Task.Delay(100);
        
        Console.WriteLine(Activity.Current.Tags["order.id"]); // "B"
    });
    
    await Task.WhenAll(task1, task2);
}
```

при этом для такого кейса будет два разных контекста
Task.Run по умолчанию НЕ наследует ExecutionContext родителя полностью, что приводит к созданию нового TraceId. Вот как это обойти:

```
var task1 = Task.Run(async () =>
{
    // ExecutionContext#1 создан для этого Task
    using var activity = Activity.StartActivity("OrderA");
    activity.SetTag("order.id", "A");
    
    // Выполняется на Thread #3
    Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}"); // 3
    Console.WriteLine($"Activity: {Activity.Current.Tags["order.id"]}"); // A
    
    await Task.Delay(100);
    
    // Continuation может быть на Thread #7
    Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}"); // 7
    Console.WriteLine($"Activity: {Activity.Current.Tags["order.id"]}"); // ВСЕГДА A!
});

var task2 = Task.Run(async () =>
{
    // ExecutionContext#2 создан для этого Task (независимо!)
    using var activity = Activity.StartActivity("OrderB");
    activity.SetTag("order.id", "B");
    
    // Выполняется на Thread #5
    Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}"); // 5
    Console.WriteLine($"Activity: {Activity.Current.Tags["order.id"]}"); // B
    
    await Task.Delay(100);
    
    // Continuation может быть на Thread #3 (да, том же что был у task1!)
    Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}"); // 3
    Console.WriteLine($"Activity: {Activity.Current.Tags["order.id"]}"); // ВСЕГДА B!
});

await Task.WhenAll(task1, task2);
```


Решение 1. Использовать одну Task с WhenAll

```
using var parentActivity = Activity.StartActivity("Parent");

// ✅ await сохраняет ExecutionContext
await Task.WhenAll(
    ProcessOrder1Async(), // Наследует ExecutionContext
    ProcessOrder2Async()  // Наследует ExecutionContext
);

async Task ProcessOrder1Async()
{
    using var childActivity = Activity.StartActivity("Child1");
    // childActivity.ParentId = parentActivity.SpanId ✅
    // childActivity.TraceId = parentActivity.TraceId ✅
    await Task.Delay(100);
}

async Task ProcessOrder2Async()
{
    using var childActivity = Activity.StartActivity("Child2");
    // childActivity.ParentId = parentActivity.SpanId ✅
    // childActivity.TraceId = parentActivity.TraceId ✅
    await Task.Delay(100);
}
```

Решение 2. Явно передать parent Activity в Task.Run

```
using var parentActivity = Activity.StartActivity("Parent");

// Захватываем родительский Activity ДО Task.Run
var parentForTask = Activity.Current;

var task1 = Task.Run(async () =>
{
    // Явно создаём child с правильным parent
    using var childActivity = activitySource.StartActivity(
        "Child1",
        ActivityKind.Internal,
        parentForTask?.Context ?? default // ✅ Явный parent!
    );
    
    // Теперь TraceId унаследуется от parent
    childActivity.TraceId == parentActivity.TraceId // true!
    
    await Task.Delay(100);
});

var task2 = Task.Run(async () =>
{
    using var childActivity = activitySource.StartActivity(
        "Child2",
        ActivityKind.Internal,
        parentForTask?.Context ?? default
    );
    
    childActivity.TraceId == parentActivity.TraceId // true!
    
    await Task.Delay(100);
});

await Task.WhenAll(task1, task2);
```



Почему не путаются?

Task.Run создаёт новый ExecutionContext для каждого Task
Каждый ExecutionContext имеет свою копию AsyncLocal значений
Даже если оба Task выполняются на одном физическом потоке поочерёдно, ExecutionContext переключается вместе с Task

```
Thread Pool Thread #3:
  [время 0-50ms]  ExecutionContext для task1 (Activity = "OrderA")
  [время 50-100ms] ExecutionContext для task2 (Activity = "OrderB")
  [время 100-150ms] ExecutionContext для task1 (Activity = "OrderA")
```


AsyncLocal не бесплатен:
Каждый await копирует ExecutionContext (shallow copy словаря)
Запись в AsyncLocal создаёт новую копию словаря (copy-on-write)
Это быстро (наносекунды), но не нулевые накладные расходы

Именно поэтому .NET Core оптимизирует:
ExecutionContext копируется только если есть AsyncLocal значения
Используется structural sharing для словарей





Chunked Transfer Encoding — если сервер использует Transfer-Encoding: chunked, данные передаются частями
Content-Length — сервер знает размер заранее, отправляет заголовок Content-Length: 50000. Клиент знает, сколько байт ожидать.
Transfer-Encoding: chunked — размер неизвестен, данные генерируются динамически (например, real-time stream). Сервер отправляет данные частями, пока не пришлет финальный chunk размером 0



var bodyString = await response.Content.ReadAsStringAsync();

Внутри этот метод:
Читает весь stream до конца из TCP-соединения
Загружает все байты в память (аналог ReadAsByteArrayAsync())
Применяет декодирование на основе Content-Type header (обычно UTF-8)
Конвертирует byte[] → string

1. Streams +
2. Транзакции в бд vs ChangeTracker +
3. Загрузка видео - PresignedUrl для скачивания, все мимо бэкенда, только метаданные. upload bar может спокойно себе позволить фронтенд сделать. тк он знает сколько загрузилось, а сколько нет +
4. динамический json
