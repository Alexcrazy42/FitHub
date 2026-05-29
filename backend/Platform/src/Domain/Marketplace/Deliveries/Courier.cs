using FitHub.Common.Entities;

namespace FitHub.Domain.Marketplace.Deliveries;

public class Courier : IEntity<CourierId>
{
    private Courier(CourierId id, string name, DateTimeOffset createdAt)
    {
        Id = id;
        Name = name;
        IsAvailable = true;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public CourierId Id { get; }
    public string Name { get; private set; }
    public bool IsAvailable { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public void MarkBusy()
    {
        if (!IsAvailable)
        {
            return;
        }

        IsAvailable = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAvailable()
    {
        if (IsAvailable)
        {
            return;
        }

        IsAvailable = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public static Courier Create(string name)
    {
        return new Courier(CourierId.New(), name, DateTimeOffset.UtcNow);
    }
}
