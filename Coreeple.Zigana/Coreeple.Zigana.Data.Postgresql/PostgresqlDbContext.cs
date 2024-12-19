using Coreeple.Zigana.Data.Abstractions;
using Npgsql;
using System.Data;

namespace Coreeple.Zigana.Data.Postgresql;
public class PostgresqlDbContext(string connectionString) : IDbContext
{
    public IDbConnection CreateConnection() => new NpgsqlConnection(connectionString);
}
