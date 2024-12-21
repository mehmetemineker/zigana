using Coreeple.Zigana.Data.Abstractions;
using Coreeple.Zigana.Data.Entities;
using Dapper;

namespace Coreeple.Zigana.Data.Postgresql.Repositories;
public class EndpointTransactionRepository(IDbContext dbContext) : IEndpointTransactionRepository
{
    public async Task<Guid> InsertAsync(EndpointTransaction endpointTransaction)
    {
        using var connection = dbContext.CreateConnection();

        var sql = """
            INSERT INTO "EndpointTransactions" ("Id", "EndpointId", "RequestId", "Name", "Status", "Date")
            VALUES (@Id, @EndpointId, @RequestId, @Name, @Status, @Date)
        """;

        await connection.ExecuteAsync(sql, endpointTransaction);

        return endpointTransaction.Id;
    }
}
