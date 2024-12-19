using Coreeple.Zigana.Core.Data.Repositories;
using Coreeple.Zigana.Core.Json;
using Coreeple.Zigana.Core.Services.Dtos;
using Coreeple.Zigana.Core.Types;
using Coreeple.Zigana.Core.Utils;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Services;
public class EndpointService(IApiRepository apiRepository, IEndpointRepository endpointRepository) : IEndpointService
{
    public async Task<Guid> CreateAsync(CreateEndpointDto dto, CancellationToken cancellationToken = default)
    {
        return await endpointRepository.CreateAsync(dto.ApiId, dto.Path, dto.Method, dto.Actions, dto.Response, cancellationToken);
    }

    public async Task<Endpoint> FindEndpointAsync(Microsoft.AspNetCore.Http.HttpContext context, CancellationToken cancellationToken = default)
    {
        var endpoints = await GetEndpointsAsync(cancellationToken);
        var path = context.Request.Path;
        var method = context.Request.Method;

        if (HttpUtils.TryGetMatchingEndpoint(path, [.. endpoints.Keys], out var matchingEndpoint))
        {
            var key = $"{matchingEndpoint.Pattern.Split(':')[0]}:{method}";

            if (!endpoints.TryGetValue(key, out Guid endpointId))
            {
                throw new Exception("Method Not Allowed");
            }

            var endpoint = await GetEndpointByIdAsync(endpointId, cancellationToken);

            if (endpoint != null)
            {
                await SetRequestAsync(context, endpoint, matchingEndpoint.Parameters, cancellationToken);

                return endpoint;
            }
        }

        throw new Exception("Not Found");
    }

    private async Task<Dictionary<string, Guid>> GetEndpointsAsync(CancellationToken cancellationToken = default)
    {
        var endpointsTask = endpointRepository.GetAllPathsAsync(cancellationToken);
        var apisTask = apiRepository.GetAllPathsAsync(cancellationToken);

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
                    paths.Add($"{api.Path}{endpoint.Path}:{endpoint.Method}", endpoint.Id);
                }
            }

            return paths;
        }

        return [];
    }

    private async Task<Endpoint?> GetEndpointByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var endpoint = await endpointRepository.GetByIdWithApiAsync(id, cancellationToken);

        if (endpoint == null)
        {
            return default;
        }

        var result = new Endpoint
        {
            Id = endpoint.Id,
            Path = endpoint.Path,
        };

        if (endpoint.Response != null)
        {
            result.Responses = JsonSerializer.Deserialize<Dictionary<string, Response>>(endpoint.Response, CustomJsonSerializerOptions.DefaultJsonSerializerOptions);
        }

        if (endpoint.Defs != null)
        {
            result.Defs = JsonSerializer.Deserialize<JsonObject>(endpoint.Defs, CustomJsonSerializerOptions.DefaultJsonSerializerOptions) ?? [];
        }

        if (endpoint.Actions != null)
        {
            result.Actions = JsonSerializer.Deserialize<Dictionary<string, Types.Action>>(endpoint.Actions, CustomJsonSerializerOptions.ActionConverterJsonSerializerOptions);
        }

        return result;
    }

    private static async Task SetRequestAsync(
        Microsoft.AspNetCore.Http.HttpContext context,
        Endpoint endpoint,
        Dictionary<string, object?> routeParameters,
        CancellationToken cancellationToken = default)
    {
        var query = HttpUtils.StringValuesToObject(context.Request.Query.ToDictionary());
        var headers = HttpUtils.StringValuesToObject(context.Request.Headers.ToDictionary());
        var body = await new StreamReader(context.Request.Body, Encoding.UTF8).ReadToEndAsync(cancellationToken);

        endpoint.Request = new Request
        {
            Query = JsonNode.Parse(JsonSerializer.Serialize(query))?.AsObject() ?? [],
            Headers = JsonNode.Parse(JsonSerializer.Serialize(headers))?.AsObject() ?? [],
        };

        if (context.Request.ContentType == "application/json" && !string.IsNullOrEmpty(body))
        {
            endpoint.Request.Body = JsonNode.Parse(body) ?? JsonNode.Parse("{}")!;
        }

        if (routeParameters.Count > 0)
        {
            endpoint.Request.Route = JsonNode.Parse(JsonSerializer.Serialize(routeParameters))?.AsObject() ?? [];
        }

        if (Guid.TryParse(context.TraceIdentifier, out var requestId))
        {
            endpoint.RequestId = requestId;
        }
    }
}

public interface IEndpointService
{
    Task<Guid> CreateAsync(CreateEndpointDto dto, CancellationToken cancellationToken = default);
    Task<Endpoint> FindEndpointAsync(Microsoft.AspNetCore.Http.HttpContext context, CancellationToken cancellationToken = default);
}
