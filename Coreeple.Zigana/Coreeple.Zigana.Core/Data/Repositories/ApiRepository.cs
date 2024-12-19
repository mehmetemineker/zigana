using Coreeple.Zigana.Core.Data.Entities;
using Dapper;

namespace Coreeple.Zigana.Core.Data.Repositories;

public class ApiRepository(IDapperContext context) : IApiRepository
{
    public async Task<Guid> CreateAsync(Api api)
    {
        using var connection = context.CreateConnection();

        var sql = """
            INSERT INTO "Apis" ("Id", "Path", "Name", "Description", "Defs")
            VALUES(@Id, @Path, @Name, @Description, @Defs::json)
        """;

        await connection.ExecuteAsync(sql, api);

        return api.Id;
    }

    public async Task<IEnumerable<Api>?> GetAllPathsAsync(CancellationToken cancellationToken = default)
    {
        using var connection = context.CreateConnection();

        var sql = """
            SELECT "Id", "Path" FROM "Apis"
        """;

        var result = await connection.QueryAsync<Api>(new CommandDefinition(sql, cancellationToken));

        return result;
    }
}

public interface IApiRepository
{
    Task<Guid> CreateAsync(Api api);
    Task<IEnumerable<Api>?> GetAllPathsAsync(CancellationToken cancellationToken = default);
}