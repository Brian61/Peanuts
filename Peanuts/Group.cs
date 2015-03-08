using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Peanuts
{
    /// <summary>
    /// Instances of the Group class form the interface for access to a 
    /// collection of Entity instances.
    /// </summary>
    [Serializable]
    public sealed class Group : IEnumerable<Entity>
    {
        internal readonly SortedDictionary<int, Entity> EntitiesById;
        
        [NonSerialized]
        private ISet<Action<Entity, TagSet, TagSet>> _listeners;
        
        [OnDeserializing]
        private void BeforeDeserialize(StreamingContext c)
        {
            if (null == _listeners)
                _listeners = new HashSet<Action<Entity, TagSet, TagSet>>();
        }

        [OnDeserialized]
        private void AfterDeserialize(StreamingContext c)
        {
            Peanuts.EntityIdGenerator.EnsureGreaterThan(EntitiesById.Keys.Max());
        }
        
        /// <summary>
        /// Creates a new Group instance.
        /// </summary>
        public Group()
        {
            EntitiesById = new SortedDictionary<int, Entity>();
            _listeners = new HashSet<Action<Entity, TagSet, TagSet>>();
        }
        
        /// <summary>
        /// Add a listener to the list of listeners to be notified of
        /// configuration changes to entities in the group.  Listeners
        /// are also notified of entity addition and removal.
        /// </summary>
        /// <param name="listener">The callback Func for the listener.</param>
        public void AddListener(Action<Entity, TagSet, TagSet> listener)
        {
            _listeners.Add(listener);
        }
        
        /// <summary>
        /// Removes a listener from the list of listeners to be notified of
        /// configuration changes to entities in the group.
        /// </summary>
        /// <param name="listener">The callback Func for the listener.</param>
        public void RemoveListener(Action<Entity, TagSet, TagSet> listener)
        {
            _listeners.Remove(listener);
        }
        
        private void NotifyListeners(Entity entity, TagSet current, TagSet previous)
        {
            foreach (var listener in _listeners)
                listener(entity, current, previous);
        }
        
        /// <summary>
        /// Create a new instance of Entity containing the specified Component subtypes.
        /// </summary>
        /// <param name="components">The Component subtypes contained by the new entity.</param>
        /// <returns>A new Entity instance.</returns>
        public Entity NewEntity(params Component[] components)
        {
            var entity = new Entity(components);
            EntitiesById[entity.Id] = entity;
            NotifyListeners(entity, entity.LockTag, null);
            return entity;
        }

        private Entity NewEntityClone(IEnumerable<Component> source)
        {
            return NewEntity(source.Select(c => c.Clone() as Component).ToArray());
        }

        /// <summary>
        /// Create a new instance of a Entity containing Component subtypes as specified in the given recipe.
        /// </summary>
        /// <param name="book">The RecipeBook containing the recipe.</param>
        /// <param name="recipeName">The name of the recipe.</param>
        /// <returns>A new Entity instance.</returns>
        public Entity NewEntity(RecipeBook book, string recipeName)
        {
            return NewEntityClone(book.GetComponentsFor(recipeName));
        }

        /// <summary>
        /// Creates a new instance of Entity that is a copy of the provided prototype Entity.
        /// </summary>
        /// <param name="prototype">The Entity instance to be copied.</param>
        /// <returns>A new Entity instance.</returns>
        public Entity NewEntity(Entity prototype)
        {
            return NewEntityClone(prototype.GetAll());
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
        /// <returns>A new Entity containing components indicated by the TagSet.</returns>
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
        public Entity this[int id] {
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
            NotifyListeners(entity, null, entity.LockTag);
            entity.ClearAll();        
        }
        
        /// <summary>
        /// Adds a Component subtype to a Entity instance.
        /// </summary>
        /// <param name="entity">The entity to be modified.</param>
        /// <param name="component">The Component subtype instance to be added.</param>
        public void AddComponent(Entity entity, Component component)
        {
            var previous = (TagSet)entity.LockTag.Clone();
            entity.Add(component);
            NotifyListeners(entity, entity.LockTag, previous);
        }
        
        /// <summary>
        /// Removes a Component subtype from a Entity instance.
        /// </summary>
        /// <param name="entity">The entity to be modified.</param>
        /// <param name="component">The Component subtype instance to be removed.</param>
        public void RemoveComponent(Entity entity, Component component)
        {
            var previous = (TagSet)entity.LockTag.Clone();
            entity.Remove(component);
            NotifyListeners(entity, entity.LockTag, previous);
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
            var previous = (TagSet)entity.LockTag.Clone();
            entity.Morph(prototype);
            NotifyListeners(entity, entity.LockTag, previous);
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
}