using System.Linq;
using NUnit.Framework;

namespace Peanuts.Tests
{
    [TestFixture()]
    public class HarvesterTests
    {
        [Test()]
        public void ChangeVendorTest()
        {
            Peanuts.Initialize();
            var proc = new Harvester(null, typeof(MockNutA));
            var vendorA = new Vendor();
            Assert.DoesNotThrow(() => proc.ChangeVendor(vendorA));
            vendorA.MakeBag(typeof (MockNutA));
            Assert.AreEqual(1, proc.MatchingBags.Count);
            var vendorB = new Vendor();
            proc.ChangeVendor(vendorB);
            vendorB.MakeBag(typeof(MockNutA));
            Assert.AreEqual(1, proc.MatchingBags.Count);
            vendorA.MakeBag(typeof (MockNutA));
            Assert.AreEqual(1, proc.MatchingBags.Count);
        }

        [Test()]
        public void OnChangeBagMixTest()
        {
            Peanuts.Initialize();
            var vendor = new Vendor();
            var proc = new Harvester(vendor, typeof(MockNutA));
            var bag = vendor.MakeBag(typeof (MockNutA));
            Assert.IsTrue(proc.MatchingBags.Contains(bag));
            var nut = bag.Get<MockNutA>();
            vendor.Remove(bag, nut);
            Assert.IsFalse(proc.MatchingBags.Contains(bag));
        }

    }
}
