using Microsoft.AspNetCore.Http;

namespace Coreeple.Zigana.AspNet.Middlewares;
public class HttpRequestHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);
    }
}
