using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
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

        /// <summary>
        /// Create a new RecipeBook.
        /// </summary>
        public RecipeBook()
        {
            _recipes = new Dictionary<string, IDictionary<Type, Component>>();
        }

        /// <summary>
        /// Create a new RecipeBook from a stream.
        /// </summary>
        /// <param name="stream">The Stream object containing a json recipe colleciton.</param>
        public RecipeBook(Stream stream) : this()
        {
            Load(stream);
        }

        /// <summary>
        /// Loads a RecipeBook from a Stream of Json encoded recipes.
        /// Will add to the current recipe dictionary and replace any with
        /// the same recipe name.
        /// </summary>
        /// <param name="stream">A Stream containing Json encoded recipes.</param>
        public void Load(Stream stream)
        {
            JsonObject root = (JsonObject)JsonValue.Load(stream);
            var list = root.Keys.ToList();
            list.ForEach(name => MergePrototype(name, root));
            list.ForEach(name => _recipes[name] = ComponentsFor(root[name]));
        }
 
        private static JsonValue MergeValueObjects(JsonValue proto, JsonValue derived)
        {
            var protoObj = proto as JsonObject;
            var derivedObj = derived as JsonObject;
            var merged = new JsonObject(protoObj);
            foreach (var kvp in derivedObj)
                merged[kvp.Key] = kvp.Value;
            return merged;
        }

        private static JsonObject MergePrototype(string name, JsonObject root)
        {
            var target = root[name] as JsonObject;
            JsonValue protoName;
            if (!target.TryGetValue(PrototypeKeyword, out protoName))
                return target;
            var proto = MergePrototype((string)protoName, root);
            target.Remove(PrototypeKeyword);
            var merged = new JsonObject(proto);
            foreach (var kvp in target)
            {
                JsonValue mval;
                merged[kvp.Key] = !merged.TryGetValue(kvp.Key, out mval) ? kvp.Value
                    : MergeValueObjects(mval, kvp.Value);
            }
            root[name] = merged;
            return merged;
        }

        private IDictionary<Type, Component> ComponentsFor(JsonValue recipe)
        {
            var obj = recipe as JsonObject;
            var dict = new Dictionary<Type, Component>();
            foreach (var kvp in obj)
            {
                var ctype = Peanuts.GetType(kvp.Key);
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(kvp.Value.ToString())))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    var dcjser = new DataContractJsonSerializer(ctype);
                    dict[ctype] = dcjser.ReadObject(stream) as Component;
                }
            }
            return dict;
        }

        private readonly IDictionary<string, IDictionary<Type, Component>> _recipes;
    }
}
