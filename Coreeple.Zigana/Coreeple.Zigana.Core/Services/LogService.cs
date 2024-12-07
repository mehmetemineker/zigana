using Coreeple.Zigana.Core.Data.Repositories;

namespace Coreeple.Zigana.Core.Services;
public class LogService(IEndpointRequestRepository endpointRequestRepository) : ILogService
{
    public async Task BeginAsync(Guid id, Guid endpointId, CancellationToken cancellationToken = default)
    {
        await endpointRequestRepository.BeginAsync(id, endpointId, DateTime.Now, cancellationToken);
    }

    public async Task EndAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await endpointRequestRepository.EndAsync(id, DateTime.Now, cancellationToken);
    }
}

public interface ILogService
{
    Task BeginAsync(Guid id, Guid endpointId, CancellationToken cancellationToken = default);
    Task EndAsync(Guid id, CancellationToken cancellationToken = default);
}