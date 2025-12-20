using FitHub.Domain.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitHub.Data.Messaging;

public class MessageAttachmentConfiguration : IEntityTypeConfiguration<MessageAttachment>
{
    public void Configure(EntityTypeBuilder<MessageAttachment> builder)
    {
        builder.HasOne(att => att.Message)
            .WithMany(m => m.Attachments)
            .HasForeignKey(att => att.MessageId);

        builder.Property(x => x.Data)
            .HasColumnType("jsonb");
    }
}
