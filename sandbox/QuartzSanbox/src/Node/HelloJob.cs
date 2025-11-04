using System.Globalization;
using Quartz;

namespace Node;

public class HelloJob : IJob
{
    private readonly ILogger<HelloJob> logger;

    public HelloJob(ILogger<HelloJob> logger)
    {
        this.logger = logger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        var fireTime = context.FireTimeUtc.LocalDateTime.ToString(CultureInfo.InvariantCulture);
        var instanceId = Environment.GetEnvironmentVariable("NodeId") ?? "unknown";

        logger.LogInformation("[Node: {instanceId}] Job {jobName} executed at {fireTime}", instanceId, jobName, fireTime);
        return Task.CompletedTask;
    }
}