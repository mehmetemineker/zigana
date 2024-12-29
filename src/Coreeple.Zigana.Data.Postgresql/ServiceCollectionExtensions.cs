using Coreeple.Zigana.AspNet;
using Coreeple.Zigana.Data.Abstractions;
using Coreeple.Zigana.Data.Postgresql;
using Coreeple.Zigana.Data.Postgresql.Repositories;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionExtensions
{
    public static ZiganaBuilder UseNpgsql(this ZiganaBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddSingleton<IDbContext>(_ => new PostgresqlDbContext(connectionString));

        builder.Services.AddScoped<IApiRepository, ApiRepository>();
        builder.Services.AddScoped<IEndpointRepository, EndpointRepository>();
        builder.Services.AddScoped<IEndpointRequestTransactionRepository, EndpointTransactionRepository>();
        builder.Services.AddScoped<IEndpointTransactionLogRepository, EndpointTransactionLogRepository>();

        return builder;
    }
}
