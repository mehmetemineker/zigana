using Coreeple.Zigana.Core.Data.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddZigana(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/test", async (IApiService endpointService) =>
{
    var endpoints = await endpointService.GetEndpoints();
    return Results.Ok(endpoints);
});

app.UseHttpsRedirection();

app.UseZigana();

app.Run();
