using Zigana.Services.Abstractions;
using Zigana.Services.Dtos;
using System.Diagnostics;

namespace Zigana.Services.Diagnostics;
public class ZiganaDiagnosticListener(IEndpointLogService endpointLogService) : IObserver<KeyValuePair<string, object?>>
{
    public void OnCompleted() { }
    public void OnError(Exception error) { }
    public void OnNext(KeyValuePair<string, object?> value)
    {
        var activity = Activity.Current;

        if (activity is null) return;

        var endpointId = GetTagItem(activity, "endpoint.id");
        var requestId = GetTagItem(activity, "request.id");

        if (endpointId is null || requestId is null) return;

        var opt = value.Key.Split('.').Last();

        string status = "OK";
        string? message = null;

        foreach (var activityEvent in activity.Events)
        {
            if (activityEvent.Name == "exception")
            {
                status = "FAIL";
                message = activityEvent.Tags.FirstOrDefault(m => m.Key == "exception.message").Value?.ToString();
            }
            else if (activityEvent.Name == "action.skip")
            {
                status = "SKIP";
            }
        }

        endpointLogService.AddTransactionAsync(new EndpointRequestTransactionCreateDto()
        {
            Id = (Guid)requestId,
            EndpointId = (Guid)endpointId,
            Name = $"{activity.DisplayName}.{opt}",
            Status = status,
            Message = message,
            Date = DateTime.Now,
        });
    }

    private static object? GetTagItem(Activity? activity, string key)
    {
        if (activity is null) return default;

        var tag = activity.GetTagItem(key);
        if (tag is not null) return tag;

        return GetTagItem(activity.Parent, key);
    }
}
