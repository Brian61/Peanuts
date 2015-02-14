using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Peanuts.Tests
{
    public class MockProcess : Process, IProcess
    {
        public List<int> NotifiedBags = new List<int>(); 

        public MockProcess(Vendor vendor, params Type[] requiredNutTypes) 
            : base(vendor, requiredNutTypes)
        {
        }

        void IProcess.OnChangeBagMix(int id, Mix lockMix)
        {
            base.OnChangeBagMix(id, lockMix);
            NotifiedBags.Add(id);
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