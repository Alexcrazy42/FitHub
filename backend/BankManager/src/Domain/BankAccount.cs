using FitHub.Common.Entities;

namespace FitHub.BankManager.Domain;

public class BankAccount : IEntity<BankAccountId>
{
    private BankAccount(BankAccountId id, string name, string currency)
    {
        Id = id;
        Name = name;
        Currency = currency;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public BankAccountId Id { get; }
    public string Name { get; private set; }
    public string Currency { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new LogicViolationException("Банковский аккаунт неактивен!");
        }

        IsActive = false;
    }

    public static BankAccount Create(string name, string currency)
    {
        return new BankAccount(BankAccountId.New(), name, currency);
    }
}
