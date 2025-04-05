using Zigana.Services.Dtos;

namespace Zigana.Services.Abstractions;
public interface IEndpointLogService
{
    Task AddTransactionAsync(EndpointRequestTransactionCreateDto dto);
}