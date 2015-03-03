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
    /// EntityChangeEvent type enumeration for listeners. 
    /// </summary>
	public enum EntityChangeEvent {
    	/// <summary>
    	/// Indicates an entity has been added to the group.
    	/// </summary>
		Added = 1,
		
		/// <summary>
		/// Indicates an entity has been removed from the group.
		/// </summary>
		Removed = 2,
		
		/// <summary>
		/// Indicates the entity's set of Component types has changed.
		/// </summary>
		Reconfigured = 3
	}

    /// <summary>
    /// Instances of the Group class form the interface for access to a 
    /// collection of Entity instances.
    /// </summary>
    [JsonConverter(typeof(GroupSerializer))]
    public sealed class Group : IEnumerable<Entity>
    {
        internal readonly SortedDictionary<int, Entity> EntitiesById;
        private readonly ISet<Action<Entity, EntityChangeEvent>> _listeners;

        /// <summary>
        /// Creates a new Group instance.
        /// </summary>
        public Group()
        {
            EntitiesById = new SortedDictionary<int, Entity>();
            _listeners = new HashSet<Action<Entity, EntityChangeEvent>>();
        }
        
        /// <summary>
        /// Add a listener to the list of listeners to be notified of
        /// configuration changes to entities in the group.  Listeners
        /// are also notified of entity addition and removal.
        /// </summary>
        /// <param name="listener">The callback Func for the listener.</param>
        public void AddListener(Action<Entity, EntityChangeEvent> listener)
        {
        	_listeners.Add(listener);
        }
        
        /// <summary>
        /// Removes a listener from the list of listeners to be notified of
        /// configuration changes to entities in the group.
        /// </summary>
        /// <param name="listener">The callback Func for the listener.</param>
        public void RemoveListener(Action<Entity, EntityChangeEvent> listener)
        {
        	_listeners.Remove(listener);
        }
        
        private void NotifyListeners(Entity entity, EntityChangeEvent change) 
        {
        	foreach(var listener in _listeners)
        		listener(entity, change);
        }
        
        internal void AddEntity(Entity entity)
        {
        	EntitiesById[entity.Id] = entity;
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
            NotifyListeners(rval, EntityChangeEvent.Added);
            return rval;
        }

        /// <summary>
        /// Create a new instance of a Entity containing Component subtypes as specified in the given recipe.
        /// </summary>
        /// <param name="recipe">A Recipe instance describing the Component subtypes.</param>
        /// <returns>A new Entity instance.</returns>
        public Entity NewEntity(Recipe recipe)
        {
            return NewEntity(recipe.ToComponentArray());
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
        /// Creates a new instance of Entity containing default instances of the indicated Component subtypes.
        /// </summary>
        /// <param name="tagSet">A TagSet instance defining the set of subtypes.</param>
        /// <returns></returns>
        public Entity NewEntity(TagSet tagSet)
        {
        	return NewEntity(Enumerable.Range(0, Peanuts.NumberOfTypes())
        	                 .Where(tagSet.IsSet)
        	                 .Select(Peanuts.GetType)
        	                 .ToArray());
        	                 
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
            NotifyListeners(entity, EntityChangeEvent.Removed);
			entity.ClearAll();
        }
        
        /// <summary>
        /// Adds a Component subtype to a Entity instance.
        /// </summary>
        /// <param name="entity">The entity to be modified.</param>
        /// <param name="component">The Component subtype instance to be added.</param>
       public void AddComponent(Entity entity, Component component)
        {
        	entity.Add(component);
        	NotifyListeners(entity, EntityChangeEvent.Reconfigured);
        }
        
        /// <summary>
        /// Removes a Component subtype from a Entity instance.
        /// </summary>
        /// <param name="entity">The entity to be modified.</param>
        /// <param name="component">The Component subtype instance to be removed.</param>
        public void RemoveComponent(Entity entity, Component component)
        {
        	entity.Remove(component);
        	NotifyListeners(entity, EntityChangeEvent.Reconfigured);
        }
        
        /// <summary>
        /// Morph (change) the target Entity instance to have the same Component subtypes as the 
        /// prototype Entity instance.  Component subtype instances contained in both will not be
        /// modified.  Component subtypes not included in the prototype will be removed from
        /// the target.  Component subtypes present in the prototype but not in the target
        /// will be copied from the prototype to the target.
        /// </summary>
        /// <param name="entity">The entity to be modified.</param>
        /// <param name="prototype">A Entity instance serving as a template.</param>
        public void MorphEntity(Entity entity, Entity prototype)
        {
        	entity.Morph(prototype);
        	NotifyListeners(entity, EntityChangeEvent.Reconfigured);
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