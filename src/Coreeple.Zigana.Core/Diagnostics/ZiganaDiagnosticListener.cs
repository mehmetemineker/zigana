using System.Diagnostics;

namespace Coreeple.Zigana.Core.Diagnostics;
public class ZiganaDiagnosticListener : IObserver<KeyValuePair<string, object?>>
{
    public void OnCompleted() { }

    public void OnError(Exception error) { }
    public void OnNext(KeyValuePair<string, object?> value)
    {
        switch (value.Key)
        {
            case $"{ZiganaDiagnosticSource.EndpointOperationName}.Start":
                Console.WriteLine($"ZiganaEndpoint.Start - activity id: {Activity.Current?.Id}");
                break;
            case $"{ZiganaDiagnosticSource.EndpointOperationName}.Stop":
                Console.WriteLine($"ZiganaEndpoint.Stop - activity id: {Activity.Current?.Id}");
                break;
            default:
                break;
        }
    }
}


