using LinqToDB.Mapping;

namespace Linq2Db.Postgres.Models;

[Table("products")]
public class Product
{
    [PrimaryKey, Identity]
    [Column("id")]
    public int Id { get; set; }

    [Column("name"), NotNull]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("price"), NotNull]
    public decimal Price { get; set; }

    [Column("stock"), NotNull]
    public int Stock { get; set; }

    [Column("created_at"), NotNull]
    public DateTime CreatedAt { get; set; }
}