using System;
using System.Collections.Generic;
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
	   
		internal TagSet LockTag { get; private set; }
		
		/// <summary>
		/// The unique integer id of the Entity instance.
		/// </summary>
		public int Id {
			get;
			private set;
		}

		#region ISerializable implementation
		Entity(SerializationInfo info, StreamingContext context)
		{
			_compsByType = new Dictionary<Type, Component>();
			Id = info.GetInt32("Id");
			var enumerator = info.GetEnumerator();
			while(enumerator.MoveNext()) {
				var name = enumerator.Current.Name;
				if (name == "Id") continue;
				var ptype = Peanuts.GetType(name);
				var comp = enumerator.Value as Component;
				_compsByType[ptype] = comp;
			}
			LockTag = new TagSet(_compsByType.Keys);
		}
		
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Id", Id, typeof(Int32));
			foreach(var kv in _compsByType) {
				info.AddValue(kv.Key.Name, kv.Value, kv.Key);
			}
		}
		#endregion
		
		internal Dictionary<Type, Component> GetDictionary()
		{
			return _compsByType;
		}

		internal Entity(int id, Dictionary<Type, Component> contents)
		{
			Id = id;
			Peanuts.EntityIdGenerator.EnsureGreaterThan(id);
			_compsByType = contents;
			LockTag = new TagSet(contents.Keys);
		}
		
		/// <summary>
		/// Empty entity intended for return value when no entity is available and
		/// null return is undesireable.
		/// </summary>
		public static readonly Entity Empty = new Entity(0, new Dictionary<Type, Component>());

		internal Entity(IEnumerable<Component> components)
		{
			_compsByType = new Dictionary<Type, Component>();
			foreach (var p in components)
			{
				_compsByType[p.GetType()] = p;
			}
			LockTag = new TagSet(_compsByType.Keys);
			Id = Peanuts.EntityIdGenerator.Next();
		}
 
		internal IEnumerable<Component> GetAll()
		{
			return _compsByType.Values;
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
			var type = typeof (T);
			if (_compsByType.ContainsKey(type))
			{
				component = _compsByType[type] as T;
				return true;
			}
			component = null;
			return false;
		}

		/// <summary>
		/// This is the 'key-fits-lock' test that checks to see if the indicated TagSet describes a subset of the contents.
		/// </summary>
		/// <param name="key">A TagSet object describing the Component subtypes of interest.</param>
		/// <returns>True if all of the subtypes indicated by key exist in this Entity.</returns>
		public bool Contains(TagSet key)
		{
			return key.KeyFitsLock(LockTag);
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
			foreach(var compType in compTypes)
			{
				if (!_compsByType.ContainsKey(compType))
					return false;
			}
			return true;
		}
		
		internal void Add(Component component)
		{
			var compType = component.GetType();
			var cid = Peanuts.GetId(compType);
			_compsByType[compType] = component;
			LockTag.Set(cid);
		}

		internal void Remove(Component component)
		{
			var compType = component.GetType();
			var cid = Peanuts.GetId(compType);
			_compsByType.Remove(compType);
			LockTag.Clear(cid);
		}

		internal void Morph(Entity prototype)
		{
			var proto = prototype.LockTag;
			for (var i = 0; i < Peanuts.NumberOfTypes(); i++)
			{
				if (LockTag.IsSet(i))
				{
					if (proto.IsSet(i))
						continue;
					_compsByType.Remove(Peanuts.GetType(i));
					LockTag.Clear(i);
				}
				else
				{
					if (!proto.IsSet(i))
						continue;
					var compType = Peanuts.GetType(i);
					_compsByType[compType] = prototype._compsByType[compType].Clone() as Component;
					LockTag.Set(i);
				}
			}
		}

		internal void ClearAll()
		{
			_compsByType.Clear();
			LockTag.ClearAll();
		}
	}
}