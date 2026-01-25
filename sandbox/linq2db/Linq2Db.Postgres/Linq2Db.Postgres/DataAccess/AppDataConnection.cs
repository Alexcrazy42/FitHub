using Linq2Db.Postgres.Models;
using LinqToDB;
using LinqToDB.Data;

namespace Linq2Db.Postgres.DataAccess;

public class AppDataConnection : DataConnection
{
    public AppDataConnection(DataOptions options) 
        : base(options)
    {
    }

    public ITable<User> Users => this.GetTable<User>();
    public ITable<Product> Products => this.GetTable<Product>();
    public ITable<Order> Orders => this.GetTable<Order>();
    public ITable<OrderItem> OrderItems => this.GetTable<OrderItem>();
}