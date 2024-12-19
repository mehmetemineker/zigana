using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddZigana(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddHttpClient("ZiganaHttpClient")
            .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                }
            });

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;

                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            };
        });

        return services;
    }
}
