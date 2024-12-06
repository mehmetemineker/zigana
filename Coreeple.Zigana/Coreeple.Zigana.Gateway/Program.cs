using Coreeple.Zigana.Core.Data.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddZigana(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/test", async (IEndpointService endpointService) =>
{
    var endpoint = await endpointService.GetEndpointById(Guid.Parse("bb2bedb9-7b4f-42db-80b4-1396282477bf"));
    return Results.Ok(endpoint);
});

app.UseHttpsRedirection();

app.UseZigana();

app.Run();
