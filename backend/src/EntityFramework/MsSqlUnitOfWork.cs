using FitHub.Common.Entities;
using FitHub.Common.Entities.Storage;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Common.EntityFramework;

internal sealed class MsSqlUnitOfWork<TContext> : IUnitOfWork
    where TContext : DbContext
{
    private readonly TContext context;

    public MsSqlUnitOfWork(TContext context)
    {
        this.context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException(ex.Message, ex);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
        {
            // 2601 = unique constraint violation (duplicate key)
            throw new UniqueConstraintException("Сущность с такими данными уже существует", ex);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
        {
            // 547 = foreign key violation
            throw new UnexpectedException("Нарушение внешнего ключа в базе данных", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new UnexpectedException("Ошибка сохранения данных в базе данных", ex);
        }
        catch (SqlException ex)
        {
            throw new UnexpectedException("База данных недоступна или произошла ошибка SQL Server", ex);
        }
    }
}
