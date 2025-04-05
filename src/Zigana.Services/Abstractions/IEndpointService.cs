using Zigana.Services.Dtos;

namespace Zigana.Services.Abstractions;
public interface IEndpointService
{
    Task<Guid> CreateAsync(CreateEndpointDto dto);
    Task<EndpointDto> FindEndpointAsync(string path, string method, CancellationToken cancellationToken = default);
}

