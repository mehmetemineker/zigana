using Coreeple.Zigana.AspNet.Extensions;
using Coreeple.Zigana.Core.Data;
using Coreeple.Zigana.Core.Data.Repositories;
using Coreeple.Zigana.Core.Services;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionExtensions
{
    public static ZiganaBuilder AddZigana(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSingleton<IDapperContext, DapperContext>();
        services.AddTransient<IApiRepository, ApiRepository>();
        services.AddTransient<IEndpointRepository, EndpointRepository>();
        services.AddTransient<IEndpointService, EndpointService>();

        return new ZiganaBuilder(services, configuration);
    }
}
