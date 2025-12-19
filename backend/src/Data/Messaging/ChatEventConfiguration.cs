using FitHub.Domain.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Messaging;

public class ChatEventConfiguration : IEntityTypeConfiguration<ChatEvent>
{
    public void Configure(EntityTypeBuilder<ChatEvent> builder)
    {
        builder.HasOne(x => x.Chat)
            .WithMany()
            .HasForeignKey(x => x.ChatId);

        builder.HasIndex(x => x.CreatedAt);
    }
}
