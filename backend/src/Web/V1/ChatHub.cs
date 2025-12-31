using FitHub.Common.AspNetCore;
using FitHub.Common.Utilities.System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FitHub.Web.V1;


/// <summary>
/// Методы, которые могут отправляться от сервера для клиента
/// </summary>
/// <remarks>
/// Примеры:
/// {"type":1,"target":"ReceiveMessage","arguments":["Pso76nwThXSiAz-gZY4MJw","string","2025-12-28T17:31:31.8206199Z"]}
/// {"type":1,"target":"UserConnected","arguments":[{"connectionId":"_7-osTGxlJLLMD6HaWjdCA","userId":null,"randomId":"313a60e1-a2aa-4875-818d-12003a91d9de"}]}
/// {"type":1,"target":"ReceiveMessage","arguments":["Bgiws9t004ulD21nT8T1GA","string from group","2025-12-28T17:57:43.125617Z"]}
/// </remarks>
public interface IChatHub
{
    Task CreateMessage(string user, string message, DateTime timestamp);
    Task UpdateMessage(string user, string message, DateTime timestamp);
    Task UserConnected(UserConnectedDto dto);
    Task UserDisconnected(string userId);
    Task MessageDeleted(string messageId);
    Task UserTyping(string user);
}

public class UserConnectedDto
{
    public string? ConnectionId { get; set; }

    public string? UserId { get; set; }

    public string? UserName { get; set; }
}

[Authorize]
public class ChatHub : Hub<IChatHub>
{
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;

        var userId = user?.GetUserId();
        var userName = user?.GetUsername();

        var dto = new UserConnectedDto()
        {
            ConnectionId = Context.ConnectionId,
            UserId = userId,
            UserName = userName,
        };

        // TODO: добавить человека в группы по всем его чатам
        // TODO: выставить пользователю флаг, что он онлайн
        // TODO: отправить всем релевантным пользователям (друзья) сообщение, что он онлайн

        await Clients.All.UserConnected(dto);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // TODO: выставить флаг, что он не онлайн
        // TODO: отправить всем релевантным пользователям (друзья) сообщение, что он стал офлайн

        var userId = Context.User?.GetUserId();
        await Clients.All.UserDisconnected(userId.Required());
        await base.OnDisconnectedAsync(exception);
    }

    // Методы, которые клиент может вызывать через WebSocket
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.CreateMessage(user, message, DateTime.UtcNow);
    }

    public async Task NotifyTyping(string user)
    {
        // TODO: это мы отправляем только в чат куда идет печать
        await Clients.Others.UserTyping(user);
    }

    //хотим добавиться к группе (надо ли?)
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var userName = Context.User?.GetUsername();

        await Clients.Group(groupName)
            .CreateMessage(userName.Required(), "Пользователь добавился", DateTime.Now);
        // Уведомить группу
        // await Clients.Group(groupName)
        //     .UserJoinedGroup(userName, groupName);
    }
}
