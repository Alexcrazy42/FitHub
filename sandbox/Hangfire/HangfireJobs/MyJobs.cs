namespace Hangfire;

public static class MyJobs
{
    public static void DoWork()
    {
        var nodeId = Environment.GetEnvironmentVariable("NODE_ID") ?? "unknown";
        Console.WriteLine($"[Job] Выполняется на ноде {nodeId} в {DateTime.Now}");
    }
}

