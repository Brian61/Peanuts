using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Peanuts.Tests
{
    public class MockProcess : Process, IProcess
    {
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

        public IEnumerable<Bag> GetMatchingBags()
        {
            return MatchingBags;
        }
    }
}