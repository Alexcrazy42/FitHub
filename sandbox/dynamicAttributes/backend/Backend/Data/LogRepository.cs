using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class LogRepository
{
    private readonly ApplicationDbContext dbContext;

    public LogRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Log>> GetLogs()
    {
        return await dbContext.Set<Log>()
            .Where(x => x.Payload.RootElement.GetProperty("Name").GetString() == "log")
            .ToListAsync();
    }

    public async Task<Log> GetLog()
    {
        return await dbContext.Set<Log>()
            .FirstAsync(x => x.Payload.RootElement.GetProperty("Orders")[1].GetProperty("Price").GetInt32() == 1);
    }

    public async Task<IReadOnlyList<Log>> GetLogs1()
    {
        return await dbContext.Set<Log>()
            .Where(x => EF.Functions.JsonContains(x.Payload, @"{""Name"": ""Joe"", ""Age"": 25}"))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Log>> GetLogs2()
    {
        return await dbContext.Set<Log>()
            .Where(x => EF.Functions.JsonExists(x.Payload, "Age"))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Log>> GetLogs3(int minPrice)
    {
        var result = await dbContext.Set<Log>()
            .FromSqlInterpolated($"""
                                  SELECT *
                                  FROM "Log"
                                  WHERE "Payload" @? '$.Orders[*] ? (@.Price >= 100)'
                                  """)
            .ToListAsync();

        return result;
    }
}