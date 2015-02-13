using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Peanuts
{
    /// <summary>
    /// This class holds a list of recipes which may be used to create Bag instances.
    /// Instances of this class may only be created through the static method Load.
    /// </summary>
    public sealed class RecipeBook
    {
        private readonly Dictionary<string, Recipe> _recipes = new Dictionary<string, Recipe>();

        private RecipeBook()
        {
        }

        /// <summary>
        /// Tests for and conditionally retrieves a Recipe instance for the given name.
        /// </summary>
        /// <param name="name">A string giving the name of the recipe</param>
        /// <param name="recipe">an out param of type Recipe to recieve a matching recipe.</param>
        /// <returns>True if a recipe matching name was found and a recipe supplied.</returns>
        public bool TryGet(string name, out Recipe recipe)
        {
            return _recipes.TryGetValue(name, out recipe);
        }

        /// <summary>
        /// Retrieves a Recipe instance for the given name.
        /// </summary>
        /// <param name="name">A string giving the recipe name</param>
        /// <returns>A recipe instance with the indicated name.</returns>
        public Recipe Get(string name)
        {
            return _recipes[name];
        }

        /// <summary>
        /// This static method is used to create a RecipeBook instance from a source of Json text.
        /// The Json text must take the form of an object containing one or more recipes.  Each
        /// recipe has its name as a key and the corresponding object value should contain a set
        /// of key/value pairs where the key is either the name of a Nut subtype or the special
        /// key 'Prototype'.  The value for Nut subtypes should be a Json object describing that
        /// subtype.  The value for Prototype should be the name of another recipe whose Nut subtypes
        /// will be included in the current recipe (this is recursive and must not be circular).
        /// </summary>
        /// <param name="source">A TextReader subtype for the Json source text</param>
        /// <returns>A new instance of RecipeBook containing the supplied recipes</returns>
        public static RecipeBook Load(TextReader source)
        {
            var book = new RecipeBook();
            using (var reader = new JsonTextReader(source))
            {
                if (!reader.Read() || (reader.TokenType != JsonToken.StartObject))
                    throw new JsonException("Missing expected start object token");
                while (reader.Read() && (reader.TokenType == JsonToken.PropertyName))
                {
                    var recipeName = (string) reader.Value;
                    var recipe = new Recipe(recipeName, book, reader);
                    book._recipes[recipeName] = recipe;
                }
                if (reader.TokenType != JsonToken.EndObject)
                    throw new JsonException("Unexpected end of input");
            }
            return book;
        }
    }
}