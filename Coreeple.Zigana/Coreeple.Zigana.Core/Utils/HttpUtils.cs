using Microsoft.Extensions.Primitives;

namespace Coreeple.Zigana.Core.Utils;
public class HttpUtils
{
    public static Dictionary<string, object> StringValuesToObject(Dictionary<string, List<string>> values)
    {
        return values.Select(m => new KeyValuePair<string, object>(
                m.Key,
                m.Value.Count == 1 ? m.Value.First() : m.Value.ToArray()))
            .ToDictionary();
    }

    public static Dictionary<string, object> StringValuesToObject(Dictionary<string, StringValues> values)
    {
        return StringValuesToObject(values.ToDictionary(m => m.Key, m => m.Value.Select(m => m!).ToList()));
    }

    public static Dictionary<string, object> StringValuesToObject(Dictionary<string, IEnumerable<string>> values)
    {
        return StringValuesToObject(values);
    }
}
