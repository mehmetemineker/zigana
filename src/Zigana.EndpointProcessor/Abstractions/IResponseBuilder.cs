using Zigana.Core.Types;

namespace Zigana.EndpointProcessor.Abstractions;
public interface IResponseBuilder
{
    void Build(Dictionary<string, Response> response);
}