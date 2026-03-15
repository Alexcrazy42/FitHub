using FitHub.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.EmailNotifications;

public class EmailNotificationConfiguration : IEntityTypeConfiguration<EmailNotification>
{
    public void Configure(EntityTypeBuilder<EmailNotification> builder)
    {

    }
}
