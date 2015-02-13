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
        private static int _maxBagId;

        private readonly Dictionary<int, Nut> _nutsById;
        private readonly Mix _mask;

        /// <summary>
        /// The unique integer id of the Bag instance.
        /// </summary>
        public int Id { get; private set; }

        internal Dictionary<int, Nut> GetDictionary()
        {
            return _nutsById;
        }

        internal Bag(int id, Dictionary<int, Nut> contents)
        {
            Id = id;
            if (id > _maxBagId)
                _maxBagId = id;
            _nutsById = contents;
            _mask = new Mix(contents.Keys);
        }

        internal Bag(params Nut[] nuts)
        {
            _nutsById = new Dictionary<int, Nut>();
            foreach (var p in nuts)
            {
                _nutsById[Nut.GetId(p.GetType())] = p;
            }
            _mask = new Mix(_nutsById.Keys);
            Id = ++_maxBagId;
        }
 
        internal Dictionary<int, Nut>.ValueCollection GetAll()
        {
            return _nutsById.Values;
        }

        /// <summary>
        /// Gets a Nut for a given identifier.
        /// </summary>
        /// <param name="nutId">The integer id of the Nut type</param>
        /// <returns>A Nut derived instance.</returns>
        public Nut Get(int nutId)
        {
            return _nutsById[nutId];
        }

        /// <summary>
        /// Checks for existance of an identified Nut type and if found gives the corresponding instance.
        /// </summary>
        /// <param name="nutId">The integer id of the Nut type</param>
        /// <param name="nut">An out parameter to recieve the Nut derived instance if it exists.</param>
        /// <returns>True if an instance of the indicated Nut subtype exists and the out param is valid.</returns>
        public bool TryGet(int nutId, out Nut nut)
        {
            return _nutsById.TryGetValue(nutId, out nut);
        }

        /// <summary>
        /// This is the 'key-fits-lock' test that checks to see if the indicated Mix describes a subset of the contents.
        /// </summary>
        /// <param name="key">A Mix object describing the Nut subtypes of interest.</param>
        /// <returns>True if all of the subtypes indicated by key exist in this Bag.</returns>
        public bool Contains(Mix key)
        {
            return key.IsSubsetOf(_mask);
        }

        internal void Add(Nut nut)
        {
            var pid = Nut.GetId(nut.GetType());
            _nutsById[pid] = nut;
            _mask.Set(pid);
        }

        internal void Remove(Nut nut)
        {
            var pid = Nut.GetId(nut.GetType());
            _nutsById.Remove(pid);
            _mask.Clear(pid);
        }

        internal void Morph(Bag prototype)
        {
            var proto = prototype._mask;
            for (var i = 0; i < Nut.NumberOfTypes(); i++)
            {
                if (_mask.IsSet(i))
                {
                    if (proto.IsSet(i))
                        continue;
                    _nutsById.Remove(i);
                    _mask.Clear(i);
                }
                else
                {
                    if (!proto.IsSet(i))
                        continue;
                    _nutsById[i] = prototype._nutsById[i].Clone();
                    _mask.Set(i);
                }
            }
        }

        internal void ClearAll()
        {
            _nutsById.Clear();
            _mask.ClearAll();
        }
    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    public class BagSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var gob = (Bag) value;
            writer.WriteStartObject();
            writer.WritePropertyName("BagId");
            writer.WriteValue(gob.Id);
            foreach (var p in gob.GetDictionary().Values)
            {
                var ptype = p.GetType();
                writer.WritePropertyName(ptype.Name);
                serializer.Serialize(writer, p, ptype);
            }
            writer.WriteEndObject();
        }

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
            var dict = new Dictionary<int, Nut>();

            while (reader.Read() && (reader.TokenType != JsonToken.EndObject))
            {

                if (reader.TokenType != JsonToken.PropertyName)
                    throw new JsonException("Missing expected property name");
                var keyName = (string) reader.Value;
                var ptype = Nut.GetType(keyName);
                var pid = Nut.GetId(ptype);
                reader.Read();
                var value = serializer.Deserialize(reader, ptype) as Nut;
                if (null == value)
                    throw new JsonException("Unable to deserialize nut");
                dict[pid] = value;
            }
            if (reader.TokenType != JsonToken.EndObject)
                throw new JsonException("Unexpected end of stream in open object");
            return new Bag(bid, dict);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Bag).IsAssignableFrom(objectType);
        }
    }
}