using Microsoft.AspNetCore.Http;

namespace Coreeple.Zigana.Core.AspNet.Middleware;
public class TraceIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.TraceIdentifier = Guid.NewGuid().ToString();

        if (context.Request.Headers.TryGetValue("X-Request-Id", out var requestId))
        {
            context.TraceIdentifier = requestId.ToString();
        }

        context.Response.Headers["X-Request-Id"] = context.TraceIdentifier;

        await next(context);
    }
}
