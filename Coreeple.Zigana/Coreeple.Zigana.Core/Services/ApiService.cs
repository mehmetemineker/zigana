using Coreeple.Zigana.Core.Data.Repositories;
using Coreeple.Zigana.Core.Json.Converters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Primitives;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coreeple.Zigana.Core.Services;
public class ApiService(IApiRepository apiRepository, IEndpointRepository endpointRepository) : IApiService
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        Converters = { new ActionJsonConverter() },
        PropertyNameCaseInsensitive = true,
    };

    public async Task<Types.Endpoint> FindEndpoint(HttpContext context)
    {
        var endpoints = await GetEndpoints();
        var path = context.Request.Path;
        var method = context.Request.Method;

        if (TryGetMatchingEndpoint(path, [.. endpoints.Keys], out var matchingEndpoint))
        {
            var key = $"{matchingEndpoint.Pattern.Split(':')[0]}:{method}";

            if (!endpoints.TryGetValue(key, out Guid endpointId))
            {
                throw new Exception("Method Not Allowed");
            }

            var endpoint = await GetEndpointById(endpointId);

            if (endpoint != null)
            {
                await SetRequestAsync(context, endpoint, matchingEndpoint.Parameters);

                return endpoint;
            }
        }

        throw new Exception("Not Found");
    }

    private async Task<Dictionary<string, Guid>> GetEndpoints()
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
                    paths.Add($"{api.Path}{endpoint.Path}:{endpoint.Method}", endpoint.Id);
                }
            }

            return paths;
        }

        return [];
    }

    private async Task<Types.Endpoint?> GetEndpointById(Guid id)
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

        return result;
    }

    private static async Task SetRequestAsync(HttpContext context, Types.Endpoint endpoint, Dictionary<string, object?> routeParameters)
    {
        var query = StringValuesToObject(context.Request.Query.ToDictionary());
        var headers = StringValuesToObject(context.Request.Headers.ToDictionary());
        var body = await new StreamReader(context.Request.Body, Encoding.UTF8).ReadToEndAsync();

        endpoint.Request = new Types.Request
        {
            Query = JsonNode.Parse(JsonSerializer.Serialize(query))?.AsObject(),
            Headers = JsonNode.Parse(JsonSerializer.Serialize(headers))?.AsObject(),
        };

        if (context.Request.ContentType == "application/json" && !string.IsNullOrEmpty(body))
        {
            endpoint.Request.Body = JsonNode.Parse(body);
        }

        if (routeParameters.Count > 0)
        {
            endpoint.Request.Route = JsonNode.Parse(JsonSerializer.Serialize(routeParameters))?.AsObject();
        }
    }

    private static Dictionary<string, object> StringValuesToObject(Dictionary<string, StringValues> values)
    {
        return values.Select(m => new KeyValuePair<string, object>(
                m.Key,
                m.Value.Count == 1 ? m.Value.ToString() : m.Value.ToArray()))
            .ToDictionary();
    }

    public static bool TryGetMatchingEndpoint(string path, HashSet<string> endpoints, out (string Pattern, Dictionary<string, object?> Parameters) matchingEndpoint)
    {
        var values = new RouteValueDictionary();

        foreach (var endpoint in endpoints)
        {
            var template = TemplateParser.Parse(endpoint.Split(':')[0]);
            var matcher = new TemplateMatcher(template, values);

            if (matcher.TryMatch(path, values))
            {
                matchingEndpoint = (endpoint, values.ToDictionary());

                return true;
            }
        }

        matchingEndpoint = (string.Empty, new Dictionary<string, object?>());

        return false;
    }
}

public interface IApiService
{
    Task<Types.Endpoint> FindEndpoint(HttpContext context);
}
