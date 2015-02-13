using System;
using System.Collections.Generic;
using System.Linq;

namespace Peanuts
{
    /// <summary>
    /// Instances of this class describe a specific mixture of unique Nut subtypes as a custom bitarray.
    /// </summary>
    public sealed class Mix
    {
        const int BitSize = (sizeof(uint) * 8) - 1;
        const int ByteSize = 5;  // log_2(BitSize + 1)

        uint[] _bits;

        internal Mix()
        {
            var n = (Nut.NumberOfTypes() >> BitSize) + 1;
            _bits = new uint[n];
        }

        /// <summary>
        /// Create a new Mix described by the given Nut subtypes.
        /// </summary>
        /// <param name="nutTypes">A list of Type objects for valid Nut subtypes.</param>
        public Mix(params Type[] nutTypes)
            :this()
        {
            foreach (var t in nutTypes)
                Set(Nut.GetId(t));
        }

        /// <summary>
        /// Creates a new Mix described by the given Nut subtypes.
        /// </summary>
        /// <param name="nutTypeNames">A list of string names for valid Nut subtypes.</param>
        public Mix(params string[] nutTypeNames)
            :this()
        {
            foreach (var s in nutTypeNames)
                Set(Nut.GetId(s));
        }

        //internal Mix(params Nut[] nuts)
        //    :this()
        //{
        //    foreach (var p in nuts)
        //        Set(Nut.GetId(p.GetType()));
        //}

        internal Mix(Dictionary<int, Nut>.KeyCollection nutIds)
            :this()
        {
            foreach(var i in nutIds)
                Set(i);
        }

        /// <summary>
        /// Test for the existance of a Nut subtype in the Mix instance using the corresponding integer id.
        /// </summary>
        /// <param name="index">The unique integer id of the Nut subtype to check.</param>
        /// <returns>True if the Mix instance contains the indicated Nut subtype.</returns>
        public bool IsSet(int index)
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

        //public void SetAll()
        //{
        //    var count = _bits.Length;
        //    for (var i = 0; i < count; i++)
        //        _bits[i] = 0xffffffff;
        //}

        internal void ClearAll()
        {
            Array.Clear(_bits, 0, _bits.Length);
        }

        /// <summary>
        /// Implements the 'key fits lock' test to determine if the current Mix instance is a subset
        /// of the indicated Mix instance.
        /// </summary>
        /// <param name="other">A Mix instance describing the possible superset.</param>
        /// <returns>True if this mix is a subset of the other mix.</returns>
        public bool IsSubsetOf(Mix other)
        {
            if (other == null)
                throw new ArgumentNullException("other");
            if (_bits.Length != other._bits.Length)
                throw new ArgumentException("Mix size mismatch");
            var otherBits = other._bits;
            return !_bits.Where((bit, i) => (bit & otherBits[i]) != bit).Any();
        }
    }
}