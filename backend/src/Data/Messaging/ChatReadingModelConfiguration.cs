using FitHub.Domain.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Messaging;

public class ChatReadingModelConfiguration : IEntityTypeConfiguration<ChatReadingModel>
{
    public void Configure(EntityTypeBuilder<ChatReadingModel> builder)
    {
        builder.HasOne(x => x.Chat)
            .WithMany()
            .HasForeignKey(x => x.ChatId);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.LastMessage)
            .WithMany()
            .HasForeignKey(x => x.LastMessageId);

        builder.HasIndex(x => new { x.ChatId, x.UserId });
    }
}
