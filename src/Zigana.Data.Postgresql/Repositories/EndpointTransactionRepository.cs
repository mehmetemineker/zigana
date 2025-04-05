using Zigana.Data.Abstractions;
using Zigana.Data.Entities;
using Dapper;

namespace Zigana.Data.Postgresql.Repositories;
public class EndpointTransactionRepository(IDbContext dbContext) : IEndpointRequestTransactionRepository
{
    public async Task<Guid> InsertAsync(EndpointRequestTransaction endpointTransaction)
    {
        using var connection = dbContext.CreateConnection();

        var sql = """
            INSERT INTO "EndpointRequestTransactions" ("Id", "EndpointId", "Name", "Status", "Message", "Date")
            VALUES (@Id, @EndpointId, @Name, @Status, @Message, @Date)
        """;

        await connection.ExecuteAsync(sql, endpointTransaction);

        return endpointTransaction.Id;
    }
}
