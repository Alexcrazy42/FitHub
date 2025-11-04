# OpenTelemetry + Jaeger

OTLP протокол



- "16686:16686" # Jaeger UI
- "6831:6831/udp" # Jaeger agent UDP port
- "5778:5778" # Jaeger agent HTTP port
- "14268:14268" # Jaeger collector HTTP port

COLLECTOR_OTLP_ENABLED=true



Activity

Activity.Current.Id берётся из async-локального контекста, который устанавливается либо автоматически (инструментацией), либо вручную (через StartActivity). 
Он "путешествует" по async-цепочке и позволяет создавать вложенные спаны, которые потом отправляются в Jaeger как часть одного распределённого трейса. 

Когда ты используешь async/await, .NET создаёт ExecutionContext — объект, который "путешествует" вместе с Task.
Внутри ExecutionContext хранятся:
SecurityContext
CultureInfo
... и AsyncLocal<T> значения!
→ При await — этот контекст захватывается и восстанавливается после возобновления.
→ Именно поэтому Activity.Current (который внутри использует AsyncLocal<Activity>) сохраняется между await.


🧩 Как Activity.Current использует AsyncLocal?
Внутри System.Diagnostics.Activity есть:

csharp

private static readonly AsyncLocal<Activity> s_current = new();
→ Когда ты вызываешь activity.Start() → s_current.Value = activity
→ Когда вызываешь activity.Stop() → s_current.Value = previousActivity (или null)

→ Именно поэтому Activity.Current работает "магически" — он просто читает из AsyncLocal<Activity>.