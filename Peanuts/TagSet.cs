using System;
using System.Collections.Generic;
using System.Linq;

namespace Peanuts
{
    /// <summary>
    /// Instances of this class describe a specific mixture of unique Component 
    /// subtypes as a custom bitarray.
    /// </summary>
    public sealed class TagSet : IComparable<TagSet>, ICloneable
    {
        const int BitSize = (sizeof(uint) * 8) - 1;
        const int ByteSize = 5;  // log_2(BitSize + 1)

        uint[] _bits;

        internal TagSet()
        {
            var n = (Peanuts.NumberOfTypes() >> BitSize) + 1;
            _bits = new uint[n];
        }

        internal TagSet(IEnumerable<Type> compTypes)
            :this()
        {
            foreach (var t in compTypes)
                Set(Peanuts.GetId(t));
        }

        /// <summary>
        /// Create a new TagSet described by the given Component subtypes.
        /// </summary>
        /// <param name="compTypes">A list of Type objects for valid Component subtypes.</param>
        public TagSet(params Type[] compTypes)
            :this((IEnumerable<Type>) compTypes)
        {
        }
        
        internal TagSet(TagSet other)
        {
        	_bits = (uint[]) other._bits.Clone();
        }

        internal bool IsSet(int index)
        {
            var b = index >> ByteSize;
            if (b >= _bits.Length)
                return false;

            return (_bits[b] & (1 << (index & BitSize))) != 0;
        }

        internal void Set(int index)
        {
            var b = index >> ByteSize;
            if (b >= _bits.Length)
                Array.Resize(ref _bits, b + 1);

            _bits[b] |= 1u << (index & BitSize);
        }

        internal void Clear(int index)
        {
            var b = index >> ByteSize;
            if (b >= _bits.Length)
                return;

            _bits[b] &= ~(1u << (index & BitSize));
        }

        internal void ClearAll()
        {
            Array.Clear(_bits, 0, _bits.Length);
        }

        /// <summary>
        /// Implements the 'key fits lock' test to determine if the current TagSet instance is a subset
        /// of the indicated TagSet instance.
        /// </summary>
        /// <param name="lockMix">A TagSet instance describing the possible superset.</param>
        /// <returns>True if this mix is a subset of the lockMix mix.</returns>
        public bool KeyFitsLock(TagSet lockMix)
        {
            if (lockMix == null)
                throw new ArgumentNullException("lockMix");
            if (_bits.Length != lockMix._bits.Length)
                throw new ArgumentException("TagSet size mismatch");
            var lockBits = lockMix._bits;
            return !_bits.Where((bit, i) => (bit & lockBits[i]) != bit).Any();
        }

        /// <summary>
        /// Implements IComparable interface for type TagSet.
        /// </summary>
        /// <param name="other">Another TagSet instance to be compared against.</param>
        /// <returns>An integer representing sort order.</returns>
        public int CompareTo(TagSet other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            if (_bits.Length != other._bits.Length)
                return _bits.Length - other._bits.Length;
            for (var i = 0; i < _bits.Length; i++ )
            {
                var r = _bits[i].CompareTo(other._bits[i]);
                if (r != 0)
                    return r;
            }
            return 0;
        }

        /// <summary>
        /// Implements Clone as deep copy.
        /// </summary>
        /// <returns>A deep copy of this object.</returns>
		public object Clone()
		{
			return new TagSet(this);
		}
    }
}