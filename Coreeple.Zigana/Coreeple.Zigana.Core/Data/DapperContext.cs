using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace Coreeple.Zigana.Core.Data;
public class DapperContext : IDapperContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}

public interface IDapperContext
{
    public IDbConnection CreateConnection();
}