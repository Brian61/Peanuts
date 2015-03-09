using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Peanuts
{
    /// <summary>
    /// A Entity holds a collection of Components.
    /// </summary>
    [Serializable]
    public sealed class Entity : ISerializable
    {
        private readonly Dictionary<Type, Component> _compsByType;
        private Group _group;

        /// <summary>
        /// The unique integer id of the Entity instance.
        /// </summary>
        public int Id { get; private set; }

        #region ISerializable implementation
        private Entity(SerializationInfo info, StreamingContext context)
            : this(0)
        {
            Id = info.GetInt32("Id");
            Peanuts.EntityIdGenerator.EnsureGreaterThan(Id);
            var enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var name = enumerator.Current.Name;
                if (name == "Id") continue;
                var ptype = Peanuts.GetType(name);
                var comp = enumerator.Value as Component;
                _compsByType[ptype] = comp;
            }
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id, typeof(Int32));
            foreach (var kv in _compsByType)
            {
                info.AddValue(kv.Key.Name, kv.Value, kv.Key);
            }
        }
        #endregion

        private Entity(int id)
        {
            Id = id;
            _group = null;
            _compsByType = new Dictionary<Type, Component>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        public Entity()
            : this(Peanuts.EntityIdGenerator.Next())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="components">The components.</param>
        public Entity(IEnumerable<Component> components)
            : this()
        {
            Id = Peanuts.EntityIdGenerator.Next();
            foreach (var p in components) Add(p);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public Entity(Entity source)
            : this(source._compsByType.Values.Select(c => c.Clone() as Component))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="components">The components.</param>
        public Entity(params Component[] components)
            : this(components as IEnumerable<Component>)
        {
        }

        internal void SetGroup(Group group)
        {
            _group = group;
        }

        /// <summary>
        /// Gets the Component subtype T from the current Entity.
        /// </summary>
        /// <returns>A T (Component subtype) instance.</returns>
        public T Get<T>() where T : Component
        {
            return _compsByType[typeof(T)] as T;
        }

        /// <summary>
        /// Checks for existance of an identified Component type and if found gives the corresponding instance.
        /// </summary>
        /// <param name="component">An out parameter to recieve the Component derived instance if it exists.</param>
        /// <returns>True if an instance of the indicated Component subtype exists and the out param is valid.</returns>
        public bool TryGet<T>(out T component) where T : Component
        {
            var type = typeof(T);
            if (_compsByType.ContainsKey(type))
            {
                component = _compsByType[type] as T;
                return true;
            }
            component = null;
            return false;
        }

        /// <summary>
        /// Test for existance of a given Component subtype in this entity.
        /// </summary>
        /// <param name="compType">The Component subtype to check.</param>
        /// <returns>True if contained.</returns>
        public bool Contains(Type compType)
        {
            return _compsByType.ContainsKey(compType);
        }

        /// <summary>
        /// Test for existance of multiple Component subtypes in this entity.
        /// </summary>
        /// <param name="compTypes">The Component subtypes to check.</param>
        /// <returns>True if all indicated subtypes are contained.</returns>
        public bool Contains(params Type[] compTypes)
        {
            return Contains(compTypes as IEnumerable<Type>);
        }

        /// <summary>
        /// Determines whether this Entity contains all of the given types.
        /// </summary>
        /// <param name="compTypes">The comp types.</param>
        /// <returns>True if all types contained.</returns>
        public bool Contains(IEnumerable<Type> compTypes)
        {
            return compTypes.All(_compsByType.ContainsKey);
        }

        private void NotifyChangeComponentSet(Type ctype, bool added)
        {
            _group.NotifyListeners(this, ctype, added);
        }

        /// <summary>
        /// Adds the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public void Add(Component component)
        {
            var compType = component.GetType();
            var cid = Peanuts.GetId(compType);
            _compsByType[compType] = component;
            if (null != _group)
                NotifyChangeComponentSet(compType, true);
        }

        /// <summary>
        /// Removes the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public void Remove(Component component)
        {
            var compType = component.GetType();
            var cid = Peanuts.GetId(compType);
            _compsByType.Remove(compType);
            if (null != _group)
                NotifyChangeComponentSet(compType, false);
        }

        /// <summary>
        /// Morphes the specified prototype.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        public void Morph(Entity prototype)
        {
            var tbr = _compsByType.Keys.Except(prototype._compsByType.Keys).ToList();
            var tba = prototype._compsByType.Keys.Except(_compsByType.Keys).ToList();
            foreach (var ct in tbr)
                Remove(_compsByType[ct]);
            foreach (var ct in tba)
                Add((Component)prototype._compsByType[ct].Clone());
        }
    }
}