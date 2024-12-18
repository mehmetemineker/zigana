using Coreeple.Zigana.Core.Data.Repositories;
using Coreeple.Zigana.Core.Services.Dtos;

namespace Coreeple.Zigana.Core.Services;
public class ApiService(IApiRepository apiRepository) : IApiService
{
    public async Task<Guid> CreateAsync(CreateApiDto dto, CancellationToken cancellationToken = default)
    {
        return await apiRepository.CreateAsync(dto.Path, dto.Name, dto.Description, dto.Defs, cancellationToken);
    }
}

public interface IApiService
{
    Task<Guid> CreateAsync(CreateApiDto dto, CancellationToken cancellationToken = default);
}