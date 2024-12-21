using Coreeple.Zigana.Services.Dtos;

namespace Coreeple.Zigana.Services.Abstractions;
public interface IEndpointService
{
    Task<Guid> CreateAsync(CreateEndpointDto dto);
    Task<EndpointDto> FindEndpointAsync(string path, string method, CancellationToken cancellationToken = default);
}

