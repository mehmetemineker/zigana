using Coreeple.Zigana.Data.Entities;

namespace Coreeple.Zigana.Data.Abstractions;
public interface IEndpointRepository
{
    Task<Guid> InsertAsync(Endpoint endpoint);
    Task<IEnumerable<Endpoint>> GetPathsAsync(CancellationToken cancellationToken = default);
}
