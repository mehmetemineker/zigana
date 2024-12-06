using Coreeple.Zigana.AspNet.Extensions;
using Coreeple.Zigana.Core.Data;
using Coreeple.Zigana.Core.Data.Repositories;
using Coreeple.Zigana.Core.Data.Services;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionExtensions
{
    public static ZiganaBuilder AddZigana(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSingleton<IDapperContext, DapperContext>();
        services.AddScoped<IEndpointRepository, EndpointRepository>();
        services.AddScoped<IEndpointService, EndpointService>();

        return new ZiganaBuilder(services, configuration);
    }
}
