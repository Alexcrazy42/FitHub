using FitHub.Common.AspNetCore;
using FitHub.Common.Utilities.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FitHub.Web.V1;


public record SendToUserRequest(string UserId, string Message);
public record SendToGroupRequest(string GroupName, string Message);

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

    [HttpPost("send-to-user")]
    public async Task<IActionResult> SendToUser([FromBody] SendToUserRequest request)
    {
        var userName = HttpContext.User.GetUsername().Required();

        await hubContext.Clients.User(request.UserId)
            .CreateMessage(userName, request.Message, DateTime.UtcNow);

        return Ok(new { success = true, message = "Message sent to user" });
    }

    [HttpPost("send-to-group")]
    public async Task<IActionResult> SendToGroup([FromBody] SendToGroupRequest request)
    {
        var userName = HttpContext.User.GetUsername().Required();

        await hubContext.Clients.Group(request.GroupName)
            .CreateMessage(userName, request.Message, DateTime.UtcNow);

        return Ok(new { success = true, message = "Message sent to group" });
    }

    [HttpDelete("message/{messageId}")]
    public async Task<IActionResult> DeleteMessage(string messageId)
    {
        // Здесь логика удаления из БД

        await hubContext.Clients.All.MessageDeleted(messageId); // TODO: и тут мы должны сами достать id чата и отправить в нужную группу
        return Ok(new { success = true });
    }
}
