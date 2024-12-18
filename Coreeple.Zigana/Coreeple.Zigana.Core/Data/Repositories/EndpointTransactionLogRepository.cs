using Dapper;

namespace Coreeple.Zigana.Core.Data.Repositories;

public class EndpointTransactionLogRepository(IDapperContext context) : IEndpointTransactionLogRepository
{
    public async Task AddAsync(Guid id, Guid transactionId, string level, string log, DateTime date, CancellationToken cancellationToken = default)
    {
        using var connection = context.CreateConnection();

        var sql = """
            INSERT INTO "EndpointTransactionLogs" ("Id", "TransactionId", "Level", "Log", "Date")
            VALUES (@Id, @TransactionId, @Level, @Log, @Date)
        """;

        var result = await connection
            .ExecuteAsync(new CommandDefinition(sql, new
            {
                Id = id,
                TransactionId = transactionId,
                Level = level,
                Log = log,
                Date = date,
            }, cancellationToken: cancellationToken));
    }
}

public interface IEndpointTransactionLogRepository
{
    Task AddAsync(Guid id, Guid transactionId, string level, string log, DateTime date, CancellationToken cancellationToken = default);
}