using Coreeple.Zigana.AspNet.Middlewares;
using Coreeple.Zigana.Data.Abstractions;
using Coreeple.Zigana.Services.Abstractions;
using Coreeple.Zigana.Services.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

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

        using var scope = app.ApplicationServices.CreateScope();
        var endpointLogService = scope.ServiceProvider.GetRequiredService<IEndpointLogService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();

        DiagnosticListener.AllListeners.Subscribe(new ZiganaDiagnosticSubscriber(endpointLogService));

        return app;
    }
}
