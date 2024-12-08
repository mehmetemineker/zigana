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

        SetHeaderDefaultResponseContentType(context);
        SetHeaderRequestId(context, requestId);

        var endpoint = await endpointService.FindEndpointAsync(context, context.RequestAborted);

        await logService.BeginAsync(requestId, endpoint.Id, context.RequestAborted);

        await actionExecuteManager.RunAsync(endpoint, context.RequestAborted);

        await context.Response.WriteAsJsonAsync(endpoint, context.RequestAborted);

        await logService.EndAsync(requestId, context.RequestAborted);

        await next(context);
    }

    private static void SetHeaderRequestId(HttpContext context, Guid requestId)
    {
        const string headerKey = "X-Request-Id";
        context.Request.Headers[headerKey] = requestId.ToString();
        context.Response.Headers[headerKey] = requestId.ToString();
    }

    private static void SetHeaderDefaultResponseContentType(HttpContext context) =>
        context.Response.ContentType = "application/json";
}
