namespace FitHub.Contracts;

public class ListResponse<T>
{
    public IReadOnlyList<T> Items { get; set; } = new List<T>();

    public static ListResponse<T> Create(IReadOnlyList<T> items) => new() { Items = items };
}
