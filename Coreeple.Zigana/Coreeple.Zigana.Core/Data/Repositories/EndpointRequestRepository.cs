using Dapper;

namespace Coreeple.Zigana.Core.Data.Repositories;

public class EndpointRequestRepository(IDapperContext context) : IEndpointRequestRepository
{
    public async Task BeginAsync(Guid id, Guid endpointId, DateTime beginDate, CancellationToken cancellationToken = default)
    {
        using var connection = context.CreateConnection();

        var sql = """
            INSERT INTO "EndpointRequests" ("Id", "EndpointId", "BeginDate", "Status")
            VALUES (@Id, @EndpointId, @BeginDate, @Status)
        """;

        var result = await connection
            .ExecuteAsync(new CommandDefinition(sql, new
            {
                Id = id,
                EndpointId = endpointId,
                BeginDate = beginDate,
                Status = "CONTINUE"
            }, cancellationToken: cancellationToken));
    }

    public async Task UpdateStatusAsync(Guid id, string status, CancellationToken cancellationToken = default)
    {
        const string allowedStatus = "ERROR";

        if (status != allowedStatus)
        {
            throw new Exception("Status Not Allowed");
        }

        using var connection = context.CreateConnection();

        var sql = """
            UPDATE "EndpointRequests" SET "Status" = @Status
            WHERE "Id" = @Id
        """;

        var result = await connection
            .ExecuteAsync(new CommandDefinition(sql, new
            {
                Id = id,
                Status = status
            }, cancellationToken: cancellationToken));
    }

    public async Task EndAsync(Guid id, DateTime endDate, CancellationToken cancellationToken = default)
    {
        using var connection = context.CreateConnection();

        var sql = """
            UPDATE "EndpointRequests" SET "EndDate" = @EndDate, "Status" = @Status
            WHERE "Id" = @Id
        """;

        var result = await connection
            .ExecuteAsync(new CommandDefinition(sql, new
            {
                Id = id,
                EndDate = endDate,
                Status = "FINISH"
            }, cancellationToken: cancellationToken));
    }
}

public interface IEndpointRequestRepository
{
    Task BeginAsync(Guid id, Guid endpointId, DateTime beginDate, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);
    Task EndAsync(Guid id, DateTime endDate, CancellationToken cancellationToken = default);
}