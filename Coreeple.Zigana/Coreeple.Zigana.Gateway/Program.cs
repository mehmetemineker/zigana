using Coreeple.Zigana.Core.Data.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddZigana(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/test", async (HttpContext context, IApiService endpointService) =>
{
    var endpoints = await endpointService.GetEndpoints();

    var endpoint = await endpointService.GetEndpointById(endpoints.First().Value, context.Request);

    return Results.Ok(endpoint);
});

app.UseHttpsRedirection();

app.UseZigana();

app.Run();
