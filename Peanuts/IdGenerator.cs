using System;

namespace Peanuts
{
    /// <summary>
    /// IdGenerator used to generate contextually unique integer identifiers for Entity instances.
    /// Is compatible with Json.Net serialization.
    /// </summary>
    [Serializable]
    public sealed class IdGenerator
    {
        private int _lastId;

        /// <summary>
        /// The value of the last integer ID generated.
        /// </summary>
        public int LastId { get { return _lastId; } internal set { _lastId = value; } }

        /// <summary>
        /// Construct an instance of IdGenerator.  Intended for advanced usage.
        /// </summary>
        /// <param name="lastId">Optional integer used to set the bottom of the id range.</param>
        public IdGenerator(int lastId = 0)
        {
            LastId = lastId;
        }

        internal int Next()
        {
            return ++LastId;
        }

        internal void EnsureGreaterThan(int id)
        {
            if (id > LastId)
                LastId = id;
        }
    }
}