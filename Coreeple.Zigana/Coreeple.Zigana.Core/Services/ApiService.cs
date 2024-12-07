using Coreeple.Zigana.Core.Data.Repositories;
using Coreeple.Zigana.Core.Json.Converters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Data.Services;
public class ApiService(IApiRepository apiRepository, IEndpointRepository endpointRepository) : IApiService
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        Converters = { new ActionJsonConverter() },
        PropertyNameCaseInsensitive = true,
    };


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

    public async Task<Types.Endpoint?> GetEndpointById(Guid id, HttpRequest httpRequest)
    {
        var endpoint = await endpointRepository.GetByIdWithApi(id);

        if (endpoint == null)
        {
            return default;
        }

        var result = new Types.Endpoint
        {
            Id = endpoint.Id,
            Path = endpoint.Path,
        };

        if (endpoint.Response != null)
        {
            result.Response = JsonSerializer.Deserialize<IEnumerable<Types.Response>>(endpoint.Response, jsonSerializerOptions);
        }

        if (endpoint.Definitions != null)
        {
            result.Definitions = JsonSerializer.Deserialize<JsonObject>(endpoint.Definitions, jsonSerializerOptions);
        }

        if (endpoint.Actions != null)
        {
            result.Actions = JsonSerializer.Deserialize<IEnumerable<Types.Action>>(endpoint.Actions, jsonSerializerOptions);
        }

        result.Request = new Types.Request();

        var query = StringValuesToObject(httpRequest.Query.ToDictionary());
        var headers = StringValuesToObject(httpRequest.Headers.ToDictionary());
        var body = await new StreamReader(httpRequest.Body, Encoding.UTF8).ReadToEndAsync();

        result.Request.QueryParameters = JsonNode.Parse(JsonSerializer.Serialize(query))?.AsObject();
        result.Request.Headers = JsonNode.Parse(JsonSerializer.Serialize(headers))?.AsObject();

        if (httpRequest.ContentType == "application/json" && !string.IsNullOrEmpty(body))
        {
            result.Request.Body = JsonNode.Parse(body);
        }

        return result;
    }

    private static Dictionary<string, object> StringValuesToObject(Dictionary<string, StringValues> values)
    {
        return values.Select(m => new KeyValuePair<string, object>(
                m.Key,
                m.Value.Count == 1 ? m.Value.ToString() : m.Value.ToArray()))
            .ToDictionary();
    }

}

public interface IApiService
{
    Task<Dictionary<string, Guid>> GetEndpoints();
    Task<Types.Endpoint?> GetEndpointById(Guid id, HttpRequest httpRequest);
}
