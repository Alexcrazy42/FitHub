using LinqToDB.Mapping;

namespace Linq2Db.Postgres.Models;

[Table("orders")]
public class Order
{
    [PrimaryKey, Identity]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id"), NotNull]
    public int UserId { get; set; }

    [Column("order_number"), NotNull]
    public string OrderNumber { get; set; } = null!;

    [Column("total_amount"), NotNull]
    public decimal TotalAmount { get; set; }

    [Column("status"), NotNull]
    public string Status { get; set; } = null!;

    [Column("created_at"), NotNull]
    public DateTime CreatedAt { get; set; }
}