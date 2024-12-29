using Coreeple.Zigana.AspNet;
using Coreeple.Zigana.Core.Diagnostics;
using Coreeple.Zigana.Core.Types;
using Coreeple.Zigana.EndpointProcessor;
using Coreeple.Zigana.EndpointProcessor.Abstractions;
using Coreeple.Zigana.EndpointProcessor.ActionExecutors;
using Coreeple.Zigana.EndpointProcessor.ResponseBuilders;
using Coreeple.Zigana.Services;
using Coreeple.Zigana.Services.Abstractions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;
public static class ServiceCollectionExtensions
{
    public static ZiganaBuilder AddZigana(this IServiceCollection services, IConfiguration configuration)
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

        services.AddSingleton(new ZiganaDiagnosticSource());

        services.AddScoped<IApiService, ApiService>();
        services.AddScoped<IEndpointService, EndpointService>();
        services.AddScoped<IEndpointLogService, EndpointLogService>();

        services.AddScoped<IEndpointContext, EndpointContext>();

        services.AddScoped<IActionExecuteManager, ActionExecuteManager>();
        services.AddScoped<IActionExecutor<HttpRequestAction>, HttpRequestActionExecutor>();
        services.AddScoped<IActionExecutor<HtmlParserAction>, HtmlParserActionExecutor>();
        services.AddScoped<IActionExecutor<ParallelAction>, ParallelActionExecutor>();

        services.AddScoped<IResponseBuilder, ResponseBuilder>();

        return new ZiganaBuilder(services, configuration);
    }
}
