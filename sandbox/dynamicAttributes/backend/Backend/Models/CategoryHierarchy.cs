namespace Backend.Models;

/// <summary>
/// Closure Table для иерархии категорий
/// Хранит все связи предок-потомок для быстрого поиска
/// </summary>
public class CategoryHierarchy
{
    public Guid AncestorId { get; set; }
    public Category Ancestor { get; set; } = null!;
    public Guid DescendantId { get; set; }
    public Category Descendant { get; set; } = null!;
    public int Depth { get; set; }
}
