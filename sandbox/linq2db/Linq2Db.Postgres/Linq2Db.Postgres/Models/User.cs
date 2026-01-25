using LinqToDB.Mapping;

namespace Linq2Db.Postgres.Models;

[Table("users")]
public class User
{
    [PrimaryKey, Identity]
    [Column("id")]
    public int Id { get; set; }

    [Column("username"), NotNull]
    public string Username { get; set; } = null!;

    [Column("email"), NotNull]
    public string Email { get; set; } = null!;

    [Column("created_at"), NotNull]
    public DateTime CreatedAt { get; set; }
}