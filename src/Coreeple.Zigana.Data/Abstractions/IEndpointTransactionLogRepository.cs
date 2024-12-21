using Coreeple.Zigana.Data.Entities;

namespace Coreeple.Zigana.Data.Abstractions;
public interface IEndpointTransactionLogRepository
{
    Task<Guid> InsertAsync(EndpointTransactionLog endpointTransactionLog);
}
