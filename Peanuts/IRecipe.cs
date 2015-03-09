using System;
using System.Collections.Generic;

namespace Peanuts
{
    /// <summary>
    /// The interface to Entity recipes.
    /// </summary>
    public interface IRecipe
    {
        /// <summary>
        /// The recipe name, readonly.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Get the enumeration of Component instances created using this recipe.
        /// </summary>
        /// <returns>IEnumerable(Component)</returns>
        IEnumerable<Component> Components();
    }
}
