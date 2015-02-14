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
            var n = (Peanuts.NumberOfTypes() >> BitSize) + 1;
            _bits = new uint[n];
        }

        internal Mix(IEnumerable<Type> nutTypes)
            :this()
        {
            foreach (var t in nutTypes)
                Set(Peanuts.GetId(t));
        }

        /// <summary>
        /// Create a new Mix described by the given Nut subtypes.
        /// </summary>
        /// <param name="nutTypes">A list of Type objects for valid Nut subtypes.</param>
        public Mix(params Type[] nutTypes)
            :this((IEnumerable<Type>) nutTypes)
        {
        }

        /// <summary>
        /// Test for the existance of a Nut subtype in the Mix instance using the corresponding integer id.
        /// </summary>
        /// <param name="index">The unique integer id of the Nut subtype to check.</param>
        /// <returns>True if the Mix instance contains the indicated Nut subtype.</returns>
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
        /// Implements the 'key fits lock' test to determine if the current Mix instance is a subset
        /// of the indicated Mix instance.
        /// </summary>
        /// <param name="lockMix">A Mix instance describing the possible superset.</param>
        /// <returns>True if this mix is a subset of the lockMix mix.</returns>
        public bool KeyFitsLock(Mix lockMix)
        {
            if (lockMix == null)
                throw new ArgumentNullException("lockMix");
            if (_bits.Length != lockMix._bits.Length)
                throw new ArgumentException("Mix size mismatch");
            var lockBits = lockMix._bits;
            return !_bits.Where((bit, i) => (bit & lockBits[i]) != bit).Any();
        }
    }
}