using Coreeple.Zigana.Core.Diagnostics;
using Coreeple.Zigana.Services.Abstractions;
using System.Diagnostics;

namespace Coreeple.Zigana.Services.Diagnostics;
public class ZiganaDiagnosticSubscriber(IEndpointLogService endpointLogService) : IObserver<DiagnosticListener>
{
    public void OnCompleted() { }
    public void OnError(Exception error) { }
    public void OnNext(DiagnosticListener listener)
    {
        if (listener.Name == ZiganaDiagnosticSource.SourceName)
        {
            listener.Subscribe(new ZiganaDiagnosticListener(endpointLogService));
        }
    }
}
