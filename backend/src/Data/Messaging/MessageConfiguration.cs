using FitHub.Domain.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Messaging;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasMany(msg => msg.Attachments)
            .WithOne(att => att.Message)
            .HasForeignKey(att => att.MessageId);

        builder.HasOne(msg => msg.Chat)
            .WithMany()
            .HasForeignKey(msg => msg.ChatId);

        builder.HasOne(msg => msg.ReplyMessage)
            .WithMany(msg => msg.RepliedMessages)
            .HasForeignKey(msg => msg.ReplyMessageId);

        builder.HasOne(msg => msg.ForwardedMessage)
            .WithMany()
            .HasForeignKey(msg => msg.ForwardedMessageId);
    }
}
