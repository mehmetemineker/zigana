using Coreeple.Zigana.Core.Data.Entities;
using Dapper;

namespace Coreeple.Zigana.Core.Data.Repositories;

public class EndpointRepository(IDapperContext context) : IEndpointRepository
{
    public async Task<Guid> CreateAsync(Guid apiId, string path, string method, string? actions, string? response, CancellationToken cancellationToken = default)
    {
        using var connection = context.CreateConnection();

        var sql = """
            INSERT INTO "Endpoints" ("Id", "ApiId", "Path", "Method", "Actions", "Response")
            VALUES(@Id, @ApiId, @Path, @Method, @Actions::json, @Response::json)
        """;

        Guid id = Guid.NewGuid();

        await connection.ExecuteAsync(new CommandDefinition(sql, new
        {
            Id = id,
            ApiId = apiId,
            Path = path,
            Method = method,
            Actions = actions,
            Response = response,
        }, cancellationToken: cancellationToken));

        return id;
    }

    public async Task<IEnumerable<Endpoint>?> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = context.CreateConnection();

        var sql = """
            SELECT "Id", "ApiId", "Path", "Method"
            FROM "Endpoints"
        """;

        var result = await connection.QueryAsync<Endpoint>(new CommandDefinition(sql, cancellationToken));

        return result;
    }

    public async Task<Endpoint?> GetByIdWithApiAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = context.CreateConnection();

        var sql = """
            SELECT 
                ep."Id", ep."ApiId", api."Path" || ep."Path" AS "Path", ep."Method",
                ep."Actions", ep."Response", api."Defs"
            FROM "Endpoints" ep
            LEFT JOIN public."Apis" api ON api."Id" = ep."ApiId"
            WHERE ep."Id" = @Id
        """;

        var result = await connection
            .QuerySingleOrDefaultAsync<Endpoint>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return result;
    }
}

public interface IEndpointRepository
{
    Task<Guid> CreateAsync(Guid apiId, string path, string method, string? actions, string? response, CancellationToken cancellationToken = default);
    Task<IEnumerable<Endpoint>?> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Endpoint?> GetByIdWithApiAsync(Guid id, CancellationToken cancellationToken = default);
}