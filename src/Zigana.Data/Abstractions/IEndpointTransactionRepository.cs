using Zigana.Data.Entities;

namespace Zigana.Data.Abstractions;
public interface IEndpointRequestTransactionRepository
{
    Task<Guid> InsertAsync(EndpointRequestTransaction endpointRequestTransaction);
}
