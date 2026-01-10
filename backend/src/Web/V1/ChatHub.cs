using FitHub.Application.Messaging;
using FitHub.Application.Users;
using FitHub.Common.AspNetCore;
using FitHub.Common.Utilities.System;
using FitHub.Contracts.V1.Messaging.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FitHub.Web.V1;


/// <summary>
/// Методы, которые могут отправляться от сервера к клиенту
/// </summary>
/// <remarks>
/// Примеры:
/// {"type":1,"target":"ReceiveMessage","arguments":["Pso76nwThXSiAz-gZY4MJw","string","2025-12-28T17:31:31.8206199Z"]}
/// {"type":1,"target":"UserConnected","arguments":[{"connectionId":"_7-osTGxlJLLMD6HaWjdCA","userId":null,"randomId":"313a60e1-a2aa-4875-818d-12003a91d9de"}]}
/// {"type":1,"target":"ReceiveMessage","arguments":["Bgiws9t004ulD21nT8T1GA","string from group","2025-12-28T17:57:43.125617Z"]}
/// </remarks>
public interface IChatHub
{
    Task CreateMessage(MessageResponse messageResponse);
    Task UpdateMessage(MessageResponse messageResponse);
    Task MessageDeleted(string chatId, string messageId);

    Task UserTyping(string userId, string userName, string chatId);
    Task UserStopTyping(string userId, string userName, string chatId);

    Task UserOnline(string userId);
    Task UserOffline(string userId, DateTimeOffset endOnlineAt);
}

public class UserConnectedDto
{
    public string? UserId { get; set; }

    public string? UserName { get; set; }
}

[Authorize]
public class ChatHub : Hub<IChatHub>
{
    private readonly ILogger<ChatHub> logger;
    private readonly IChatService chatService;
    private readonly IUserService userService;

    public ChatHub(ILogger<ChatHub> logger, IChatService chatService, IUserService userService)
    {
        this.logger = logger;
        this.chatService = chatService;
        this.userService = userService;
    }

    public override async Task OnConnectedAsync()
    {
        var user = Context.User;

        var userId = user.Required().GetParsedUserId();

        await userService.StartOnlineAt(userId, Context.ConnectionAborted);

        var userChats = await chatService.GetUserChatsAsync(userId, Context.ConnectionAborted);

        var chatUsers = userChats
            .SelectMany(x => x.Participants.Select(participant => participant.User).ToList())
            .DistinctBy(x => x.Id)
            .ToList();

        foreach (var chat in userChats)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chat.Id.ToString().GetChatGroupName());
        }

        foreach (var chatUser in chatUsers)
        {
            await Clients.User(chatUser.Id.ToString()).UserOnline(userId.ToString());
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = Context.User;

        var userId = user.Required().GetParsedUserId();

        var startOnlineAt = await userService.EndOnlineAt(userId, Context.ConnectionAborted);

        var userChats = await chatService.GetUserChatsAsync(userId, Context.ConnectionAborted);

        var chatUsers = userChats
            .SelectMany(x => x.Participants.Select(participant => participant.User).ToList())
            .DistinctBy(x => x.Id)
            .ToList();

        foreach (var chatUser in chatUsers)
        {
            await Clients.User(chatUser.Id.ToString()).UserOffline(userId.ToString(), startOnlineAt);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task NotifyTyping(string chatId)
    {
        var userId = Context.User?.GetUserId();
        var userName = Context.User?.GetUsername();
        await Clients.OthersInGroup(chatId.GetChatGroupName()).UserTyping(userId.Required(), userName.Required(), chatId);
    }

    public async Task NotifyStopTyping(string chatId)
    {
        var userId = Context.User?.GetUserId();
        var userName = Context.User?.GetUsername();
        await Clients.OthersInGroup(chatId.GetChatGroupName()).UserStopTyping(userId.Required(), userName.Required(), chatId);
    }

    // хотим добавиться к группе (надо ли, мы и так делаем это при подключении + каждый раз когда нас куда то добавляют)
    public async Task JoinChat(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.GetChatGroupName());
    }

    public async Task Heartbeat()
    {
        var userId = Context.User.Required().GetParsedUserId();
        // TODO: Добавить здесь логику по выставлению статуса в бд (онлайн + когда в последний раз отмечался онлайн)
        // TODO: добавить фоновую джобу на отключение (> 1 минуты не делал Heartbeat - оффлайн)
        // TODO: по каждой записи userId -> connectionId обновляешь время последнего heartbeat
        // TODO: добавить фоновую джобу на отключение connectionId (удаление из бд + исключений из групп) для тех кто не делал heartbeat давно

        await userService.StartOnlineAt(userId, Context.ConnectionAborted);

        await Task.Delay(100);
    }
}
