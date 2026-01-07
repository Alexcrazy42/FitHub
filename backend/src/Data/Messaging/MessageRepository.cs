using FitHub.Application.Common;
using FitHub.Application.Messaging;
using FitHub.Application.Messaging.Queries;
using FitHub.Authentication;
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

    public Task<IReadOnlyList<Message>> GetMessagesAsync(GetMessagesQuery messagesQuery, PagedQuery paged, CancellationToken ct = default)
    {
        var dbQuery = ReadRaw()
            .Include(x => x.Attachments)
            .Include(x => x.Views)
                .ThenInclude(view => view.User)
            .Where(x => x.ChatId == messagesQuery.ChatId)
            .Take(paged.PageSize);

        dbQuery = messagesQuery.IsDescending ?
            dbQuery.OrderByDescending(x => x.CreatedAt)
                .Where(x => x.CreatedAt <= messagesQuery.From)
            :
            dbQuery.OrderBy(x => x.CreatedAt)
                .Where(x => x.CreatedAt >= messagesQuery.From);

        return dbQuery.ToReadOnlyListAsync(ct);
    }

    public Task<IReadOnlyList<Message>> GetUnreadMessagesOlderThan(
        Message message,
        IdentityUserId userId,
        CancellationToken ct = default)
    {
        // специально берем все сообщения (даже те, что удалили), чтобы их тоже можно было прочитать
        return DbSet
            .Include(x => x.Chat)
            .Where(m => m.ChatId == message.ChatId)
            .Where(m => m.CreatedAt <= message.CreatedAt)
            .Where(m => m.CreatedById != userId)
            .Where(m => Context.MessageView.Any(v =>
                v.MessageId == m.Id &&
                v.UserId == userId &&
                v.ViewedAt == null))
            .ToReadOnlyListAsync(ct);
    }
}
