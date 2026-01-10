using FitHub.Application.Common;
using FitHub.Common.Utilities.System;
using FitHub.Contracts;

namespace FitHub.Web.Common;

public static class PageExtensions
{
    public static PagedQuery ToQuery(this PagedRequest? request)
    {
        var pageNumber = 1;
        var pageSize = 10;

        if (request?.PageNumber is not null)
        {
            pageNumber = request.PageNumber.Value;
        }

        if (request?.PageSize is not null && request.PageSize > 0 && request.PageSize <= 1000)
        {
            pageSize = request.PageSize.Value;
        }

        return new PagedQuery(pageNumber: pageNumber, pageSize: pageSize);
    }

    public static ListResponse<TTo> ToListResponse<TFrom, TTo>(this PagedResult<TFrom> pagedResult, Func<TFrom, TTo> converter)
        where TFrom : class
        where TTo : class
    {
        var newItems = pagedResult.Items.Select(converter).ToList();
        if (pagedResult.TotalItems is null || pagedResult.CurrentPage is null || pagedResult.PageSize is null)
        {
            return ListResponse<TTo>.Create(newItems);
        }
        return ListResponse<TTo>.Create(newItems, pagedResult.TotalItems.Required(), pagedResult.CurrentPage.Required(), pagedResult.PageSize.Required());
    }

    public static ListResponse<TTo> ToListResponse<TFrom, TTo>(this IReadOnlyList<TFrom> items, Func<TFrom, TTo> converter)
        where TFrom : class
        where TTo : class
    {
        var newItems = items.Select(converter).ToList();
        return ListResponse<TTo>.Create(newItems);
    }
}

