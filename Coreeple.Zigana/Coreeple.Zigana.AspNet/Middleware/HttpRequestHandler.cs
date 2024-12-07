using Coreeple.Zigana.Core.Services;
using Microsoft.AspNetCore.Http;

namespace Coreeple.Zigana.AspNet.Middleware;
public class HttpRequestHandler(RequestDelegate next, IApiService apiService)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        var endpoint = await apiService.FindEndpoint(context);

        await context.Response.WriteAsJsonAsync(endpoint);

        await next(context);
    }
}
