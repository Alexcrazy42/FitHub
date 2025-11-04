using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FitHub.Common.EntityFramework;

internal sealed class PostgresUnitOfWork<TContext> : IUnitOfWork
    where TContext : DbContext
{
    private readonly TContext context;

    public PostgresUnitOfWork(TContext context)
    {
        this.context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var message = "Произошла непредвиденная ошибка!";
        try
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Данные были изменены другим пользователем. Обновите страницу.", ex);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            // 23505 = unique constraint violation
            message = "Запись с такими данными уже существует.";
            throw new ValidationException(message, ex);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23503")
        {
            // 23503 = foreign key violation
            message = "Невозможно удалить запись, так как она используется в других данных.";
            throw new ValidationException(message, ex);
        }
        catch (DbUpdateException ex)
        {
            throw new UnexpectedException("Ошибка сохранения данных в базе данных", ex);
        }
        catch (PostgresException ex)
        {
            throw new UnexpectedException("База данных недоступна или произошла ошибка PostgreSQL", ex);
        }
    }
}
