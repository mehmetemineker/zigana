using Zigana.Data.Entities;

namespace Zigana.Data.Abstractions;
public interface IEndpointTransactionLogRepository
{
    Task<Guid> InsertAsync(EndpointTransactionLog endpointTransactionLog);
}
