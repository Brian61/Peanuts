using Newtonsoft.Json;

namespace Peanuts
{
    public sealed class IdGenerator
    {
        [JsonProperty]
        private int _lastId;

        public IdGenerator(int lastId = 0)
        {
            _lastId = lastId;
        }

        public int Next()
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