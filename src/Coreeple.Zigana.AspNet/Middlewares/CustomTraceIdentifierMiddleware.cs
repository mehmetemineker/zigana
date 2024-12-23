using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Coreeple.Zigana.AspNet.Middlewares;

public class CustomTraceIdentifierMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, DiagnosticSource diagnosticSource)
    {
        context.TraceIdentifier = Guid.NewGuid().ToString();

        if (context.Request.Headers.TryGetValue("X-Request-Id", out var requestId))
        {
            context.TraceIdentifier = requestId.ToString();
        }

        context.Response.Headers["X-Request-Id"] = context.TraceIdentifier;
        context.Items["RequestId"] = context.TraceIdentifier;

        await next(context);
    }
}