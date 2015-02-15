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

        /// <summary>
        /// The set of Bag instances meeting the 'key fits lock' criteria.
        /// </summary>
        protected ISet<Bag> MatchingBags { get; private set; } 

        /// <summary>
        /// The constructor for this base class.  
        /// </summary>
        /// <param name="vendor">The Vendor instance of interest, may be null.</param>
        /// <param name="requiredNutTypes">A list of Type objects for the Nut subtypes of interest.</param>
        protected Process(Vendor vendor, params Type[] requiredNutTypes)
        {
            _key = new Mix(requiredNutTypes);
            MatchingBags = new HashSet<Bag>();
            //_bagIds = new HashSet<int>();
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
            MatchingBags.Clear();
            //_bagIds.Clear();
            if (null != vendor)
                vendor.Register(this);
        }

        /// <summary>
        /// Called internally whenever a mix changes for a Bag instance of potential interest
        /// to this Process.  Bag instances may be added or removed from the tracked list.
        /// </summary>
        /// <param name="bag">The Bag instance that has changed.</param>
        public void OnChangeBagMix(Bag bag)
        {
            if (_key.KeyFitsLock((bag.Mask)))
                MatchingBags.Add(bag);
            else
                MatchingBags.Remove(bag);
        }
    }
}
