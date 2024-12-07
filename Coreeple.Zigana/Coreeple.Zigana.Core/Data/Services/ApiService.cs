using Coreeple.Zigana.Core.Data.Repositories;

namespace Coreeple.Zigana.Core.Data.Services;
public class ApiService(IApiRepository apiRepository, IEndpointRepository endpointRepository) : IApiService
{
    public async Task<Dictionary<string, Guid>> GetEndpoints()
    {
        var endpointsTask = endpointRepository.GetAll();
        var apisTask = apiRepository.GetAll();

        await Task.WhenAll(endpointsTask, apisTask);

        var endpoints = await endpointsTask;
        var apis = await apisTask;

        if (apis != null && endpoints != null)
        {
            var paths = new Dictionary<string, Guid>();

            foreach (var endpoint in endpoints)
            {
                var api = apis.FirstOrDefault(x => x.Id == endpoint.ApiId);

                if (api != null)
                {
                    paths.Add($"{api.Path}{endpoint.Path}", endpoint.Id);
                }
            }

            return paths;
        }

        return [];
    }
}

public interface IApiService
{
    Task<Dictionary<string, Guid>> GetEndpoints();
}