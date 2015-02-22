using Newtonsoft.Json;

namespace Peanuts
{
    /// <summary>
    /// IdGenerator used to generate contextually unique integer identifiers for Entity instances.
    /// Is compatible with Json.Net serialization.
    /// </summary>
    public sealed class IdGenerator
    {
    	/// <summary>
    	/// The value of the last integer ID generated.
    	/// </summary>
        [JsonProperty]
        public int LastId { get; internal set; }

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