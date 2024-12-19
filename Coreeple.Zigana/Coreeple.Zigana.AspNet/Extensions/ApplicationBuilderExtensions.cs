namespace Microsoft.AspNetCore.Builder;
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseZigana(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseExceptionHandler();
        app.UseStatusCodePages();

        return app;
    }
}
