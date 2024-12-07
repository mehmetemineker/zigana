using Coreeple.Zigana.Core.Services;
using Microsoft.AspNetCore.Http;

namespace Coreeple.Zigana.AspNet.Middleware;
public class HttpRequestHandler(RequestDelegate next, IEndpointService endpointService, ILogService logService)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = await endpointService.FindEndpointAsync(context, context.RequestAborted);

        var requestId = Guid.NewGuid();

        await logService.BeginAsync(requestId, endpoint.Id, context.RequestAborted);

        await context.Response.WriteAsJsonAsync(endpoint, context.RequestAborted);

        await logService.EndAsync(requestId, context.RequestAborted);

        SetDefaultResponseContentType(context);

        await next(context);
    }

    private static void SetDefaultResponseContentType(HttpContext context) =>
        context.Response.ContentType = "application/json";
}
