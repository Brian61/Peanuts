using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Peanuts
{
    /// <summary>
    /// This class holds a list of recipes which may be used to create Entity instances.
    /// Instances of this class may only be created through the static method Load.
    /// </summary>
    public sealed class RecipeBook
    {
        /// <summary>
        /// Keyword used in recipe book json source texts to indicate 'inheritance' 
        /// of a prototype recipe.
        /// </summary>
        public const string PrototypeKeyword = "Prototype";

        /// <summary>
        /// Create a new RecipeBook from a stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>A new RecipeBook instance.</returns>
        public static RecipeBook Load(Stream stream)
        {
            var book = new RecipeBook();
            InitializeCaches();
            ExtractBlueprints(stream);
            book.ProcessBlueprints();
            DiscardCaches();
            return book;
        }

        /// <summary>
        /// Test for existance of a recipe in this book.
        /// </summary>
        /// <param name="recipeName">The name of the recipe.</param>
        /// <returns>True if the recipe exists in this book.</returns>
        public bool Contains(string recipeName)
        {
            return _recipes.ContainsKey(recipeName);
        }

        /// <summary>
        /// Create a TagSet for the indicated recipe.
        /// </summary>
        /// <param name="recipeName">The name of the recipe.</param>
        /// <returns>A new TagSet.</returns>
        public TagSet GetTagSetFor(string recipeName)
        {
            return new TagSet(_recipes[recipeName].Keys);
        }

        internal IEnumerable<Component> GetComponentsFor(string recipeName)
        {
            return _recipes[recipeName].Values;
        }

        private RecipeBook()
        {
            _recipes = new Dictionary<string, IDictionary<Type, Component>>();
        }

        private static void InitializeCaches()
        {
            _blueprintCache = new Dictionary<string, JsonObject>();
            _memoryStreamCache = new Dictionary<JsonValue, MemoryStream>();
            _serializerCache = new Dictionary<Type, DataContractJsonSerializer>();
        }

        private static void DiscardCaches()
        {
            _blueprintCache = null;
            _memoryStreamCache = null;
            _serializerCache = null;
        }

        private static MemoryStream GetMemoryStreamFor(JsonValue val)
        {
            MemoryStream ms;
            if (!_memoryStreamCache.TryGetValue(val, out ms))
            {
                ms = new MemoryStream(Encoding.UTF8.GetBytes(val.ToString()));
                _memoryStreamCache[val] = ms;
            }
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private static DataContractJsonSerializer GetSerializerFor(Type ctype)
        {
            DataContractJsonSerializer ser;
            if (!_serializerCache.TryGetValue(ctype, out ser))
            {
                ser = new DataContractJsonSerializer(ctype);
                _serializerCache[ctype] = ser;
            }
            return ser;
        }

        private static void ExtractBlueprints(Stream stream)
        {
            JsonObject root = (JsonObject)JsonValue.Load(stream);
            foreach (var item in root)
            {
                _blueprintCache[item.Key] = (JsonObject)item.Value;
            }
        }

        private void ProcessBlueprints()
        {
            foreach(var blueprint in _blueprintCache)
            {
                ComponentsFor(blueprint.Key, blueprint.Value);
            }
        }

        private IDictionary<Type, Component> ComponentsFor(string name, JsonObject obj)
        {
            IDictionary<Type, Component> list;
            if (_recipes.TryGetValue(name, out list))
                return list;
            if (obj.ContainsKey(PrototypeKeyword))
            {
                var proto = (string)obj[PrototypeKeyword];
                list = new Dictionary<Type, Component>(ComponentsFor(proto, _blueprintCache[proto]));
            } else {
                list = new Dictionary<Type, Component>();
            }
            foreach(var item in obj) {
                if (item.Key == PrototypeKeyword) continue;
                Type ctype = Peanuts.GetType(item.Key);
                var ser = GetSerializerFor(ctype);
                var ms = GetMemoryStreamFor(item.Value);
                Component comp = (Component) ser.ReadObject(ms);
                Component proto;
                if (list.TryGetValue(ctype, out proto))
                {
                    var jobj = (JsonObject) item.Value;
                    var merged = (Component)proto.Clone();
                    foreach (var finfo in ctype.GetFields(BindingFlags.Public))
                    {
                        if (jobj.ContainsKey(finfo.Name))
                            finfo.SetValue(merged, finfo.GetValue(comp));
                    }
                    foreach (var pinfo in ctype.GetProperties(BindingFlags.Public))
                    {
                        if (jobj.ContainsKey(pinfo.Name))
                            pinfo.SetValue(merged, pinfo.GetValue(comp));
                    }
                    comp = merged;
                }
                list[ctype] = comp;
            }
            _recipes[name] = list;
            return list;
        }

        private readonly IDictionary<string, IDictionary<Type, Component>> _recipes;

        [ThreadStatic]
        private static IDictionary<string, JsonObject> _blueprintCache;

        [ThreadStatic]
        private static IDictionary<JsonValue, MemoryStream> _memoryStreamCache;

        [ThreadStatic]
        private static IDictionary<Type, DataContractJsonSerializer> _serializerCache;

    }
}
