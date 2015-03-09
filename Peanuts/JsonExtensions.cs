using System;
using System.IO;
using System.Json;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Peanuts
{
    static class JsonExtensions
    {
        /// <summary>
        /// Merge the target JsonObject into the prototype JsonObject to the indicated depth.
        /// Note that both target and prototype are treated as immutable.
        /// </summary>
        /// <param name="target">The 'derived' target.</param>
        /// <param name="prototype">The 'parent' prototype.</param>
        /// <param name="depth">The recursion depth, optional integer defaults to 0.</param>
        /// <returns>A new JsonObject containing the merged result.</returns>
        public static JsonObject MergeInto(this JsonObject target, JsonObject prototype, int depth = 0)
        {
            var merged = new JsonObject(prototype);
            foreach (var kvp in target)
            {
                var value = kvp.Value;
                JsonValue protoValue;
                if (depth > 0 && merged.TryGetValue(kvp.Key, out protoValue))
                {
                    var child = value as JsonObject;
                    var childProto = protoValue as JsonObject;
                    value = child.MergeInto(childProto, depth - 1);
                }
                merged[kvp.Key] = value;
            }
            return merged;
        }

        /// <summary>
        /// Transforms a JsonObject source into a Component of the named type using
        /// DataContractJsonSerializer.
        /// </summary>
        /// <param name="source">JsonObject</param>
        /// <param name="typeName">String</param>
        /// <returns>a new Component</returns>
        public static Component ToComponent(this JsonObject source, String typeName)
        {
            var ctype = Peanuts.GetType(typeName);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(source.ToString())))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var dcjser = new DataContractJsonSerializer(ctype);
                return dcjser.ReadObject(stream) as Component;
            }
        }
    }
}
