using Coreeple.Zigana.Core.Data.Entities;
using Dapper;

namespace Coreeple.Zigana.Core.Data.Repositories;

public class EndpointRepository : IEndpointRepository
{
    private readonly IDapperContext _context;
    public EndpointRepository(IDapperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public async Task<Endpoint?> GetEndpointById(Guid id)
    {
        using var connection = _context.CreateConnection();

        var sql = """
            SELECT * FROM "Endpoints"
            WHERE "Id" = @Id
        """;

        var result = await connection
            .QuerySingleOrDefaultAsync<Endpoint>(sql, new { Id = id });

        return result;
    }
}

public interface IEndpointRepository
{
    Task<Endpoint?> GetEndpointById(Guid id);
}