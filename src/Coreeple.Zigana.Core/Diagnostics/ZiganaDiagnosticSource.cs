using System.Diagnostics;

namespace Coreeple.Zigana.Core.Diagnostics;
public static class ZiganaDiagnosticSource
{
    public static readonly DiagnosticSource Instance = new DiagnosticListener("Coreeple.Zigana");
}
