using Coreeple.Zigana.Data.Entities;

namespace Coreeple.Zigana.Data.Abstractions;
public interface IApiRepository
{
    Task<Guid> InsertAsync(Api api);
    Task<IEnumerable<Api>> GetPathsAsync(CancellationToken cancellationToken = default);
}
