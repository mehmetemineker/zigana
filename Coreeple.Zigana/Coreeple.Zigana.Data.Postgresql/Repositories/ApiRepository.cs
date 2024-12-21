using Coreeple.Zigana.Data.Abstractions;
using Coreeple.Zigana.Data.Entities;
using Dapper;

namespace Coreeple.Zigana.Data.Postgresql.Repositories;
public class ApiRepository(IDbContext dbContext) : IApiRepository
{
    public async Task<Guid> InsertAsync(Api api)
    {
        using var connection = dbContext.CreateConnection();

        var sql = """
            INSERT INTO "Apis" ("Id", "Path", "Name", "Description", "Defs", "IsActive")
            VALUES(@Id, @Path, @Name, @Description, @Defs::json, false)
        """;

        await connection.ExecuteAsync(sql, api);

        return api.Id;
    }

    public async Task<IEnumerable<Api>> GetPathsAsync(CancellationToken cancellationToken = default)
    {
        using var connection = dbContext.CreateConnection();

        var sql = """
            SELECT "Id", "Path", "IsActive"
            FROM "Apis"
        """;

        return await connection.QueryAsync<Api>(new CommandDefinition(sql, cancellationToken));
    }
}
