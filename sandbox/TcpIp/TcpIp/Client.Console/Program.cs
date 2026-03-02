using System.Net.Sockets;
using System.Text;
using Spectre.Console;

TcpClient? client;
NetworkStream? stream;

AnsiConsole.Clear();

AnsiConsole.Write(
    new FigletText("TCP Chat")
        .LeftJustified()
        .Color(Color.Cyan1));

var username = AnsiConsole.Ask<string>("Введите ваше [green]имя[/]:");

var host = "127.0.0.1";

try
{
    client = new TcpClient(host, 9000);
    stream = client.GetStream();
    
    var data = Encoding.UTF8.GetBytes(username);
    await stream.WriteAsync(data, 0, data.Length);

    AnsiConsole.MarkupLine("[green]Подключено к серверу![/]\n");

    _ = Task.Run(ReceiveMessages);

    await SendMessages();
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
}

async Task ReceiveMessages()
{
    var buffer = new byte[1024];
    
    try
    {
        while (stream != null)
        {
            var bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytes == 0) break;

            var message = Encoding.UTF8.GetString(buffer, 0, bytes);
            
            if (message.StartsWith("Система:"))
            {
                AnsiConsole.MarkupLine($"[yellow]{message}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[blue]{message}[/]");
            }
        }
    }
    catch
    {
        // ignored
    }
}

async Task SendMessages()
{
    while (true)
    {
        var message = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(message)) continue;
        
        if (message.ToLower() == "/exit")
        {
            client?.Close();
            break;
        }

        if (stream != null)
        {
            var data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
            
            AnsiConsole.MarkupLine($"[green]Вы: {message}[/]");
        }
    }
}
