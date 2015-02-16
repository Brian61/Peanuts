using System;
using System.Collections.Generic;

namespace Peanuts
{
    /// <summary>
    /// A class that provides tracking of current Bag instances of interest for the current Vendor instance.  
    /// </summary>
    public class Harvester
    {
        /// <summary>
        /// The current Vendor instance, can be null.
        /// </summary>
        public Vendor BagVendor { get; private set; }

        private readonly Mix _key;

        /// <summary>
        /// The set of Bag instances meeting the 'key fits lock' criteria.
        /// </summary>
        public ISet<Bag> MatchingBags { get; private set; } 

        /// <summary>
        /// Construct a new Process.  
        /// </summary>
        /// <param name="vendor">The Vendor instance of interest, may be null.</param>
        /// <param name="requiredNutTypes">A list of Type objects for the Nut subtypes of interest.</param>
        public Harvester(Vendor vendor, params Type[] requiredNutTypes)
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
            if (null != vendor)
                vendor.Register(this);
        }

        internal void OnChangeBagMix(Bag bag)
        {
            if (_key.KeyFitsLock((bag.Mask)))
                MatchingBags.Add(bag);
            else
                MatchingBags.Remove(bag);
        }
    }
}
