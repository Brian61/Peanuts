using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace Peanuts
{
    /// <summary>
    /// IRecipe provider for JsonObject recipes.
    /// </summary>
    public sealed class JsonRecipe : IRecipe
    {
        /// <summary>
        /// The prototype keyword
        /// </summary>
        public const string PrototypeKeyword = "Prototype";

        /// <summary>
        /// The recipe name.
        /// </summary>
        public String Name { get; private set; }

        private RecipeBook _book;
        private JsonObject _data;

        /// <summary>
        /// Creates a new recipe with the supplied name using the supplied data.
        /// If the recipe contains the Prototype keyword, the book parameter
        /// must be set to a valid RecipeBook instance.
        /// </summary>
        /// <param name="name">string</param>
        /// <param name="data">JsonObject containing component type name : 
        /// component field (name:value) dictionary pairs.</param>
        /// <param name="book">RecipeBook - may be null if recipe does not contain the Prototype keyword.</param>
        public JsonRecipe(String name, JsonObject data, RecipeBook book = null)
        {
            Name = name;
            _data = data;
            _book = book;
            if (null == book && data.ContainsKey(PrototypeKeyword))
                throw new ArgumentNullException("book", "book is null and data contains Prototype");
        }

        private JsonObject MergePrototype()
        {
            if (!_data.ContainsKey(PrototypeKeyword))
                return _data;
            var protoName = (String)_data[PrototypeKeyword];
            var recipe = _book.GetRecipe(protoName);
            var proto = recipe as JsonRecipe; 
            JsonObject protoData = null;
            if (null != proto) {
                protoData = proto._data;
            } else {
                protoData = CreateFromComponents(recipe);
            }
            var derived = new JsonObject(_data.Where(kvp => kvp.Key != PrototypeKeyword));
            return derived.MergeInto(protoData, 1);           
        }

        private static JsonObject CreateFromComponents(IRecipe recipe)
        {
            var components = recipe.Components().ToDictionary(comp => comp.GetType().Name, comp => comp);
            var ser = new DataContractJsonSerializer(components.GetType(), new DataContractJsonSerializerSettings
            {
                KnownTypes = Peanuts.AllTypes(),
                UseSimpleDictionaryFormat = true
            });
            JsonObject result = null;
            using (var ms = new MemoryStream()) {
                ser.WriteObject(ms, components);
                ms.Seek(0, SeekOrigin.Begin);
                result = (JsonObject) JsonValue.Load(ms);
            }
            return result;
        }

        /// <summary>
        /// Get the enumeration of Component instances created using this recipe.
        /// </summary>
        /// <returns>
        /// IEnumerable(Component)
        /// </returns>
        public IEnumerable<Component> Components()
        {
            return MergePrototype().Select(kvp => ((JsonObject)kvp.Value).ToComponent(kvp.Key));
        }

        /// <summary>
        /// Loads a collection of JsonRecipes into a RecipeBook from a Stream.
        /// </summary>
        /// <param name="book">RecipeBook</param>
        /// <param name="stream">Stream containing Json recipe data as an object containing
        /// recipe_name: recipe_data pairs.</param>
        public static void LoadCollection(RecipeBook book, Stream stream)
        {
            JsonObject root = (JsonObject)JsonValue.Load(stream);
            foreach (var kvp in root)
            {
                var recipe = new JsonRecipe(kvp.Key, (JsonObject)kvp.Value, book);
                book.AddRecipe(recipe);
            }
        }
    }
}
