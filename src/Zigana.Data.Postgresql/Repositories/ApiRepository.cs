﻿using Zigana.Data.Abstractions;
using Zigana.Data.Entities;
using Dapper;

namespace Zigana.Data.Postgresql.Repositories;
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
}
