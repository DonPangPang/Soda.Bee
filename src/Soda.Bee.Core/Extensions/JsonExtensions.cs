using Newtonsoft.Json;

namespace Soda.Bee.Core.Extensions;

public static class JsonExtensions
{
    public static T? ToObject<T>(this string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static string ToJson(this object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
}