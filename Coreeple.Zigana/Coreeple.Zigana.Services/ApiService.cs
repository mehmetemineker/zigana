using Coreeple.Zigana.Data.Abstractions;
using Coreeple.Zigana.Data.Entities;
using Coreeple.Zigana.Services.Dtos;

namespace Coreeple.Zigana.Services;
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

public interface IApiService
{
    Task<Guid> InsertAsync(CreateApiDto dto);
}