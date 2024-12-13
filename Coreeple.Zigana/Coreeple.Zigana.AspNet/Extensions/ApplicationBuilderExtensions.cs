using Coreeple.Zigana.AspNet.Middleware;

namespace Microsoft.AspNetCore.Builder;
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseZigana(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseExceptionHandler();
        app.UseStatusCodePages();

        app.UseMiddleware<TraceIdMiddleware>();
        app.UseMiddleware<HttpRequestHandler>();

        return app;
    }
}
