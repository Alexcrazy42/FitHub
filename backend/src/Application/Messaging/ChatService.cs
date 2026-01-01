using FitHub.Application.Messaging.Commands;
using FitHub.Application.Users;
using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Messaging;
using FitHub.Shared.Messaging;

namespace FitHub.Application.Messaging;

// TODO: добавить метод добавления пользователя в чат (добавляем MessageView + ReadModel + сообщение в чат)

internal sealed class ChatService : IChatService
{
    private readonly IChatRepository chatRepository;
    private readonly IUserService userService;
    private readonly IUnitOfWork unitOfWork;
    private readonly ICurrentIdentityUserIdAccessor userIdAccessor;
    private readonly IMessageRepository messageRepository;
    private readonly IMessageAttachmentRepository messageAttachmentRepository;

    public ChatService(IChatRepository chatRepository,
        IUnitOfWork unitOfWork,
        ICurrentIdentityUserIdAccessor userIdAccessor,
        IUserService userService,
        IMessageRepository messageRepository,
        IMessageAttachmentRepository messageAttachmentRepository)
    {
        this.chatRepository = chatRepository;
        this.unitOfWork = unitOfWork;
        this.userIdAccessor = userIdAccessor;
        this.userService = userService;
        this.messageRepository = messageRepository;
        this.messageAttachmentRepository = messageAttachmentRepository;
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

        var chat = Chat.Create(command.Type);

        var users = await userService.GetUsersAsync(command.ParticipantUserIds, ct);

        var participants = users.Select(user => ChatParticipant.Create(user, chat)).ToList();

        chat.SetParticipants(participants, currentUserId);

        if (chat.Type == ChatType.Group)
        {
            chat.SetGroupName(currentUserId);

            var message = Message.Create(chat, "");
            var createGroupAttachment = MessageAttachment.CreateGroupCreatedAttachment(message);
            await messageRepository.PendingAddAsync(message, ct);
            await messageAttachmentRepository.PendingAddAsync(createGroupAttachment, ct);
        }

        await chatRepository.PendingAddAsync(chat, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return chat;
    }

    public Task InviteUserAsync(InitiatorAndTargetUserCommand command, CancellationToken ct)
    {
        // MessageView + ReadModel
        throw new NotImplementedException();
    }

    public Task ExcludeUserAsync(InitiatorAndTargetUserCommand command, CancellationToken ct)
    {
        // MessageView + ReadModel
        throw new NotImplementedException();
    }
}
