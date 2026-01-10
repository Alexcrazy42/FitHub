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
            .HasForeignKey(att => att.MessageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Data)
            .HasColumnType("jsonb");

        builder.HasOne(m => m.CreatedBy)
            .WithMany()
            .HasForeignKey(m => m.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.UpdatedBy)
            .WithMany()
            .HasForeignKey(m => m.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
