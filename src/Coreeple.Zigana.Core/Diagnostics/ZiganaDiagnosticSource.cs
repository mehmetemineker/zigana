using System.Diagnostics;

namespace Coreeple.Zigana.Core.Diagnostics;
public class ZiganaDiagnosticSource() : DiagnosticListener("Coreeple.Zigana")
{
    public const string SourceName = "Coreeple.Zigana";
    public const string EndpointOperationName = $"{SourceName}.Endpoint";
    public const string ActionOperationName = $"{SourceName}.Action";
}