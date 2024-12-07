using Coreeple.Zigana.Core.Services;
using Microsoft.AspNetCore.Http;

namespace Coreeple.Zigana.AspNet.Middleware;
public class HttpRequestHandler(RequestDelegate next, IEndpointService endpointService)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        var endpoint = await endpointService.FindEndpointAsync(context, context.RequestAborted);

        await context.Response.WriteAsJsonAsync(endpoint, context.RequestAborted);

        await next(context);
    }
}
