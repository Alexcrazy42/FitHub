using FitHub.BankManager.Application;
using FitHub.BankManager.Application.Payments;
using FitHub.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FitHub.BankManager.Data;

public class BankManagerUnitOfWork : IBankManagerUnitOfWork
{
    private readonly BankManagerDataContext context;

    public BankManagerUnitOfWork(BankManagerDataContext context)
    {
        this.context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        try
        {
            return await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Данные оплаты были изменены другим процессом. Обновите состояние оплаты.", ex);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            throw new ValidationException("Запись с такими данными уже существует.", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new UnexpectedException("Ошибка сохранения данных BankManager.", ex);
        }
    }
}
