namespace FitHub.Web.V1;

public static class ChatHubExtensions
{
    public static string GetChatGroupName(this string chatId)
    {
        return $"chat-{chatId}";
    }
}
