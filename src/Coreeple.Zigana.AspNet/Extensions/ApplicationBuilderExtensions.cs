using Coreeple.Zigana.AspNet.Middlewares;

namespace Microsoft.AspNetCore.Builder;
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseZigana(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseExceptionHandler();
        app.UseStatusCodePages();

        app.UseMiddleware<CustomTraceIdentifierMiddleware>();
        app.UseMiddleware<HttpRequestHandlerMiddleware>();

        return app;
    }
}
