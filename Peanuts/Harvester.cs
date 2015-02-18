using System;
using System.Collections.Generic;

namespace Peanuts
{
    /// <summary>
    /// A class that provides tracking of current Bag instances of interest for the current Vendor instance.  
    /// </summary>
    public class Harvester : IEnumerable<Bag>
    {
        private readonly SortedDictionary<int, Bag> _bags;

        internal void Add(Bag bag)
        {
            _bags.Add(bag.Id, bag);
        }

        internal bool Remove(Bag bag)
        {
            return _bags.Remove(bag.Id);
        }

        internal Harvester()
        {
            _bags = new SortedDictionary<int, Bag>();
        }

        /// <summary>
        /// Implementation of IEnumerator&lt;Bag&gt; interface.
        /// </summary>
        /// <returns>An Enumerator.</returns>
        public IEnumerator<Bag> GetEnumerator()
        {
            return _bags.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
