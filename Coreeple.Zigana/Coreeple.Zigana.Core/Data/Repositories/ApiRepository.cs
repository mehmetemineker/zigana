using Coreeple.Zigana.Core.Data.Entities;
using Dapper;

namespace Coreeple.Zigana.Core.Data.Repositories;

public class ApiRepository(IDapperContext context) : IApiRepository
{
    public async Task<IEnumerable<Api>?> GetAll()
    {
        using var connection = context.CreateConnection();

        var sql = """
            SELECT "Id", "Path" FROM "Apis"
        """;

        var result = await connection.QueryAsync<Api>(sql);

        return result;
    }
}

public interface IApiRepository
{
    Task<IEnumerable<Api>?> GetAll();
}