using Coreeple.Zigana.Core.Data.Entities;
using Dapper;

namespace Coreeple.Zigana.Core.Data.Repositories;

public class EndpointRepository(IDapperContext context) : IEndpointRepository
{
    public async Task<IEnumerable<Endpoint>?> GetAll()
    {
        using var connection = context.CreateConnection();

        var sql = """
            SELECT "Id", "ApiId", "Path" FROM "Endpoints"
        """;

        var result = await connection.QueryAsync<Endpoint>(sql);

        return result;
    }
}

public interface IEndpointRepository
{
    Task<IEnumerable<Endpoint>?> GetAll();
}