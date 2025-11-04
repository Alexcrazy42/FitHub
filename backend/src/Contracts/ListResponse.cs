namespace FitHub.Contracts;

public class ListResponse<T>
{
    /// <summary>
    /// Список элементов
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Номер текущей страницы
    /// </summary>
    public int? CurrentPage { get; private set; }

    /// <summary>
    /// Количество элементов на странице
    /// </summary>
    public int? PageSize { get; private set; }

    /// <summary>
    /// Общее количество элементов
    /// </summary>
    public int? TotalItems { get; private set; }

    /// <summary>
    /// Общее количество страниц
    /// </summary>
    public int? TotalPages => (int)Math.Ceiling((double)(TotalItems ?? 0) / PageSize ?? 1);

    public static ListResponse<T> Create(IReadOnlyList<T> items) => new() { Items = items };

    public static ListResponse<T> Create(IReadOnlyList<T> items, int totalItems, int currentPage, int pageSize)
    {
        return new ListResponse<T>
        {
            Items = items,
            TotalItems = totalItems,
            CurrentPage = currentPage,
            PageSize = pageSize
        };
    }
}
