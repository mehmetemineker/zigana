using Zigana.Data.Abstractions;
using Zigana.Data.Entities;
using Zigana.Services.Abstractions;
using Zigana.Services.Dtos;

namespace Zigana.Services;
public class EndpointLogService(IEndpointRequestTransactionRepository endpointRequestTransactionRepository)
    : IEndpointLogService
{
    public async Task AddTransactionAsync(EndpointRequestTransactionCreateDto dto)
    {
        await endpointRequestTransactionRepository.InsertAsync(new EndpointRequestTransaction()
        {
            Id = dto.Id,
            EndpointId = dto.EndpointId,
            Name = dto.Name,
            Status = dto.Status,
            Message = dto.Message,
            Date = dto.Date,
        });
    }
}