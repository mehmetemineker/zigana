using Zigana.Core.Helpers;
using Zigana.Core.Types;
using Zigana.Data.Abstractions;
using Zigana.Data.Entities;
using Zigana.Services.Abstractions;
using Zigana.Services.Dtos;
using Zigana.Services.Exceptions;
using System.Text.Json;
using System.Text.Json.Nodes;
using Action = Zigana.Core.Types.Action;

namespace Zigana.Services;
public class EndpointService(IEndpointRepository endpointRepository) : IEndpointService
{
    public async Task<Guid> CreateAsync(CreateEndpointDto dto)
    {
        return await endpointRepository.InsertAsync(new Endpoint()
        {
            ApiId = dto.ApiId,
            Path = dto.Path,
            Method = dto.Method,
            Actions = dto.Actions,
            Response = dto.Response
        });
    }

    public async Task<EndpointDto> FindEndpointAsync(string path, string method, CancellationToken cancellationToken = default)
    {
        var endpoints = await endpointRepository.GetPathsAsync(cancellationToken);

        var paths = new Dictionary<string, Guid>();

        foreach (var endpoint in endpoints)
        {
            paths.Add($"{endpoint.Path}:{endpoint.Method}", endpoint.Id);
        }

        if (HttpHelpers.TryGetMatchingEndpoint(path, [.. paths.Keys], out var matchingEndpoint))
        {
            var key = $"{matchingEndpoint.Pattern.Split(':')[0]}:{method}";

            if (!paths.TryGetValue(key, out Guid endpointId))
            {
                throw new HttpMethodNotAllowedServiceException();
            }

            var endpoint = await endpointRepository.GetByIdWithApiAsync(endpointId, cancellationToken);

            var result = new EndpointDto()
            {
                Id = endpoint.Id,
                Path = endpoint.Path,
            };

            result.Request.Route = JsonNode.Parse(JsonSerializer.Serialize(matchingEndpoint.Parameters))?.AsObject() ?? [];

            if (endpoint.Response != null)
            {
                result.Responses = JsonSerializer.Deserialize<Dictionary<string, Response>>(endpoint.Response, JsonHelpers.DefaultJsonSerializerOptions);
            }

            if (endpoint.Defs != null)
            {
                result.Defs = JsonSerializer.Deserialize<JsonObject>(endpoint.Defs, JsonHelpers.DefaultJsonSerializerOptions) ?? [];
            }

            if (endpoint.Actions != null)
            {
                result.Actions = JsonSerializer.Deserialize<Dictionary<string, Action>>(endpoint.Actions, JsonHelpers.ActionConverterJsonSerializerOptions);
            }

            return result;
        }

        throw new EndpointNotFoundServiceException();
    }
}