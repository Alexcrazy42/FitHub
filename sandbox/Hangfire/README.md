# Hangfire.Jobs

Hangfire
Задачи
Storage под задачи
Воркеры, которые опрашивают хранилище и выполняют задачи
Дашбоард - просмотр джоб, ошибок, повторов и тд


HangFire.Job - основная информация о задаче: метод, аргументы, дата создания, состояние
HangFire.State - История состояний задачи: Enqueued, Processing, Succeeded, Failed, Scheduled и т.д.
HangFire.JobParameter - дополнительные параметры задачи (например, JobName, Description, TimeZone)
HangFire.JobQueue - Очереди задач (по умолчанию — default). Воркеры читают отсюда.
HangFire.Counter - счетчики производительности (например, количество выполненных джоб в минуту)
HangFire.AggregatedCounter - Агрегированные счётчики (для оптимизации)
HangFire.List - спользуется для хранения списков (например, recurring-jobs, succeeded, failed)
HangFire.Set - Множества (например, для recurring-jobs, monitoring)
HangFire.Hash - Хранилище пар ключ-значение (например, recurring jobs, сведения о серверах)
HangFire.Server - Информация о запущенных воркерах (имя, время последнего "пульса", количество потоков)

Минусы:
нельзя изменить крон без пересоздания джобы
приостановить recurring джобу нельзя - только удалить
Хранить параметры отдельно от кода - нельзя, только в захваченном замыкании
Иметь несколько расписаний на одну джобу с разными конфигами - нельзя
Динамически подгружать конфиг при запуске - только если не зашит в делегат

как hangfire запоминает куда передавать значение:
{
  "Type": "MyApp.Jobs.ReportJob, MyApp",
  "Method": "GenerateMonthlyReport",
  "ParameterTypes": "[\"System.String\", \"System.Int32\"]",
  "Arguments": "[\"\"\"Q1\"\"\", \"42\"]"
}