using System;
using System.Collections.Generic;

namespace Peanuts
{
    /// <summary>
    /// An abstract base class fulfilling the IProcess contract and providing tracking of
    /// current Bag instances of interest for the current Vendor instance.  
    /// </summary>
    public abstract class Process : IProcess
    {
        /// <summary>
        /// The current Vendor instance, can be null.
        /// </summary>
        protected Vendor BagVendor { get; private set; }

        private readonly Mix _key;
        private readonly ISet<int> _bagIds;

        /// <summary>
        /// This method gives access to the unique identifiers for all Bag instances in
        /// the current Vendor instance which match the mix of interest.
        /// </summary>
        /// <returns>An IEnumerable<int> instance holding all matching Bag identifiers.</int></returns>
        protected IEnumerable<int> MatchingBagIds()
        {
            return _bagIds;
        }

        /// <summary>
        /// The constructor for this base class.  
        /// </summary>
        /// <param name="vendor">The Vendor instance of interest, may be null.</param>
        /// <param name="requiredNutTypes">A list of Type objects for the Nut subtypes of interest.</param>
        protected Process(Vendor vendor, params Type[] requiredNutTypes)
        {
            _key = new Mix(requiredNutTypes);
            _bagIds = new HashSet<int>();
            ChangeVendor(vendor);
        }

        /// <summary>
        /// This method changes the Vendor instance of interest.  It will invalidate the current list of identifiers.
        /// </summary>
        /// <param name="vendor">The new Vendor instance of interest, may be null.</param>
        public void ChangeVendor(Vendor vendor)
        {
            if (null != BagVendor)
                BagVendor.Unregister(this);
            BagVendor = vendor;
            _bagIds.Clear();
            if (null != vendor)
                vendor.Register(this);
        }

        /// <summary>
        /// Called internally whenever a mix changes for a Bag instance of potential interest
        /// to this Process.  Bag instances may be added or removed from the tracked list.
        /// </summary>
        /// <param name="id">The contextually unique integer identifier for the Bag instance.</param>
        /// <param name="lockMix">The Mix instance describing the set of Nut subtypes in the Bag.</param>
        public void OnChangeBagMix(int id, Mix lockMix)
        {
            if (_key.KeyFitsLock(lockMix))
                _bagIds.Add(id);
            else
                _bagIds.Remove(id);
        }

        /// <summary>
        /// This method must be provided by Process subtypes.  It is intended to be
        /// called whenever the Process subtype instance is required to process the Bag
        /// instances of interest.
        /// </summary>
        /// <param name="gameTick">A long integer indicating some passage of time in game.</param>
        /// <param name="context">An optional user supplied/defined object.</param>
        public abstract void Update(long gameTick, object context = null);
    }
}