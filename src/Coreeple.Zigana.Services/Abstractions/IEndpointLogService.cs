using Coreeple.Zigana.Services.Dtos;

namespace Coreeple.Zigana.Services.Abstractions;
public interface IEndpointLogService
{
    Task AddTransactionAsync(EndpointRequestTransactionCreateDto dto);
}