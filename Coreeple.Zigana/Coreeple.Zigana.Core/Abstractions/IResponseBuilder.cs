using Coreeple.Zigana.Core.Types;

namespace Coreeple.Zigana.Core.Abstractions;
public interface IResponseBuilder
{
    void Build(Dictionary<string, Response> response);
}