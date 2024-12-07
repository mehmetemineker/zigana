using Coreeple.Zigana.Core.Data.Entities;
using Dapper;

namespace Coreeple.Zigana.Core.Data.Repositories;

public class EndpointRepository(IDapperContext context) : IEndpointRepository
{
    public async Task<IEnumerable<Endpoint>?> GetAll()
    {
        using var connection = context.CreateConnection();

        var sql = """
            SELECT "Id", "ApiId", "Path", "Method"
            FROM "Endpoints"
        """;

        var result = await connection.QueryAsync<Endpoint>(sql);

        return result;
    }

    public async Task<Endpoint?> GetByIdWithApi(Guid id)
    {
        using var connection = context.CreateConnection();

        var sql = """
            SELECT 
                ep."Id", ep."ApiId", api."Path" || ep."Path" AS "Path", ep."Method",
                ep."Actions", ep."Response", api."Definitions"
            FROM "Endpoints" ep
            LEFT JOIN public."Apis" api ON api."Id" = ep."ApiId"
            WHERE ep."Id" = @Id
        """;

        var result = await connection
            .QuerySingleOrDefaultAsync<Endpoint>(sql, new { Id = id });

        return result;
    }
}

public interface IEndpointRepository
{
    Task<IEnumerable<Endpoint>?> GetAll();
    Task<Endpoint?> GetByIdWithApi(Guid id);
}