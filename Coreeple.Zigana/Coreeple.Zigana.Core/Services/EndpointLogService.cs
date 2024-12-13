using Coreeple.Zigana.Core.Data.Repositories;

namespace Coreeple.Zigana.Core.Services;
public class EndpointLogService(IEndpointTransactionRepository endpointRequestRepository) : IEndpointLogService
{
    public void Add(Guid endpointId, Guid requestId, string name,
        string status)
    {
        endpointRequestRepository.AddAsync(Guid.NewGuid(), endpointId, requestId, name,
           DateTime.Now, status);
    }
}

public interface IEndpointLogService
{
    void Add(Guid endpointId, Guid requestId, string name, string status);
}