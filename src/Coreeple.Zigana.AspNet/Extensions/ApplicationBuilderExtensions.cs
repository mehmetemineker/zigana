using Coreeple.Zigana.AspNet.Middlewares;
using Coreeple.Zigana.Core.Diagnostics;
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


        DiagnosticListener.AllListeners.Subscribe(new ZiganaDiagnosticSubscriber());

        return app;
    }
}
