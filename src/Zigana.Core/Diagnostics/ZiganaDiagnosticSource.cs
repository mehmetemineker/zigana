using System.Diagnostics;

namespace Zigana.Core.Diagnostics;
public class ZiganaDiagnosticSource() : DiagnosticListener("Zigana")
{
    public const string SourceName = "Zigana";
    public const string EndpointOperationName = $"{SourceName}.Endpoint";
    public const string ActionOperationName = $"{SourceName}.Action";
}