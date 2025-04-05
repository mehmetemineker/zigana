using Zigana.Data.Entities;

namespace Zigana.Data.Abstractions;
public interface IApiRepository
{
    Task<Guid> InsertAsync(Api api);
}
