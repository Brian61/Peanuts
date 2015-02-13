using System;
using System.Collections.Generic;
using System.Linq;

namespace Peanuts
{
    /// <summary>
    /// The abstract base class for all data objects using this library.
    /// </summary>
    public abstract class Nut
    {
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
            foreach (var t in from asm in cd.GetAssemblies() from t in asm.GetTypes() where t.IsSealed && t.IsSubclassOf(typeof (Nut)) select t)
            {
                var id = _types.Count;
                _types.Add(t);
                _typeByName[t.Name] = t;
                _idByType[t] = id;
            }
            _initialized = true;
        }

        /// <summary>
        /// This method *must* be called prior to any other use of the libary.
        /// </summary>
        /// <param name="force">Advanced usage only.  Will possibly invalidate all existing Bag 
        /// and Vendor instances.  Intended to be set true when this method is invoked after
        /// plugins have loaded additional Nut subtypes.
        /// </param>
        public static void Initialize(bool force = false)
        {
            if (force || !_initialized)
                _performInit();
        }

        internal static int NumberOfTypes()
        {
            return _types.Count;
        }

        //public static Type GetType(int id)
        //{
        //    return _types[id];
        //}

        internal static Type GetType(string name)
        {
            return _typeByName[name];
        }

        /// <summary>
        /// Get the unique integer identifier for a given Nut subtype.
        /// </summary>
        /// <param name="type">A Type object for the given Nut subtype.</param>
        /// <returns>The unique integer identifier for the indicated Nut subtype.</returns>
        public static int GetId(Type type)
        {
            return _idByType[type];
        }

        /// <summary>
        /// Get the unique integer identifier for a given Nut subtype.
        /// </summary>
        /// <param name="name">The string name of a Nut subtype.</param>
        /// <returns>The unique integer identifier for the indicated Nut subtype.</returns>
        public static int GetId(string name)
        {
            return _idByType[_typeByName[name]];
        }

        //public static string GetName(int id)
        //{
        //    return _types[id].Name;
        //}

        protected Nut()
        {
        }

        /// <summary>
        /// This method must be supplied by Nut subtypes and should return a copy of the subtype instance.
        /// Users of the library should use either the ShallowNutAbc or DeepNutAbc class as a base class 
        /// instead of deriving directly from Nut to avoid needing to implement this method.
        /// </summary>
        /// <returns></returns>
        public abstract Nut Clone();
    }

    /// <summary>
    /// A base class for Nut subtypes that satisfies the Clone contract using shallow copy.
    /// </summary>
    public abstract class ShallowNutAbc : Nut
    {
        public sealed override Nut Clone()
        {
            return MemberwiseClone() as Nut;
        }
    }

    /// <summary>
    /// A base class for Nut subtypes that satisfies the Clone contract using deep copy.
    /// </summary>
    public abstract class DeepNutAbc : Nut
    {
        public sealed override Nut Clone()
        {
            return this.DeepClone();
        }
    }
}