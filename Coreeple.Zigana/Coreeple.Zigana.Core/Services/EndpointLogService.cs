using Coreeple.Zigana.Core.Data.Repositories;

namespace Coreeple.Zigana.Core.Services;
public class EndpointLogService(IEndpointTransactionRepository endpointRequestRepository,
    IEndpointTransactionLogRepository endpointTransactionLogRepository) : IEndpointLogService
{
    public Guid ScopeId { get; } = Guid.NewGuid();

    public void AddTransaction(Guid endpointId, Guid requestId, string name, string status)
    {
        endpointRequestRepository.AddAsync(ScopeId, endpointId, requestId, name, status, DateTime.Now);
    }

    public void AddLog(string log)
    {
        endpointTransactionLogRepository.AddAsync(Guid.NewGuid(), ScopeId, log);
    }
}

public interface IEndpointLogService
{
    void AddTransaction(Guid endpointId, Guid requestId, string name, string status);
    void AddLog(string log);
}