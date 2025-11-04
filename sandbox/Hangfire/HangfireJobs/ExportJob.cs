namespace Hangfire;

public class ExportJob
{
    private readonly ILogger<ExportJob> logger;

    public ExportJob(ILogger<ExportJob> logger) => this.logger = logger;

    public async Task ProcessAsync(ExportCommand command)
    {
        logger.LogInformation("Экспорт для пользователя {UserId}, формат: {Format}",
            command.UserId, command.Format);

        await Task.Delay(1000);

        if (command.Notify && !string.IsNullOrEmpty(command.Email))
        {
            logger.LogInformation("Уведомление отправлено на {Email}", command.Email);
        }
    }
}

public class ExportCommand
{
    public int UserId { get; set; }
    public string Format { get; set; } = "pdf";
    public string? Email { get; set; }
    public bool Notify { get; set; } = true;
}
