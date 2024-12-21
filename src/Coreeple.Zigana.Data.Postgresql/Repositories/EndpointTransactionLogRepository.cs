using Coreeple.Zigana.Data.Abstractions;
using Coreeple.Zigana.Data.Entities;
using Dapper;

namespace Coreeple.Zigana.Data.Postgresql.Repositories;
public class EndpointTransactionLogRepository(IDbContext dbContext) : IEndpointTransactionLogRepository
{
    public async Task<Guid> InsertAsync(EndpointTransactionLog endpointTransactionLog)
    {
        using var connection = dbContext.CreateConnection();

        var sql = """
            INSERT INTO "EndpointTransactionLogs" ("Id", "TransactionId", "Level", "Log", "Date")
            VALUES (@Id, @TransactionId, @Level, @Log, @Date)
        """;

        await connection.ExecuteAsync(sql, endpointTransactionLog);

        return endpointTransactionLog.Id;
    }
}
