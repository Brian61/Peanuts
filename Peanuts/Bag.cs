using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Peanuts
{
    /// <summary>
    /// A Bag holds a collection of Nuts.
    /// </summary>
    [JsonConverter(typeof(BagSerializer))]
    public sealed class Bag
    {
        private readonly Dictionary<Type, Nut> _nutsByType;
        internal readonly Mix Mask;

        /// <summary>
        /// The unique integer id of the Bag instance.
        /// </summary>
        public int Id { get; private set; }

        internal Dictionary<Type, Nut> GetDictionary()
        {
            return _nutsByType;
        }

        internal Bag(int id, Dictionary<Type, Nut> contents)
        {
            Id = id;
            Peanuts.BagIdGenerator.EnsureGreaterThan(id);
            _nutsByType = contents;
            Mask = new Mix(contents.Keys);
        }

        internal Bag(IEnumerable<Nut> nuts)
        {
            _nutsByType = new Dictionary<Type, Nut>();
            foreach (var p in nuts)
            {
                _nutsByType[p.GetType()] = p;
            }
            Mask = new Mix(_nutsByType.Keys);
            Id = Peanuts.BagIdGenerator.Next();
        }
 
        internal IEnumerable<Nut> GetAll()
        {
            return _nutsByType.Values;
        }

        /// <summary>
        /// Gets the Nut subtype T from the current Bag.
        /// </summary>
        /// <returns>A T (Nut subtype) instance.</returns>
        public T Get<T>() where T : Nut
        {
            return _nutsByType[typeof(T)] as T;
        }

        /// <summary>
        /// Checks for existance of an identified Nut type and if found gives the corresponding instance.
        /// </summary>
        /// <param name="nut">An out parameter to recieve the Nut derived instance if it exists.</param>
        /// <returns>True if an instance of the indicated Nut subtype exists and the out param is valid.</returns>
        public bool TryGet<T>(out T nut) where T : Nut
        {
            var type = typeof (T);
            if (_nutsByType.ContainsKey(type))
            {
                nut = _nutsByType[type] as T;
                return true;
            }
            nut = null;
            return false;
        }

        /// <summary>
        /// This is the 'key-fits-lock' test that checks to see if the indicated Mix describes a subset of the contents.
        /// </summary>
        /// <param name="key">A Mix object describing the Nut subtypes of interest.</param>
        /// <returns>True if all of the subtypes indicated by key exist in this Bag.</returns>
        public bool Contains(Mix key)
        {
            return key.KeyFitsLock(Mask);
        }

        internal void Add(Nut nut)
        {
            var nutType = nut.GetType();
            var pid = Peanuts.GetId(nutType);
            _nutsByType[nutType] = nut;
            Mask.Set(pid);
        }

        internal void Remove(Nut nut)
        {
            var nutType = nut.GetType();
            var pid = Peanuts.GetId(nutType);
            _nutsByType.Remove(nutType);
            Mask.Clear(pid);
        }

        internal void Morph(Bag prototype)
        {
            var proto = prototype.Mask;
            for (var i = 0; i < Peanuts.NumberOfTypes(); i++)
            {
                if (Mask.IsSet(i))
                {
                    if (proto.IsSet(i))
                        continue;
                    _nutsByType.Remove(Peanuts.GetType(i));
                    Mask.Clear(i);
                }
                else
                {
                    if (!proto.IsSet(i))
                        continue;
                    var nutType = Peanuts.GetType(i);
                    _nutsByType[nutType] = prototype._nutsByType[nutType].Clone() as Nut;
                    Mask.Set(i);
                }
            }
        }

        internal void ClearAll()
        {
            _nutsByType.Clear();
            Mask.ClearAll();
        }
    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    /// <exclude/>
    public class BagSerializer : JsonConverter
    {
        /// <summary>
        /// For internal use only.
        /// </summary>
        /// <exclude/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var bag = (Bag) value;
            writer.WriteStartObject();
            writer.WritePropertyName("BagId");
            writer.WriteValue(bag.Id);
            foreach (var kv in bag.GetDictionary())
            {
                var ptype = kv.Key;
                writer.WritePropertyName(ptype.Name);
                serializer.Serialize(writer, kv.Value, ptype);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        /// <exclude/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonException("Missing expected start object token");
            if (!reader.Read() || (reader.TokenType != JsonToken.PropertyName) 
                || (reader.Value as string != "BagId"))
                throw new JsonException("Missing expected BagId property name");
            if (!reader.Read() || (reader.TokenType != JsonToken.Integer))
                throw new JsonException("Missing expected BagId value");
            var bid = int.Parse(reader.Value.ToString()); 
            var dict = new Dictionary<Type, Nut>();

            while (reader.Read() && (reader.TokenType != JsonToken.EndObject))
            {

                if (reader.TokenType != JsonToken.PropertyName)
                    throw new JsonException("Missing expected property name");
                var keyName = (string) reader.Value;
                var ptype = Peanuts.GetType(keyName);
                reader.Read();
                var value = serializer.Deserialize(reader, ptype) as Nut;
                if (null == value)
                    throw new JsonException("Unable to deserialize nut");
                dict[ptype] = value;
            }
            if (reader.TokenType != JsonToken.EndObject)
                throw new JsonException("Unexpected end of stream in open object");
            return new Bag(bid, dict);
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        /// <exclude/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(Bag).IsAssignableFrom(objectType);
        }
    }
}