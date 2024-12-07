﻿using Coreeple.Zigana.Core.Data.Entities;
using Dapper;

namespace Coreeple.Zigana.Core.Data.Repositories;

public class EndpointRepository(IDapperContext context) : IEndpointRepository
{
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
    Task<IEnumerable<Endpoint>?> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Endpoint?> GetByIdWithApiAsync(Guid id, CancellationToken cancellationToken = default);
}