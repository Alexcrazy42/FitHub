namespace FitHub.BankManager.Application.Payments;

public interface IBankManagerUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct);
}
