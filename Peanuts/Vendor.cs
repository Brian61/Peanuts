using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Peanuts
{
    [JsonConverter(typeof(VendorSerializer))]
    public sealed class Vendor
    {
        internal readonly Dictionary<int, Bag> BagsById;
        internal readonly List<IProcess> Processes;

        private void NotifyProcesses(Bag bag)
        {
            foreach(var b in Processes)
                b.OnChangeBagMix(bag);
        }

        public Vendor(int bagCapacity = 512, int processCapacity = 64)
        {
            BagsById = new Dictionary<int, Bag>(bagCapacity);
            Processes = new List<IProcess>(processCapacity);
        }

        private Bag MakeBag(params Nut[] nuts)
        {
            var rval = new Bag(nuts);
            BagsById[rval.Id] = rval;
            NotifyProcesses(rval);
            return rval;
        }

        public Bag MakeBag(Recipe recipe)
        {
            return MakeBag(recipe.ToNutArray());
        }

        public Bag MakeBag(Bag prototype)
        {
            return MakeBag(prototype.GetAll().Select(p => p.Clone() as Nut).ToArray());
        }

        public Bag MakeBag(params Type[] nutTypes)
        {
            return MakeBag(nutTypes.Select(t => Activator.CreateInstance(t) as Nut).ToArray());
        }

        public Bag Get(int id)
        {
            return BagsById[id];
        }

        public bool TryGet(int id, out Bag bag)
        {
            return BagsById.TryGetValue(id, out bag);
        }

        public IEnumerable<Bag> AllBags()
        {
            return BagsById.Values;
        }

        public Bag Add(Bag bag, Nut nut)
        {
            bag.Add(nut);
            NotifyProcesses(bag);
            return bag;
        }

        public Bag Remove(Bag bag, Nut nut)
        {
            bag.Remove(nut);
            NotifyProcesses(bag);
            return bag;
        }

        public Bag Morph(Bag target, Bag prototype)
        {
            target.Morph(prototype);
            NotifyProcesses(target);
            return target;
        }

        public void Discard(Bag bag)
        {
            var bid = bag.Id;
            BagsById.Remove(bid);
            bag.ClearAll();
            NotifyProcesses(bag);
        }

        public void Register(IProcess process)
        {
            Processes.Add(process);
            foreach (var bag in BagsById.Values)
                process.OnChangeBagMix(bag);
        }

        public void Unregister(IProcess process)
        {
            Processes.Remove(process);
        }
    }

    public class VendorSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vob = (Vendor)value;
            writer.WriteStartArray();
            writer.WriteValue(vob.BagsById.Count);
            writer.WriteValue(vob.Processes.Count);
            foreach (var bag in vob.BagsById.Values)
            {
                serializer.Serialize(writer, bag);
            }
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartArray)
                throw new JsonException("Expected ArrayStart got: " + reader.TokenType);
            //if (!reader.Read() || (reader.TokenType != JsonToken.StartArray))
            //    throw new JsonException("Missing expected start array token" + reader.TokenType);
            if (!reader.Read() || (reader.TokenType != JsonToken.Integer))
                throw new JsonException("numBags");
            int numBags = int.Parse(reader.Value.ToString());
            if (!reader.Read() || (reader.TokenType != JsonToken.Integer))
                throw new JsonException("numProcesses");
            int numProcesses = int.Parse(reader.Value.ToString());
            var vob = new Vendor(numBags, numProcesses);
            var bags = vob.BagsById;
            while (reader.Read() && (reader.TokenType != JsonToken.EndArray))
            {
                var bag = serializer.Deserialize<Bag>(reader);
                if (null == bag)
                    throw new JsonException("Unable to deserialize bag");
                bags[bag.Id] = bag;
            }
            if (reader.TokenType != JsonToken.EndArray)
                throw new JsonException("Unexpected end of stream in open array");
            return vob;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Vendor).IsAssignableFrom(objectType);
        }
    }
}