using FitHub.Domain.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Messaging;

public class MessageViewConfiguration : IEntityTypeConfiguration<MessageView>
{
    public void Configure(EntityTypeBuilder<MessageView> builder)
    {
        builder.HasOne(view => view.User)
            .WithMany()
            .HasForeignKey(view => view.UserId);

        builder.HasOne(view => view.Message)
            .WithMany(msg => msg.Views)
            .HasForeignKey(view => view.MessageId);

        builder.HasIndex(x => x.MessageId);
    }
}
