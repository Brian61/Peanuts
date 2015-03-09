using System;
using System.Collections.Generic;

namespace Peanuts
{
    /// <summary>
    /// This class holds a list of recipes which may be used to create Entity instances.
    /// </summary>
    public sealed class RecipeBook
    {
        private Dictionary<string, IRecipe> _recipes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeBook"/> class.
        /// </summary>
        public RecipeBook()
        {
            _recipes = new Dictionary<string, IRecipe>();
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
        /// Gets the recipe.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The recipe.</returns>
        public IRecipe GetRecipe(String name)
        {
            return _recipes[name];
        }

        /// <summary>
        /// Adds the recipe.
        /// </summary>
        /// <param name="recipe">The recipe.</param>
        public void AddRecipe(IRecipe recipe)
        {
            _recipes[recipe.Name] = recipe;
        }
    }
}
