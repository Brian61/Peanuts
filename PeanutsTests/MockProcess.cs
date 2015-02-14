using System;
using System.Collections.Generic;

namespace Peanuts.Tests
{
    public class MockProcess : Process, IProcess
    {
        //public List<int> CurrentMatches = new List<int>();
        public List<Bag> NotifiedBags = new List<Bag>(); 

        public MockProcess(Vendor vendor, params Type[] requiredNutTypes) 
            : base(vendor, requiredNutTypes)
        {
        }

        void IProcess.OnChangeBagMix(Bag bag)
        {
            base.OnChangeBagMix(bag);
            NotifiedBags.Add(bag);
        }

        public IEnumerable<int> GetMatchingBagIds()
        {
            return MatchingBagIds();
        }

        public override void Update(long gameTick, object context = null)
        {
            //CurrentMatches.Clear();
            //CurrentMatches.AddRange(MatchingBagIds());
        }
    }
}