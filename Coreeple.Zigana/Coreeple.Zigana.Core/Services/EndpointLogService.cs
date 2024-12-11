using Coreeple.Zigana.Core.Data.Repositories;

namespace Coreeple.Zigana.Core.Services;
public class EndpointLogService(IEndpointTransactionRepository endpointRequestRepository) : IEndpointLogService
{
    public async Task AddAsync(Guid endpointId, Guid requestId, string type, string name,
        string status, CancellationToken cancellationToken = default)
    {
        await endpointRequestRepository.AddAsync(Guid.NewGuid(), endpointId, requestId, type, name,
            DateTime.Now, status, cancellationToken);
    }
}

public interface IEndpointLogService
{
    Task AddAsync(Guid endpointId, Guid requestId, string type, string name,
        string status, CancellationToken cancellationToken = default);
}