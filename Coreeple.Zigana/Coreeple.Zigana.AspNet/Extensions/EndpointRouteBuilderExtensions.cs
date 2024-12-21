using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Routing;
public static class ZiganaApiEndpointRouteBuilderExtensions
{
    public static IEndpointConventionBuilder MapZiganaApi(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var routeGroup = endpoints.MapGroup("");

        //routeGroup.MapPost("/apis", async Task<Guid> ([FromServices] IApiService apiService, [FromBody] CreateApiDto dto) =>
        //{
        //    return await apiService.CreateAsync(dto);
        //}).ShortCircuit();

        //routeGroup.MapPost("/endpoints", async Task<Guid> ([FromServices] IEndpointService endpointService, [FromBody] CreateEndpointDto dto) =>
        //{
        //    return await endpointService.CreateAsync(dto);
        //}).ShortCircuit();

        return new IdentityEndpointsConventionBuilder(routeGroup);
    }

    // Wrap RouteGroupBuilder with a non-public type to avoid a potential future behavioral breaking change.
    private sealed class IdentityEndpointsConventionBuilder(RouteGroupBuilder inner) : IEndpointConventionBuilder
    {
        private IEndpointConventionBuilder InnerAsConventionBuilder => inner;
        public void Add(Action<EndpointBuilder> convention) => InnerAsConventionBuilder.Add(convention);
        public void Finally(Action<EndpointBuilder> finallyConvention) => InnerAsConventionBuilder.Finally(finallyConvention);
    }
}