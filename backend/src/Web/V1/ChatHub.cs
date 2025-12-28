using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FitHub.Web.V1;


/// <summary>
/// Методы, которые могут отправляться от сервера для клиента
/// </summary>
/// <remarks>
/// Примеры:
/// {"type":1,"target":"ReceiveMessage","arguments":["Pso76nwThXSiAz-gZY4MJw","string","2025-12-28T17:31:31.8206199Z"]}
/// {"type":1,"target":"UserConnected","arguments":["Pso76nwThXSiAz-gZY4MJw"]}
/// {"type":1,"target":"ReceiveMessage","arguments":["Bgiws9t004ulD21nT8T1GA","string from group","2025-12-28T17:57:43.125617Z"]}
/// </remarks>
public interface IChatHub
{
    Task ReceiveMessage(string user, string message, DateTime timestamp);
    Task UserConnected(string connectionId);
    Task UserDisconnected(string connectionId);
    Task MessageDeleted(string messageId);
    Task UserTyping(string user);
}

[Authorize]
public class ChatHub : Hub<IChatHub>
{
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;


        await Clients.All.UserConnected(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.All.UserDisconnected(Context.ConnectionId); // событие может использоваться для онлайна
        await base.OnDisconnectedAsync(exception);
    }

    // Методы, которые клиент может вызывать через WebSocket
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.ReceiveMessage(user, message, DateTime.UtcNow);
    }

    public async Task NotifyTyping(string user)
    {
        await Clients.Others.UserTyping(user);
    }
}
