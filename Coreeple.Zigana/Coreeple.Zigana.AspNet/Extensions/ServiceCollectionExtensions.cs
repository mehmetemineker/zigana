using Coreeple.Zigana.AspNet.Extensions;
using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.ActionExecutors;
using Coreeple.Zigana.Core.Data;
using Coreeple.Zigana.Core.Data.Repositories;
using Coreeple.Zigana.Core.ResponseBuilders;
using Coreeple.Zigana.Core.Services;
using Coreeple.Zigana.Core.Types.Actions;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionExtensions
{
    public static ZiganaBuilder AddZigana(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddHttpClient("ZiganaHttpClient").ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        });

        services.AddSingleton<IDapperContext, DapperContext>();
        services.AddTransient<IApiRepository, ApiRepository>();
        services.AddTransient<IEndpointRepository, EndpointRepository>();
        services.AddTransient<IEndpointTransactionRepository, EndpointTransactionRepository>();
        services.AddTransient<IEndpointService, EndpointService>();
        services.AddTransient<IEndpointLogService, EndpointLogService>();
        services.AddTransient<IActionExecuteManager, ActionExecuteManager>();
        services.AddTransient<IActionExecutor<HttpRequestAction>, HttpRequestActionExecutor>();
        services.AddTransient<IResponseBuilder, ResponseBuilder>();
        return new ZiganaBuilder(services, configuration);
    }
}
