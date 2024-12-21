using Coreeple.Zigana.Core.Types;

namespace Coreeple.Zigana.EndpointProcessor.Abstractions;
public interface IResponseBuilder
{
    void Build(Dictionary<string, Response> response);
}