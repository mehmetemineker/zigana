using Coreeple.Zigana.Data.Entities;

namespace Coreeple.Zigana.Data.Abstractions;
public interface IEndpointTransactionRepository
{
    Task<Guid> InsertAsync(EndpointTransaction endpointTransaction);
}
