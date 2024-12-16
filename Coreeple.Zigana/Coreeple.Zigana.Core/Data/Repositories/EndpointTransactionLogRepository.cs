using Dapper;

namespace Coreeple.Zigana.Core.Data.Repositories;

public class EndpointTransactionLogRepository(IDapperContext context) : IEndpointTransactionLogRepository
{
    public async Task AddAsync(Guid id, Guid transactionId, string log, CancellationToken cancellationToken = default)
    {
        using var connection = context.CreateConnection();

        var sql = """
            INSERT INTO "EndpointTransactionLogs" ("Id", "TransactionId", "Log")
            VALUES (@Id, @TransactionId, @Log)
        """;

        var result = await connection
            .ExecuteAsync(new CommandDefinition(sql, new
            {
                Id = id,
                TransactionId = transactionId,
                Log = log,
            }, cancellationToken: cancellationToken));
    }
}

public interface IEndpointTransactionLogRepository
{
    Task AddAsync(Guid id, Guid transactionId, string log, CancellationToken cancellationToken = default);
}