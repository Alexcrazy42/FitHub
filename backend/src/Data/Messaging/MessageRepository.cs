using FitHub.Application.Common;
using FitHub.Application.Messaging;
using FitHub.Common.Entities;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Messaging;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Data.Messaging;

internal sealed class MessageRepository : DefaultPendingRepository<Message, MessageId, DataContext>, IMessageRepository
{
    public MessageRepository(DataContext context) : base(context)
    {
    }

    protected override IQueryable<Message> ReadRaw()
    {
        return DbSet
            .Include(x => x.Chat)
            .Where(x => x.DeletedAt == null)
            .Include(x => x.CreatedBy)
            .AsSplitQuery();
    }

    public async Task<Message> GetMessageAsync(MessageId id, CancellationToken ct = default)
    {
        var message = await ReadRaw()
            .Include(x => x.Attachments)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        NotFoundException.ThrowIfNull(message, "Сообщение не найдено!");

        return message;
    }

    public Task<IReadOnlyList<Message>> GetMessagesAsync(ChatId chatId, PagedQuery paged, CancellationToken ct = default)
    {
        return ReadRaw()
            .Include(x => x.Attachments)
            .Where(x => x.ChatId == chatId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((paged.PageNumber - 1) * paged.PageSize)
            .Take(paged.PageSize)
            .ToReadOnlyListAsync(ct);
    }
}
