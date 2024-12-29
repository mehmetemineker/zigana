using Coreeple.Zigana.Data.Entities;

namespace Coreeple.Zigana.Data.Abstractions;
public interface IEndpointRequestTransactionRepository
{
    Task<Guid> InsertAsync(EndpointRequestTransaction endpointRequestTransaction);
}
