using Coreeple.Zigana.Core.Abstractions;
using Coreeple.Zigana.Core.Services;
using Microsoft.AspNetCore.Http;

namespace Coreeple.Zigana.AspNet.Middleware;
public class HttpRequestHandler(
    RequestDelegate next,
    IEndpointService endpointService,
    ILogService logService,
    IActionExecuteManager actionExecuteManager)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid();

        SetDefaultResponseContentType(context);
        SetRequestId(context, requestId);

        var endpoint = await endpointService.FindEndpointAsync(context, context.RequestAborted);

        await logService.BeginAsync(requestId, endpoint.Id, context.RequestAborted);

        await actionExecuteManager.StartAsync(endpoint, context.RequestAborted);

        await context.Response.WriteAsJsonAsync(endpoint, context.RequestAborted);

        await logService.EndAsync(requestId, context.RequestAborted);

        await next(context);
    }

    private static void SetRequestId(HttpContext context, Guid requestId)
    {
        context.Request.Headers["X-Request-Id"] = requestId.ToString();
        context.Response.Headers["X-Request-Id"] = requestId.ToString();
    }

    private static void SetDefaultResponseContentType(HttpContext context) =>
        context.Response.ContentType = "application/json";
}
