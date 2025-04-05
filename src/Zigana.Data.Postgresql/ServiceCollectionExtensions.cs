using Zigana.AspNet;
using Zigana.Data.Abstractions;
using Zigana.Data.Postgresql;
using Zigana.Data.Postgresql.Repositories;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionExtensions
{
    public static ZiganaBuilder UseNpgsql(this ZiganaBuilder builder, string schema = "public")
    {
        ArgumentNullException.ThrowIfNull(builder);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var dbContext = new PostgresqlDbContext(connectionString, schema);
        dbContext.Migration();

        builder.Services.AddSingleton<IDbContext>(_ => dbContext);

        builder.Services.AddScoped<IApiRepository, ApiRepository>();
        builder.Services.AddScoped<IEndpointRepository, EndpointRepository>();
        builder.Services.AddScoped<IEndpointRequestTransactionRepository, EndpointTransactionRepository>();
        builder.Services.AddScoped<IEndpointTransactionLogRepository, EndpointTransactionLogRepository>();

        return builder;
    }
}
