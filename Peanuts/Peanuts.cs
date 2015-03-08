using System;
using System.Collections.Generic;
using System.Linq;

namespace Peanuts
{
    /// <summary>
    /// Static interface to peanuts library.
    /// </summary>
    public static class Peanuts
    {
        /// <summary>
        /// This method *must* be called prior to any other use of the libary.
        /// </summary>
        /// <param name="force">Advanced usage only.  Will possibly invalidate all existing Entity 
        /// and Group instances.  Intended to be set true when this method is invoked after
        /// plugins have loaded additional Component subtypes.
        /// </param>
        public static void Initialize(bool force = false)
        {
            if (force || !_initialized)
                _performInit();
        }

        /// <summary>
        /// IdGenerator used to generate contextually unique identifier integers for Entity instances.
        /// </summary>
        public static IdGenerator EntityIdGenerator = new IdGenerator();

        /// <summary>
        /// Extension method to convert enumerables of Entities to arrays of EntityIds
        /// </summary>
        /// <param name="entities">An enumerable Entity collection.</param>
        /// <returns>An array of integer Entity ids.</returns>
        public static int[] ToIdArray(this IEnumerable<Entity> entities)
        {
            return entities.Select(e => e.Id).ToArray();
        }

        /// <summary>
        /// Extension method to convert enumerables of Entity Ids to arrays of Entities.
        /// </summary>
        /// <param name="entityIds">An enumerable Entity Id collection.</param>
        /// <param name="group">The Group instance managing the Entities.</param>
        /// <returns>An array of Entities.</returns>
        public static Entity[] ToEntityArray(this IEnumerable<int> entityIds, Group group)
        {
            return entityIds.Select(id => group.Get(id)).ToArray();
        }

        internal static int NumberOfTypes()
        {
            return _types.Count;
        }

        internal static int GetId(Type type)
        {
            return _idByType[type];
        }

        internal static Type GetType(string name)
        {
            return _typeByName[name];
        }

        internal static Type GetType(int i)
        {
            return _types[i];
        }

        /// <summary>
        /// AllTypes is for use with DataContract serializers to extract known type information.
        /// </summary>
        /// <returns>An enumeration of all Component subtypes.</returns>
        public static IEnumerable<Type> AllTypes()
        {
            return _types;
        }

        private const int InitCollSize = 64;
        private static Dictionary<string, Type> _typeByName;
        private static Dictionary<Type, int> _idByType;
        private static List<Type> _types;
        private static bool _initialized;

        private static void _performInit()
        {
            _types = new List<Type>(InitCollSize);
            _typeByName = new Dictionary<string, Type>(InitCollSize);
            _idByType = new Dictionary<Type, int>(InitCollSize);
            var cd = AppDomain.CurrentDomain;
            foreach (var t in from asm in cd.GetAssemblies() from t in asm.GetTypes() where t.IsSealed && t.IsSubclassOf(typeof(Component)) select t)
            {
                var id = _types.Count;
                _types.Add(t);
                _typeByName[t.Name] = t;
                _idByType[t] = id;
            }
            _initialized = true;
        }
    }
}
