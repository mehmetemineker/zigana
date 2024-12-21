using Coreeple.Zigana.Data.Abstractions;
using Coreeple.Zigana.Data.Entities;
using Coreeple.Zigana.Services.Dtos;

namespace Coreeple.Zigana.Services;
public class EndpointLogService(
    IEndpointTransactionRepository endpointRequestRepository,
    IEndpointTransactionLogRepository endpointTransactionLogRepository) : IEndpointLogService
{
    public Guid ScopeId { get; } = Guid.NewGuid();

    public async Task AddTransactionAsync(EndpointTransactionCreateDto dto)
    {
        await endpointRequestRepository.InsertAsync(new EndpointTransaction()
        {
            Id = ScopeId,
            EndpointId = dto.EndpointId,
            RequestId = dto.RequestId,
            Name = dto.Name,
            Status = dto.Status,
            Date = DateTime.Now,
        });
    }

    public async Task AddLogAsync(string level, string log)
    {
        await endpointTransactionLogRepository.InsertAsync(new EndpointTransactionLog()
        {
            Id = Guid.NewGuid(),
            TransactionId = ScopeId,
            Level = level,
            Log = log,
            Date = DateTime.Now,
        });
    }
}

public interface IEndpointLogService
{
    Task AddTransactionAsync(EndpointTransactionCreateDto dto);
    Task AddLogAsync(string level, string log);
}