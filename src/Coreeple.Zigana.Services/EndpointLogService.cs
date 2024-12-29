using Coreeple.Zigana.Data.Abstractions;
using Coreeple.Zigana.Data.Entities;
using Coreeple.Zigana.Services.Abstractions;
using Coreeple.Zigana.Services.Dtos;

namespace Coreeple.Zigana.Services;
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