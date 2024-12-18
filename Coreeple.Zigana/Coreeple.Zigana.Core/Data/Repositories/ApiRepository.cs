using Coreeple.Zigana.Core.Data.Entities;
using Dapper;

namespace Coreeple.Zigana.Core.Data.Repositories;

public class ApiRepository(IDapperContext context) : IApiRepository
{
    public async Task<Guid> CreateAsync(string path, string? name, string? description, string? defs, CancellationToken cancellationToken = default)
    {
        using var connection = context.CreateConnection();

        var sql = """
            INSERT INTO "Apis" ("Id", "Path", "Name", "Description", "Defs")
            VALUES(@Id, @Path, @Name, @Description, @Defs::json)
        """;

        Guid id = Guid.NewGuid();

        await connection.ExecuteAsync(new CommandDefinition(sql, new
        {
            Id = id,
            Path = path,
            Name = name,
            Description = description,
            Defs = defs,
        }, cancellationToken: cancellationToken));

        return id;
    }

    public async Task<IEnumerable<Api>?> GetAllAsync(CancellationToken cancellationToken = default)
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
    Task<Guid> CreateAsync(string path, string? name, string? description, string? defs, CancellationToken cancellationToken = default);
    Task<IEnumerable<Api>?> GetAllAsync(CancellationToken cancellationToken = default);
}