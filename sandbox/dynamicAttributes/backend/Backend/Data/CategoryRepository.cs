using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class CategoryRepository
{
    private readonly ApplicationDbContext _db;

    public CategoryRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Получить все категории категории (включая вложенные) через Closure Table
    /// </summary>
    public async Task<List<Guid>> GetAllDescendantCategoryIdsAsync(Guid categoryId, CancellationToken ct = default)
    {
        var descendants = await _db.CategoryHierarchies
            .Where(h => h.AncestorId == categoryId)
            .Select(h => h.DescendantId)
            .ToListAsync(ct);
        
        return descendants;
    }

    /// <summary>
    /// Построить иерархию для новой категории
    /// </summary>
    public async Task RebuildHierarchyForCategoryAsync(Guid categoryId, Guid? parentId, CancellationToken ct = default)
    {
        // Удаляем старые записи для этой категории
        var oldHierarchies = await _db.CategoryHierarchies
            .Where(h => h.DescendantId == categoryId)
            .ToListAsync(ct);
        _db.CategoryHierarchies.RemoveRange(oldHierarchies);

        if (parentId == null)
        {
            // Корневая категория - только ссылка на себя
            _db.CategoryHierarchies.Add(new CategoryHierarchy
            {
                AncestorId = categoryId,
                DescendantId = categoryId,
                Depth = 0
            });
        }
        else
        {
            // Копируем все связи родителя + добавляем новую
            var parentHierarchies = await _db.CategoryHierarchies
                .Where(h => h.DescendantId == parentId)
                .ToListAsync(ct);

            foreach (var parentHierarchy in parentHierarchies)
            {
                _db.CategoryHierarchies.Add(new CategoryHierarchy
                {
                    AncestorId = parentHierarchy.AncestorId,
                    DescendantId = categoryId,
                    Depth = parentHierarchy.Depth + 1
                });
            }

            // Добавляем ссылку на себя
            _db.CategoryHierarchies.Add(new CategoryHierarchy
            {
                AncestorId = categoryId,
                DescendantId = categoryId,
                Depth = 0
            });
        }

        await _db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Получить дерево категорий с количеством товаров
    /// </summary>
    public async Task<List<CategoryNode>> GetCategoryTreeAsync(CancellationToken ct = default)
    {
        var categories = await _db.Categories
            .Include(c => c.ProductCategories)
            .ToListAsync(ct);

        var categoryIds = categories.Select(c => c.Id).ToList();
        var productCounts = await _db.ProductCategories
            .Where(pc => categoryIds.Contains(pc.CategoryId))
            .GroupBy(pc => pc.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Count, ct);

        var rootCategories = categories.Where(c => c.ParentId == null).ToList();
        
        return BuildTree(rootCategories, categories, productCounts);
    }

    private List<CategoryNode> BuildTree(
        List<Category> categories, 
        List<Category> allCategories,
        Dictionary<Guid, int> productCounts)
    {
        return categories.Select(c => new CategoryNode
        {
            Id = c.Id,
            Name = c.Name,
            ProductCount = productCounts.GetValueOrDefault(c.Id, 0),
            Children = BuildTree(
                allCategories.Where(child => child.ParentId == c.Id).ToList(),
                allCategories,
                productCounts)
        }).ToList();
    }
}

public record CategoryNode
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int ProductCount { get; init; }
    public List<CategoryNode> Children { get; init; } = new();
}
