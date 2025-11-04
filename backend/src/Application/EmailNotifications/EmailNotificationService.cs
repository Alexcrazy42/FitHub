using FitHub.Common.Entities.Storage;
using Microsoft.Extensions.Logging;

namespace FitHub.Application.EmailNotifications;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly IEmailNotificationRepository emailNotificationRepository;
    private readonly ILogger<EmailNotificationService> logger;
    private readonly IEmailService emailService;
    private readonly IUnitOfWork unitOfWork;

    public EmailNotificationService(
        IEmailNotificationRepository emailNotificationRepository,
        ILogger<EmailNotificationService> logger,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        this.emailNotificationRepository = emailNotificationRepository;
        this.logger = logger;
        this.emailService = emailService;
        this.unitOfWork = unitOfWork;
    }

    public async Task HandleFromWorker(CancellationToken ct)
    {
        var firstNotSent = await emailNotificationRepository.GetFirstNotSent(ct);
        if (firstNotSent == null)
        {
            return;
        }

        logger.LogInformation("Начинаем обрабатывать сообщение с id = {EmailNotifId}", firstNotSent.Id);

        var email = new Email([firstNotSent.ToEmail], firstNotSent.Subject, firstNotSent.BodyText, firstNotSent.BodyHtml);

        await emailService.SendEmailAsync(email, ct);

        firstNotSent.SetSentAt(DateTimeOffset.UtcNow);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
