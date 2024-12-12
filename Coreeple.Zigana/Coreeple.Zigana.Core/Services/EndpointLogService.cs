using Coreeple.Zigana.Core.Data.Repositories;

namespace Coreeple.Zigana.Core.Services;
public class EndpointLogService(IEndpointTransactionRepository endpointRequestRepository) : IEndpointLogService
{
    public async Task AddAsync(Guid endpointId, Guid requestId, string name,
        string status)
    {
        await endpointRequestRepository.AddAsync(Guid.NewGuid(), endpointId, requestId, name,
            DateTime.Now, status);
    }
}

public interface IEndpointLogService
{
    Task AddAsync(Guid endpointId, Guid requestId, string name, string status);
}