using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

namespace Peanuts
{
    /// <summary>
    /// For internal use only.
    /// </summary>
    public static class JsonHelper
    {
        public static readonly JsonSerializerSettings OutputSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
        };

        public static readonly JsonSerializerSettings InputSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        };

        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, OutputSettings);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, InputSettings);
        }

        public static T DeepClone<T>(this T source)
        {
            return ReferenceEquals(source, null)
                ? default(T)
                : Deserialize<T>(Serialize(source));
        }
    }
}

public static class ObjectExtension
{
    public static object DeepCloneObject(this object objSource)
    {
        //Get the type of source object and create a new instance of that type
        var typeSource = objSource.GetType();
        var objTarget = Activator.CreateInstance(typeSource);

        //Get all the properties of source object type
        var propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        //Assign all source property to taget object 's properties
        foreach (var property in propertyInfo.Where(property => property.CanWrite))
        {
            //check whether property type is value type, enum or string type
            if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType == typeof(String))
            {
                property.SetValue(objTarget, property.GetValue(objSource, null), null);
            }
            //else property type is object/complex types, so need to recursively call this method until the end of the tree is reached
            else
            {
                var objPropertyValue = property.GetValue(objSource, null);
                property.SetValue(objTarget, objPropertyValue == null ? null 
                    : objPropertyValue.DeepCloneObject(), null);
            }
        }
        return objTarget;
    }
}

