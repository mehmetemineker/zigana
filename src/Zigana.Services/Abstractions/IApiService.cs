using Zigana.Services.Dtos;

namespace Zigana.Services.Abstractions;
public interface IApiService
{
    Task<Guid> InsertAsync(CreateApiDto dto);
}
