using Coreeple.Zigana.Data.Abstractions;
using Coreeple.Zigana.Data.Entities;
using Dapper;

namespace Coreeple.Zigana.Data.Postgresql.Repositories;
public class EndpointRepository(IDbContext dbContext) : IEndpointRepository
{
    public async Task<Guid> InsertAsync(Endpoint endpoint)
    {
        using var connection = dbContext.CreateConnection();

        var sql = """
            INSERT INTO "Endpoints" ("Id", "ApiId", "Path", "Method", "Actions", "Response", "IsActive")
            VALUES(@Id, @ApiId, @Path, @Method, @Actions::json, @Response::json, false)
        """;

        await connection.ExecuteAsync(sql, endpoint);

        return endpoint.Id;
    }

    public async Task<IEnumerable<Endpoint>> GetPathsAsync(CancellationToken cancellationToken = default)
    {
        using var connection = dbContext.CreateConnection();

        var sql = """
            SELECT "Id", "ApiId", "Path", "Method", "IsActive"
            FROM "Endpoints"
        """;

        return await connection.QueryAsync<Endpoint>(new CommandDefinition(sql, cancellationToken));
    }
}
