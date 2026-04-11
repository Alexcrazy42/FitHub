using System.Text.Json;

namespace Backend.Entities;

public class Log : IDisposable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public JsonDocument Payload { get; set; }

    public DateTimeOffset Created { get; set; }
    

    public void Dispose()
    {
        Payload.Dispose();
    }
}