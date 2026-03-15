using Microsoft.EntityFrameworkCore;

namespace FitHub.Common.EntityFramework;

public static class QueryableExtensions
{
    public static async Task<IReadOnlyList<TSource>> ToReadOnlyListAsync<TSource>(
        this IQueryable<TSource> source,
        CancellationToken cancellationToken)
        => await source.ToListAsync(cancellationToken).ConfigureAwait(false);
}
