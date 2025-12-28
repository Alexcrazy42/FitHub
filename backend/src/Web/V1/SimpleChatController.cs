using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FitHub.Web.V1;


public record SendMessageRequest(string User, string Message);
public record SendToUserRequest(string ConnectionId, string User, string Message);
public record SendToGroupRequest(string GroupName, string User, string Message);
public record JoinGroupRequest(string ConnectionId, string GroupName);

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AChatController : ControllerBase
{
    private readonly IHubContext<ChatHub, IChatHub> hubContext;

    public AChatController(IHubContext<ChatHub, IChatHub> hubContext)
    {
        this.hubContext = hubContext;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        await hubContext.Clients.All.ReceiveMessage(
            request.User,
            request.Message,
            DateTime.UtcNow);

        return Ok(new { success = true, message = "Message sent" });
    }

    [HttpPost("send-to-user")]
    public async Task<IActionResult> SendToUser([FromBody] SendToUserRequest request)
    {
        await hubContext.Clients.Client(request.ConnectionId)
            .ReceiveMessage(request.User, request.Message, DateTime.UtcNow);

        return Ok(new { success = true, message = "Message sent to user" });
    }

    [HttpPost("send-to-group")]
    public async Task<IActionResult> SendToGroup([FromBody] SendToGroupRequest request)
    {
        await hubContext.Clients.Group(request.GroupName)
            .ReceiveMessage(request.User, request.Message, DateTime.UtcNow);

        return Ok(new { success = true, message = "Message sent to group" });
    }

    [HttpPost("join-group")]
    public async Task<IActionResult> JoinGroup([FromBody] JoinGroupRequest request)
    {
        await hubContext.Groups.AddToGroupAsync(request.ConnectionId, request.GroupName);
        return Ok(new { success = true });
    }

    [HttpDelete("message/{messageId}")]
    public async Task<IActionResult> DeleteMessage(string messageId)
    {
        // Здесь логика удаления из БД

        await hubContext.Clients.All.MessageDeleted(messageId);
        return Ok(new { success = true });
    }
}
