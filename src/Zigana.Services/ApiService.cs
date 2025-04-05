using Zigana.Data.Abstractions;
using Zigana.Data.Entities;
using Zigana.Services.Abstractions;
using Zigana.Services.Dtos;

namespace Zigana.Services;
public class ApiService(IApiRepository apiRepository) : IApiService
{
    public async Task<Guid> InsertAsync(CreateApiDto dto)
    {
        return await apiRepository.InsertAsync(new Api
        {
            Path = dto.Path,
            Name = dto.Name,
            Description = dto.Description,
            Defs = dto.Defs,
        });
    }
}