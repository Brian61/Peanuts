using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

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
        /// <param name="force">Advanced usage only.  Will possibly invalidate all existing Bag 
        /// and Vendor instances.  Intended to be set true when this method is invoked after
        /// plugins have loaded additional Nut subtypes.
        /// </param>
        public static void Initialize(bool force = false)
        {
            if (force || !_initialized)
                _performInit();
        }

        /// <summary>
        /// IdGenerator serializeable with Json.Net
        /// Used to generate contextually unique identifier integers for Bag instances.
        /// </summary>
        public static IdGenerator BagIdGenerator = new IdGenerator();

        /// <summary>
        /// OutputSettings is an instance of JsonSerializerSettings for use with Json.Net.
        /// </summary>
        public static readonly JsonSerializerSettings OutputSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
        };

        /// <summary>
        /// InputSettings is an instance of JsonSerializerSettings for use with Json.Net.
        /// </summary>
        public static readonly JsonSerializerSettings InputSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        };

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
            foreach (var t in from asm in cd.GetAssemblies() from t in asm.GetTypes() where t.IsSealed && t.IsSubclassOf(typeof(Nut)) select t)
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
