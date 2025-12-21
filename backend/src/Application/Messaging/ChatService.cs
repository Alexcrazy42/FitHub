using FitHub.Application.Messaging.Commands;
using FitHub.Application.Users;
using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Messaging;
using FitHub.Shared.Messaging;

namespace FitHub.Application.Messaging;

internal sealed class ChatService : IChatService
{
    private readonly IChatRepository chatRepository;
    private readonly IChatParticipantRepository chatParticipantRepository;
    private readonly IUserService userService;
    private readonly IUnitOfWork unitOfWork;
    private readonly ICurrentIdentityUserIdAccessor userIdAccessor;

    public ChatService(IChatRepository chatRepository,
        IUnitOfWork unitOfWork,
        IChatParticipantRepository chatParticipantRepository,
        ICurrentIdentityUserIdAccessor userIdAccessor,
        IUserService userService)
    {
        this.chatRepository = chatRepository;
        this.unitOfWork = unitOfWork;
        this.chatParticipantRepository = chatParticipantRepository;
        this.userIdAccessor = userIdAccessor;
        this.userService = userService;
    }


    public async Task<Chat> GetChatAsync(ChatId id, CancellationToken ct)
    {
        var chat = await chatRepository.GetAsync(id, ct);
        chat.CheckAccess(userIdAccessor.GetCurrentUserId());
        return chat;
    }

    public async Task<Chat> CreateChatAsync(CreateChatCommand command, CancellationToken ct)
    {

        if (command.Type == ChatType.OneToOne)
        {
            var possibleChat = await chatRepository.GetFirstOrDefaultOneToOneChatAsync(command.ParticipantUserIds, ct);

            if (possibleChat != null)
            {
                return possibleChat;
            }
        }

        var currentUserId = userIdAccessor.GetCurrentUserId();

        if (!command.ParticipantUserIds.Contains(currentUserId))
        {
            throw new ValidationException("Вы не можете создать чат без себя!");
        }

        var chat = Chat.Create(command.Type);

        var users = await userService.GetUsersAsync(command.ParticipantUserIds, ct);

        if (chat.Type == ChatType.Group)
        {
            var groupChatName = GetGroupChatName(users, currentUserId);
            chat.SetName(groupChatName);
        }

        foreach (var user in users)
        {
            var participant = ChatParticipant.Create(user, chat);
            await chatParticipantRepository.PendingAddAsync(participant, ct);
        }

        await chatRepository.PendingAddAsync(chat, ct);

        await unitOfWork.SaveChangesAsync(ct);

        return chat;
    }

    private string GetGroupChatName(IReadOnlyList<User> participants, IdentityUserId creatorId)
    {
        if (participants.Count == 0)
        {
            throw new ValidationException("Количество участников не может быть равным 0!");
        }

        const int maxNamesInTitle = 2;

        var sortedParticipants = participants
            .OrderByDescending(x => x.Id == creatorId) // true (1) идёт первым
            .ThenBy(x => x.Name)
            .ToList();

        var displayedNames = sortedParticipants
            .Take(maxNamesInTitle)
            .Select(x => x.Name);

        var chatName = String.Join(", ", displayedNames);

        var remainingCount = participants.Count - maxNamesInTitle;
        if (remainingCount > 0)
        {
            chatName += $" + {remainingCount}";
        }

        return chatName;
    }
}
