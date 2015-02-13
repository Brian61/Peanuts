using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

namespace Peanuts
{
    /// <summary>
    /// For internal use only.
    /// </summary>
    public static class JsonHelper
    {
        public static readonly JsonSerializerSettings OutputSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
        };

        public static readonly JsonSerializerSettings InputSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        };

        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, OutputSettings);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, InputSettings);
        }

        public static T DeepClone<T>(this T source)
        {
            return ReferenceEquals(source, null)
                ? default(T)
                : Deserialize<T>(Serialize(source));
        }
    }
}