using Newtonsoft.Json;

namespace Peanuts
{
    /// <summary>
    /// IdGenerator used to generate contextually unique integer identifiers for Bag instances.
    /// Is compatible with Json.Net serialization.
    /// </summary>
    public sealed class IdGenerator
    {
        [JsonProperty]
        private int _lastId;

        /// <summary>
        /// Construct an instance of IdGenerator.  Intended for advanced usage.
        /// </summary>
        /// <param name="lastId">Optional integer used to set the bottom of the id range.</param>
        public IdGenerator(int lastId = 0)
        {
            _lastId = lastId;
        }

        internal int Next()
        {
            return ++_lastId;
        }

        internal void EnsureGreaterThan(int id)
        {
            if (id > _lastId)
                _lastId = id;
        }
    }
}