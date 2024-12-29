using System.Diagnostics;

namespace Coreeple.Zigana.Core.Diagnostics;
public class ZiganaDiagnosticSubscriber : IObserver<DiagnosticListener>
{
    public void OnCompleted() { }
    public void OnError(Exception error) { }

    public void OnNext(DiagnosticListener listener)
    {
        if (listener.Name == ZiganaDiagnosticSource.SourceName)
        {
            listener.Subscribe(new ZiganaDiagnosticListener());
        }
    }
}
