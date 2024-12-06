using Coreeple.Zigana.Core.Data.Entities;
using Coreeple.Zigana.Core.Data.Repositories;

namespace Coreeple.Zigana.Core.Data.Services;
public class EndpointService : IEndpointService
{
    private readonly IEndpointRepository _endpointRepository;

    public EndpointService(IEndpointRepository endpointRepository)
    {
        ArgumentNullException.ThrowIfNull(endpointRepository);

        _endpointRepository = endpointRepository;
    }

    public async Task<Endpoint?> GetEndpointById(Guid id)
    {
        return await _endpointRepository.GetEndpointById(id);
    }
}

public interface IEndpointService
{
    Task<Endpoint?> GetEndpointById(Guid id);
}