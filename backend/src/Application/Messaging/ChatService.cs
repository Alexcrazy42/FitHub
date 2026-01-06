using FitHub.Application.Messaging.Commands;
using FitHub.Application.Users;
using FitHub.Authentication;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Messaging;
using FitHub.Domain.Messaging.Attachments;
using FitHub.Shared.Messaging;

namespace FitHub.Application.Messaging;

internal sealed class ChatService : IChatService
{
    private readonly IChatRepository chatRepository;
    private readonly IUserService userService;
    private readonly IUnitOfWork unitOfWork;
    private readonly ICurrentIdentityUserIdAccessor userIdAccessor;
    private readonly IMessageRepository messageRepository;
    private readonly IMessageAttachmentRepository messageAttachmentRepository;
    private readonly IMessageViewRepository messageViewRepository;
    private readonly IChatReadingModelRepository chatReadingModelRepository;

    public ChatService(IChatRepository chatRepository,
        IUnitOfWork unitOfWork,
        ICurrentIdentityUserIdAccessor userIdAccessor,
        IUserService userService,
        IMessageRepository messageRepository,
        IMessageAttachmentRepository messageAttachmentRepository,
        IMessageViewRepository messageViewRepository,
        IChatReadingModelRepository chatReadingModelRepository)
    {
        this.chatRepository = chatRepository;
        this.unitOfWork = unitOfWork;
        this.userIdAccessor = userIdAccessor;
        this.userService = userService;
        this.messageRepository = messageRepository;
        this.messageAttachmentRepository = messageAttachmentRepository;
        this.messageViewRepository = messageViewRepository;
        this.chatReadingModelRepository = chatReadingModelRepository;
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
        var creator = users.First(x => x.Id == currentUserId);

        var participants = users.Select(user => ChatParticipant.Create(user, chat)).ToList();
        var message = Message.Create(chat, "");

        var messageView = MessageView.Create(message, creator);

        chat.SetParticipants(participants, currentUserId);

        if (chat.Type == ChatType.Group)
        {
            chat.SetGroupName(currentUserId);
            var createGroupAttachment = MessageAttachment.CreateGroupCreatedAttachment(message);
            await messageAttachmentRepository.PendingAddAsync(createGroupAttachment, ct);


            foreach (var user in chat.Participants.Select(x => x.User).Where(x => x.Id != currentUserId).ToList())
            {
                var chatReadModel = ChatReadingModel.Create(chat, user, message, message, 1);
                await chatReadingModelRepository.PendingAddAsync(chatReadModel, ct);
            }

            var currentUserReadModel = ChatReadingModel.Create(chat, creator, message, message, 0);
            await chatReadingModelRepository.PendingAddAsync(currentUserReadModel, ct);
        }

        await messageRepository.PendingAddAsync(message, ct);
        await chatRepository.PendingAddAsync(chat, ct);
        await messageViewRepository.PendingAddAsync(messageView, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return chat;
    }

    public async Task<Message> InviteUserAsync(InitiatorAndTargetUserCommand command, CancellationToken ct)
    {
        var initiator = await userService.GetUserAsync(command.InitiatorUserId, ct);
        var invitingUser = await userService.GetUserAsync(command.TargetUserId, ct);

        var chat = await GetChatAsync(command.ChatId, ct);

        if (chat.Participants.Any(x => x.UserId == command.TargetUserId))
        {
            throw new ValidationException("Этот пользователь уже является участником чата!");
        }

        var chatParticipant = ChatParticipant.Create(invitingUser, chat);
        chat.AddParticipant(chatParticipant);


        var message = Message.Create(chat, "");
        var attachmentPayload =
            new InitiatorAndTargetUserActionAttachment(command.InitiatorUserId, initiator.GetFullName(), command.InitiatorUserId, invitingUser.GetFullName());
        var inviteAttachment = MessageAttachment.CreateInviteOrExcludeUserAttachment(message, MessageAttachmentType.InviteUser, attachmentPayload);

        foreach (var participant in chat.Participants)
        {
            var newMessageView = MessageView.Create(message, participant.User);
            await messageViewRepository.PendingAddAsync(newMessageView, ct);
        }

        var chatReadModel = ChatReadingModel.Create(chat, invitingUser, message, message, 1);

        var participantUserIds = chat.Participants.Select(x => x.UserId).ToList();
        var chatReadModels = await chatReadingModelRepository.GetAllAsync(x => x.ChatId == chat.Id && participantUserIds.Contains(x.UserId), ct);
        foreach (var readModel in chatReadModels)
        {
            readModel.UpdateLastMessageAndIncrement(message);
        }

        await messageRepository.PendingAddAsync(message, ct);
        await messageAttachmentRepository.PendingAddAsync(inviteAttachment, ct);
        await chatReadingModelRepository.PendingAddAsync(chatReadModel, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return message;
    }

    public async Task<Message> ExcludeUserAsync(InitiatorAndTargetUserCommand command, CancellationToken ct)
    {
        var initiator = await userService.GetUserAsync(command.InitiatorUserId, ct);
        var invitingUser = await userService.GetUserAsync(command.TargetUserId, ct);

        var chat = await GetChatAsync(command.ChatId, ct);

        if (!chat.Participants.Any(x => x.UserId == command.TargetUserId))
        {
            throw new ValidationException("Этот пользователь не является участником чата!");
        }

        var message = Message.Create(chat, "");
        var attachmentPayload =
            new InitiatorAndTargetUserActionAttachment(command.InitiatorUserId, initiator.GetFullName(), command.InitiatorUserId, invitingUser.GetFullName());
        var inviteAttachment = MessageAttachment.CreateInviteOrExcludeUserAttachment(message, MessageAttachmentType.ExcludeUser, attachmentPayload);
        foreach (var participant in chat.Participants)
        {
            var newMessageView = MessageView.Create(message, participant.User);
            await messageViewRepository.PendingAddAsync(newMessageView, ct);
        }

        var chatReadModel = await chatReadingModelRepository.GetFirstOrDefaultAsync(x => x.ChatId == chat.Id && x.UserId == command.TargetUserId, ct);
        if (chatReadModel == null)
        {
            throw new UnexpectedException($"Не смогли найти ChatReadingModel для пользователя {command.TargetUserId} и чата {chat.Id}");
        }
        chatReadModel.UpdateLastMessageAndIncrement(message);

        var chatParticipant = chat.Participants.First(x => x.UserId == command.TargetUserId);
        chat.DisableChatParticipant(chatParticipant);

        await messageRepository.PendingAddAsync(message, ct);
        await messageAttachmentRepository.PendingAddAsync(inviteAttachment, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return message;
    }

    public Task<IReadOnlyList<Chat>> GetUserChatsAsync(IdentityUserId userId, CancellationToken ct)
    {
        return chatRepository.GetUserChatsAsync(userId, ct);
    }
}
