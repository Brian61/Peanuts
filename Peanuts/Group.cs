using System;
using System.Collections.Generic;
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
        private readonly SortedDictionary<int, Entity> _entitiesById;
        
        [NonSerialized]
        private ISet<Action<Entity, Type, bool>> _listeners;
        
        [OnDeserializing]
        private void BeforeDeserialize(StreamingContext c)
        {
            if (null == _listeners)
                _listeners = new HashSet<Action<Entity, Type, bool>>();
        }
      
        /// <summary>
        /// Creates a new Group instance.
        /// </summary>
        public Group()
        {
            _entitiesById = new SortedDictionary<int, Entity>();
            _listeners = new HashSet<Action<Entity, Type, bool>>();
        }
        
        /// <summary>
        /// Add a listener to the list of listeners to be notified of
        /// configuration changes to entities in the group.  Listeners
        /// are also notified of entity addition and removal.
        /// </summary>
        /// <param name="listener">The callback Func for the listener.</param>
        public void AddListener(Action<Entity, Type, bool> listener)
        {
            _listeners.Add(listener);
        }
        
        /// <summary>
        /// Removes a listener from the list of listeners to be notified of
        /// configuration changes to entities in the group.
        /// </summary>
        /// <param name="listener">The callback Func for the listener.</param>
        public void RemoveListener(Action<Entity, Type, bool> listener)
        {
            _listeners.Remove(listener);
        }
        
        internal void NotifyListeners(Entity entity, Type compType, bool added)
        {
            foreach (var listener in _listeners)
                listener(entity, compType, added);
        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Add(Entity entity)
        {
            _entitiesById.Add(entity.Id, entity);
            NotifyListeners(entity, null, true);
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Remove(Entity entity)
        {
            _entitiesById.Remove(entity.Id);
            NotifyListeners(entity, null, false);
        }
        
        /// <summary>
        /// Get a Entity instance corresponding to an integer id.
        /// </summary>
        /// <param name="id">A valid integer id.</param>
        /// <returns>The corresponding Entity instance.</returns>
        public Entity Get(int id)
        {
            return _entitiesById[id];
        }

        /// <summary>
        /// Overload of index operator for get only.
        /// </summary>
        /// <param name="id">Integer id of Entity instance desired.</param>
        /// <returns>The Entity instance for id.</returns>
        public Entity this[int id] {
            get { return Get(id); }
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
            return _entitiesById.TryGetValue(id, out entity);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Entity> GetEnumerator()
        {
            return _entitiesById.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}