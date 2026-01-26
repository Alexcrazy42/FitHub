using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;

ConcurrentDictionary<string, TcpClient> clients = new();
const int PORT = 9000;

var server = new TcpListener(IPAddress.Any, PORT);
server.Start();
Console.WriteLine($"Сервер запущен на порту {PORT}");

while (true)
{
    var client = await server.AcceptTcpClientAsync();
    _ = Task.Run(() => HandleClient(client));
}

async Task HandleClient(TcpClient client)
{
    var stream = client.GetStream();
    var buffer = new byte[1024];
    var username = string.Empty;

    try
    {
        var bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
        username = Encoding.UTF8.GetString(buffer, 0, bytes);
        clients.TryAdd(username, client);
        
        Console.WriteLine($"{username} подключился");
        await BroadcastMessage($"Система: {username} присоединился к чату", username);
        
        while ((bytes = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, bytes);
            Console.WriteLine($"{username}: {message}");
            await BroadcastMessage($"{username}: {message}", username);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
    finally
    {
        if (!string.IsNullOrEmpty(username))
        {
            clients.TryRemove(username, out _);
            await BroadcastMessage($"Система: {username} покинул чат", username);
            Console.WriteLine($"{username} отключился");
        }
        client.Close();
    }
}

async Task BroadcastMessage(string message, string sender)
{
    var data = Encoding.UTF8.GetBytes(message);
    
    foreach (var kvp in clients)
    {
        if (kvp.Key == sender) continue;
        
        try
        {
            NetworkStream stream = kvp.Value.GetStream();
            await stream.WriteAsync(data, 0, data.Length);
        }
        catch
        {
            // ignored
        }
    }
}
