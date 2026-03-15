namespace FitHub.Application.EmailNotifications;

public interface IEmailNotificationService
{
    Task HandleFromWorker(CancellationToken ct);
}
