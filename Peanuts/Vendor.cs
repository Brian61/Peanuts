using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Peanuts
{
    /// <summary>
    /// The <see cref="Peanuts"/> namespace contains an implementation of an
    /// Entity Component System (ECS) library.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// Instances of the Vendor class form the interface for access to Bag instances.
    /// In concert with the Process abstract base class, this class also helps to
    /// keep Process subtype instances aware of which Bag instances are of interest.
    /// </summary>
    [JsonConverter(typeof(VendorSerializer))]
    public sealed class Vendor
    {
        internal readonly Dictionary<int, Bag> BagsById;
        internal readonly List<Harvester> Harvesters;

        private void NotifyHarvesters(Bag bag)
        {
            foreach(var b in Harvesters)
                b.OnChangeBagMix(bag);
        }

        /// <summary>
        /// Creates a new Vendor instance.
        /// </summary>
        /// <param name="bagCapacity">Optional integer specifying the initial 
        /// capacity of the Bag dictionary.</param>
        /// <param name="harvesterCapacity">Optional integer specifying the 
        /// initial capacity of the Process dictionary.</param>
        public Vendor(int bagCapacity = 512, int harvesterCapacity = 64)
        {
            BagsById = new Dictionary<int, Bag>(bagCapacity);
            Harvesters = new List<Harvester>(harvesterCapacity);
        }

        private Bag MakeBag(params Nut[] nuts)
        {
            var rval = new Bag(nuts);
            BagsById[rval.Id] = rval;
            NotifyHarvesters(rval);
            return rval;
        }

        /// <summary>
        /// Create a new instance of a Bag containing Nut subtypes as specified in the given recipe.
        /// </summary>
        /// <param name="recipe">A Recipe instance describing the Nut subtypes.</param>
        /// <returns>A new Bag instance.</returns>
        public Bag MakeBag(Recipe recipe)
        {
            return MakeBag(recipe.ToNutArray());
        }

        /// <summary>
        /// Creates a new instance of Bag that is a copy of the provided prototype Bag.
        /// </summary>
        /// <param name="prototype">The Bag instance to be copied.</param>
        /// <returns>A new Bag instance.</returns>
        public Bag MakeBag(Bag prototype)
        {
            return MakeBag(prototype.GetAll().Select(p => p.Clone() as Nut).ToArray());
        }

        /// <summary>
        /// Creates a new instance of Bag containing default instances of the indicated Nut subtypes.
        /// </summary>
        /// <param name="nutTypes">Type objects for the Nut subtypes desired.</param>
        /// <returns>A new Bag instance.</returns>
        public Bag MakeBag(params Type[] nutTypes)
        {
            return MakeBag(nutTypes.Select(t => Activator.CreateInstance(t) as Nut).ToArray());
        }

        /// <summary>
        /// Get a Bag instance corresponding to an integer id.
        /// </summary>
        /// <param name="id">A valid integer id.</param>
        /// <returns>The corresponding Bag instance.</returns>
        public Bag Get(int id)
        {
            return BagsById[id];
        }

        /// <summary>
        /// Conditionally retrieve a Bag instance for an integer id that may,
        /// or may not, be contained in the Vendor instance.
        /// </summary>
        /// <param name="id">The integer id for the desired Bag.</param>
        /// <param name="bag">An out parameter to be filled in if the Bag is found.</param>
        /// <returns>True if id is present and bag is valid.</returns>
        public bool TryGet(int id, out Bag bag)
        {
            return BagsById.TryGetValue(id, out bag);
        }

        /// <summary>
        /// Get an enumeration of all Bag instances in this Vendor instance.
        /// </summary>
        /// <returns>An IEnumerable<Bag> instance.</Bag></returns>
        public IEnumerable<Bag> AllBags()
        {
            return BagsById.Values;
        }

        /// <summary>
        /// Adds a Nut subtype to a Bag instance.
        /// </summary>
        /// <param name="bag">The Bag instance to be modified.</param>
        /// <param name="nut">The Nut subtype instance to be added.</param>
        /// <returns>The modified Bag instance.</returns>
        public Bag Add(Bag bag, Nut nut)
        {
            bag.Add(nut);
            NotifyHarvesters(bag);
            return bag;
        }

        /// <summary>
        /// Removes a Nut subtype from a Bag instance.
        /// </summary>
        /// <param name="bag">The Bag instance to be modified.</param>
        /// <param name="nut">The Nut subtype instance to be removed.</param>
        /// <returns>The modified Bag instance.</returns>
        public Bag Remove(Bag bag, Nut nut)
        {
            bag.Remove(nut);
            NotifyHarvesters(bag);
            return bag;
        }

        /// <summary>
        /// Morph (change) the target Bag instance to have the same Nut subtypes as the 
        /// prototype Bag instance.  Nut subtype instances contained in both will not be
        /// modified.  Nut subtypes not included in the prototype will be removed from
        /// the target.  Nut subtypes present in the prototype but not in the target
        /// will be copied from the prototype to the target.
        /// </summary>
        /// <param name="target">The Bag instance to be modified.</param>
        /// <param name="prototype">A Bag instance serving as a template.</param>
        /// <returns>The modified Bag (target) instance.</returns>
        public Bag Morph(Bag target, Bag prototype)
        {
            target.Morph(prototype);
            NotifyHarvesters(target);
            return target;
        }

        /// <summary>
        /// Discard a Bag instance from this Vendor instance.  Remove all of its
        /// Nut subtypes, and notify all interested harvesteres of the change.
        /// </summary>
        /// <param name="bag">The Bag instance to be discarded.</param>
        public void Discard(Bag bag)
        {
            var bid = bag.Id;
            BagsById.Remove(bid);
            bag.ClearAll();
            NotifyHarvesters(bag);
        }

        /// <summary>
        /// Register a harvester with this Vendor instance so that it may recieve
        /// notifications of changes to Bag instances of interest.
        /// </summary>
        /// <param name="harvester">The Harvester instance to be registered.</param>
        public void Register(Harvester harvester)
        {
            Harvesters.Add(harvester);
            foreach (var bag in BagsById.Values)
                harvester.OnChangeBagMix(bag);
        }

        /// <summary>
        /// Unregister a harvester from this Vendor instance so that it will no longer
        /// recieve notification of Bag instance changes.
        /// </summary>
        /// <param name="harvester">The IProcess interface of the harvester to be unregistered.</param>
        public void Unregister(Harvester harvester)
        {
            Harvesters.Remove(harvester);
        }
    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    /// <exclude/>
    public class VendorSerializer : JsonConverter
    {
        /// <summary>
        /// For internal use only.
        /// </summary>
        /// <exclude/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vob = (Vendor)value;
            writer.WriteStartArray();
            writer.WriteValue(vob.BagsById.Count);
            writer.WriteValue(vob.Harvesters.Count);
            foreach (var bag in vob.BagsById.Values)
            {
                serializer.Serialize(writer, bag);
            }
            writer.WriteEndArray();
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        /// <exclude/>
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
                throw new JsonException("numHarvesters");
            int numHarvesters = int.Parse(reader.Value.ToString());
            var vob = new Vendor(numBags, numHarvesters);
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

        /// <summary>
        /// For internal use only.
        /// </summary>
        /// <exclude/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(Vendor).IsAssignableFrom(objectType);
        }
    }
}