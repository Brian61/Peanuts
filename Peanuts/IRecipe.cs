using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peanuts
{
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
