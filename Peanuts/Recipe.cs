using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Peanuts
{
    /// <summary>
    /// Contains recipe data.
    /// </summary>
    public sealed class Recipe
    {
        /// <summary>
        /// The name of the recipe.
        /// </summary>
        public string Name { get; internal set; }

        private readonly RecipeBook _book;
        private readonly string _prototype;
        private readonly Dictionary<string, JObject> _ingredients = new Dictionary<string, JObject>();

        internal Recipe(string name, RecipeBook book, JsonReader reader)
        {
            Name = name;
            _book = book;
            if (!reader.Read() || (reader.TokenType != JsonToken.StartObject))
                throw new JsonException("Missing expected start object token");
            while (reader.Read() && (reader.TokenType == JsonToken.PropertyName))
            {
                var propName = (string) reader.Value;
                if (propName.ToUpper() == "PROTOTYPE")
                {
                    if (!reader.Read() || (reader.TokenType != JsonToken.String)) break;
                    _prototype = (string) reader.Value;
                }
                else
                {
                    if (!reader.Read()) break;
                    _ingredients[propName] = JObject.Load(reader);
                }
            }
            if (reader.TokenType != JsonToken.EndObject)
                throw new JsonException("Unexpected end of input");
        }
        
        /// <summary>
        /// GetTagSet creates a new TagSet for the recipe.
        /// </summary>
        /// <returns>A new TagSet</returns>
        public TagSet GetTagSet()
        {
        	var types = new List<Type>();
        	FindTypes(types);
        	return new TagSet(types);
        }
        
        private void FindTypes(List<Type> types)
        {
        	if (!string.IsNullOrEmpty(_prototype))
        		_book.Get(_prototype).FindTypes(types);
        	foreach (var entry in _ingredients.Keys) {
        		types.Add(Peanuts.GetType(entry));
        	}
        }

        private void FillIn(IDictionary<string, Component> recipe)
        {
            if (recipe == null) throw new ArgumentNullException("recipe");
            if (!string.IsNullOrEmpty(_prototype))
                _book.Get(_prototype).FillIn(recipe);
            foreach (var entry in _ingredients)
            {
                var compType = Peanuts.GetType(entry.Key);
                var ingredient = (Component) entry.Value.ToObject(compType);
                recipe[entry.Key] = ingredient;
            }
        }

        internal Component[] ToNutArray()
        {
            var recipe = new Dictionary<string, Component>(_ingredients.Count);
            FillIn(recipe);
            return recipe.Values.ToArray();
        }

    }
}