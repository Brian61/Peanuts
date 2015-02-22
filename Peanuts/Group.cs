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
    /// Instances of the Group class form the interface for access to a collection
    /// of related Entity instances.
    /// </summary>
    [JsonConverter(typeof(GroupSerializer))]
    public sealed class Group : IEnumerable<Entity>
    {
        internal readonly SortedDictionary<int, Entity> EntitiesById;

        /// <summary>
        /// Creates a new Group instance.
        /// </summary>
        public Group()
        {
            EntitiesById = new SortedDictionary<int, Entity>();
        }

        /// <summary>
        /// Create a new instance of Entity containing the specified Component subtypes.
        /// </summary>
        /// <param name="components">The Component subtypes contained by the new entity.</param>
        /// <returns>A new Entity instance.</returns>
        public Entity NewEntity(params Component[] components)
        {
            var rval = new Entity(components);
            EntitiesById[rval.Id] = rval;
            return rval;
        }

        /// <summary>
        /// Create a new instance of a Entity containing Component subtypes as specified in the given recipe.
        /// </summary>
        /// <param name="recipe">A Recipe instance describing the Component subtypes.</param>
        /// <returns>A new Entity instance.</returns>
        public Entity NewEntity(Recipe recipe)
        {
            return NewEntity(recipe.ToNutArray());
        }

        /// <summary>
        /// Creates a new instance of Entity that is a copy of the provided prototype Entity.
        /// </summary>
        /// <param name="prototype">The Entity instance to be copied.</param>
        /// <returns>A new Entity instance.</returns>
        public Entity NewEntity(Entity prototype)
        {
            return NewEntity(prototype.GetAll().Select(p => p.Clone() as Component).ToArray());
        }

        /// <summary>
        /// Creates a new instance of Entity containing default instances of the indicated Component subtypes.
        /// </summary>
        /// <param name="compTypes">Type objects for the Component subtypes desired.</param>
        /// <returns>A new Entity instance.</returns>
        public Entity NewEntity(params Type[] compTypes)
        {
            return NewEntity(compTypes.Select(t => Activator.CreateInstance(t) as Component).ToArray());
        }

        /// <summary>
        /// Get a Entity instance corresponding to an integer id.
        /// </summary>
        /// <param name="id">A valid integer id.</param>
        /// <returns>The corresponding Entity instance.</returns>
        public Entity Get(int id)
        {
            return EntitiesById[id];
        }

        /// <summary>
        /// Overload of index operator for get only.
        /// </summary>
        /// <param name="id">Integer id of Entity instance desired.</param>
        /// <returns>The Entity instance for id.</returns>
        public Entity this[int id]
        {
            get { return Get(id); }
            private set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Conditionally retrieve a Entity instance for an integer id that may,
        /// or may not, be contained in the Group instance.
        /// </summary>
        /// <param name="id">The integer id for the desired Entity.</param>
        /// <param name="entity">An out parameter to be filled in if the Entity is found.</param>
        /// <returns>True if id is present and entity is valid.</returns>
        public bool TryGet(int id, out Entity entity)
        {
            return EntitiesById.TryGetValue(id, out entity);
        }

        /// <summary>
        /// Discard a Entity instance from this Group instance and remove all of its
        /// Component subtype instances.
        /// </summary>
        /// <param name="entity">The Entity instance to be discarded.</param>
        public void Discard(Entity entity)
        {
            var bid = entity.Id;
            EntitiesById.Remove(bid);
            entity.ClearAll();
        }

        /// <summary>
        /// Implementation of IEnumerator&lt;Entity&gt; interface.
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<Entity> GetEnumerator()
        {
            return EntitiesById.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    /// <exclude/>
    public class GroupSerializer : JsonConverter
    {
        /// <summary>
        /// For internal use only.
        /// </summary>
        /// <exclude/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vob = (Group)value;
            writer.WriteStartArray();
            foreach (var entity in vob.EntitiesById.Values)
            {
                serializer.Serialize(writer, entity);
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
            var vob = new Group();
            var bags = vob.EntitiesById;
            while (reader.Read() && (reader.TokenType != JsonToken.EndArray))
            {
                var entity = serializer.Deserialize<Entity>(reader);
                if (null == entity)
                    throw new JsonException("Unable to deserialize entity");
                bags[entity.Id] = entity;
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
            return typeof(Group).IsAssignableFrom(objectType);
        }
    }
}