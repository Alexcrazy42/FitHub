using LinqToDB.Mapping;

namespace Linq2Db.Postgres.Models;

[Table("order_items")]
public class OrderItem
{
    [PrimaryKey, Identity]
    [Column("id")]
    public int Id { get; set; }

    [Column("order_id"), NotNull]
    public int OrderId { get; set; }

    [Column("product_id"), NotNull]
    public int ProductId { get; set; }

    [Column("quantity"), NotNull]
    public int Quantity { get; set; }

    [Column("price"), NotNull]
    public decimal Price { get; set; }
}