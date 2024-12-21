using Coreeple.Zigana.Data.Abstractions;
using Coreeple.Zigana.Data.Entities;
using Coreeple.Zigana.Data.Exceptions;
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
            SELECT 
                ep."Id", 
                ep."ApiId", 
                api."Path" || ep."Path" AS "Path", 
                ep."Method", (ep."IsActive" OR api."IsActive") AS "IsActive"
            FROM "Endpoints" ep
            INNER JOIN public."Apis" api ON api."Id" = ep."ApiId"
        """;

        return await connection.QueryAsync<Endpoint>(new CommandDefinition(sql, cancellationToken));
    }

    public async Task<Endpoint> GetByIdWithApiAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = dbContext.CreateConnection();

        var sql = """
            SELECT 
                ep."Id", 
                ep."ApiId", 
                api."Path" || ep."Path" AS "Path", 
                ep."Method", 
                ep."Actions", 
                ep."Response", 
                api."Defs",
                ep."IsActive"
            FROM "Endpoints" ep
            INNER JOIN public."Apis" api ON api."Id" = ep."ApiId"
            WHERE ep."Id" = @Id 
                AND api."IsActive" = true 
                AND ep."IsActive" = true
        """;

        var result = await connection.QuerySingleOrDefaultAsync<Endpoint>(new CommandDefinition(sql, new
        {
            Id = id
        }, cancellationToken: cancellationToken));

        return result ?? throw new RecordNotFoundDataException("Endpoint not found!");
    }
}
