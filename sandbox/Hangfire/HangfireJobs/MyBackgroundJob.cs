namespace Hangfire;

public class MyBackgroundJob
{
    private readonly ILogger<MyBackgroundJob> logger;

    public MyBackgroundJob(ILogger<MyBackgroundJob> logger)
    {
        this.logger = logger;
    }

    [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task RunAsync()
    {
        var nodeId = Environment.GetEnvironmentVariable("NODE_ID") ?? "unknown";
        var executionId = Guid.NewGuid(); // чтобы отличать попытки

        logger.LogInformation("[Job Start] Выполняется на ноде {NodeId}, ID: {ExecutionId} в {Time}",
            nodeId, executionId, DateTime.Now);


        var random = new Random();
        if (random.Next(0, 2) == 0)
        {
            logger.LogWarning("[Job Fail] Искусственная ошибка на ноде {NodeId}, ID: {ExecutionId}",
                nodeId, executionId);

            throw new InvalidOperationException("Случайная ошибка: обработка не удалась");
        }

        await Task.Delay(1000);

        logger.LogInformation("[Job Done] Успешно завершено на ноде {NodeId}, ID: {ExecutionId}",
            nodeId, executionId);
    }
}
