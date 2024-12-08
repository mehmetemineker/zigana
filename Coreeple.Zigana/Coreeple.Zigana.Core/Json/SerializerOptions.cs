using Coreeple.Zigana.Core.Json.Converters;
using System.Text.Json;

namespace Coreeple.Zigana.Core.Json;
public static class SerializerOptions
{
    public static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static readonly JsonSerializerOptions ActionConverterJsonSerializerOptions = new()
    {
        Converters = { new ActionJsonConverter() },
        PropertyNameCaseInsensitive = true,
    };
}
