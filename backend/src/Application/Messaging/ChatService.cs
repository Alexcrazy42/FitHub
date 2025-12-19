using FitHub.Application.Messaging.Commands;
using FitHub.Application.Users;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using FitHub.Domain.Messaging;
using FitHub.Shared.Messaging;

namespace FitHub.Application.Messaging;

internal sealed class ChatService : IChatService
{
    private readonly IChatRepository chatRepository;
    private readonly IChatParticipantRepository chatParticipantRepository;
    private readonly IUserRepository userRepository;
    private readonly IUnitOfWork unitOfWork;

    public ChatService(IChatRepository chatRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IChatParticipantRepository chatParticipantRepository)
    {
        this.chatRepository = chatRepository;
        this.userRepository = userRepository;
        this.unitOfWork = unitOfWork;
        this.chatParticipantRepository = chatParticipantRepository;
    }

    public async Task<Chat> GetChatAsync(ChatId id, CancellationToken ct)
    {
        var chat = await chatRepository.GetFirstOrDefaultAsync(x => x.Id == id, ct);
        NotFoundException.ThrowIfNull(chat, "Чат не найден!");
        return chat;
    }

    public async Task<Chat> CreateChatAsync(CreateChatCommand command, CancellationToken ct)
    {
        var chat = Chat.Create(command.Type);
        var users = await userRepository.GetUsersAsync(command.ParticipantUserIds, ct);

        foreach (var user in users)
        {
            var participant = ChatParticipant.Create(user, chat);
            await chatParticipantRepository.PendingAddAsync(participant, ct);
        }

        await chatRepository.PendingAddAsync(chat, ct);

        await unitOfWork.SaveChangesAsync(ct);

        return chat;
    }

}
