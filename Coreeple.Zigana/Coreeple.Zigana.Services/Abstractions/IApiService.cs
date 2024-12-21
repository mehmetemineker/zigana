using Coreeple.Zigana.Services.Dtos;

namespace Coreeple.Zigana.Services.Abstractions;
public interface IApiService
{
    Task<Guid> InsertAsync(CreateApiDto dto);
}
