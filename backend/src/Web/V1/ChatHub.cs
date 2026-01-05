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
    Task SimpleCreateMessage(string user, string message, DateTime timestamp);
    Task SimpleUpdateMessage(string user, string message, DateTime timestamp);

    Task CreateMessage(MessageResponse messageResponse);
    Task UpdateMessage(MessageResponse messageResponse);
    Task MessageDeleted(string chatId, string messageId);

    Task UserTyping(string user, string chatId);

    Task UserConnected(UserConnectedDto dto);
    Task UserDisconnected(string userId);
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

    public ChatHub(ILogger<ChatHub> logger)
    {
        this.logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var user = Context.User;

        var userId = user?.GetUserId();
        var userName = user?.GetUsername();

        var dto = new UserConnectedDto()
        {
            UserId = userId,
            UserName = userName,
        };

        // TODO: добавить человека в группы по всем его чатам
        // TODO: выставить пользователю флаг, что он онлайн
        // TODO: отправить всем релевантным пользователям (друзья/у кого он есть в чате общем) сообщение, что он онлайн

        await Clients.All.UserConnected(dto);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // TODO: выставить флаг, что он не онлайн
        // TODO: отправить всем релевантным пользователям (друзья/у кого он есть в чате общем) сообщение, что он офлайн

        var userId = Context.User?.GetUserId();
        await Clients.All.UserDisconnected(userId.Required());
        await base.OnDisconnectedAsync(exception);
    }

    public async Task NotifyTyping(string chatId)
    {
        var userId = Context.User?.GetUserId();
        logger.LogInformation("User {userId} typing", userId);
        await Clients.OthersInGroup(chatId.GetChatGroupName()).UserTyping(userId.Required(), chatId);
    }

    //хотим добавиться к группе (надо ли, мы и так делаем это при подключении + каждый раз когда нас куда то добавляют)
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var userName = Context.User?.GetUsername();

        await Clients.Group(groupName)
            .SimpleCreateMessage(userName.Required(), "Пользователь добавился", DateTime.Now);
    }

    public async Task Heartbeat()
    {
        var userId = Context.User?.GetUserId();
        // TODO: Добавить здесь логику по выставлению статуса в бд (онлайн + когда в последний раз отмечался онлайн)
        // TODO: добавить фоновую джобу на отключение (> 1 минуты не делал Heartbeat - оффлайн)

        await Clients.User(userId.Required())
            .SimpleCreateMessage(userId, "Heartbeat", DateTime.Now);
    }
}
