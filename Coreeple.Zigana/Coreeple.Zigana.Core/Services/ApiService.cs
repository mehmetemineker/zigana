using Coreeple.Zigana.Core.Data.Repositories;
using Coreeple.Zigana.Core.Services.Dtos;

namespace Coreeple.Zigana.Core.Services;
public class ApiService(IApiRepository apiRepository) : IApiService
{
    public async Task<Guid> CreateAsync(CreateApiDto dto, CancellationToken cancellationToken = default)
    {
        return await apiRepository.CreateAsync(new Data.Entities.Api
        {
            Path = dto.Path,
            Name = dto.Name,
            Description = dto.Description,
            Defs = dto.Defs,
        });
    }
}

public interface IApiService
{
    Task<Guid> CreateAsync(CreateApiDto dto, CancellationToken cancellationToken = default);
}