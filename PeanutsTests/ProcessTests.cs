using System.Linq;
using NUnit.Framework;

namespace Peanuts.Tests
{
    [TestFixture()]
    public class ProcessTests
    {
        [Test()]
        public void ChangeVendorTest()
        {
            Peanuts.Initialize();
            var proc = new MockProcess(null, typeof(MockNutA));
            var vendorA = new Vendor();
            Assert.DoesNotThrow(() => proc.ChangeVendor(vendorA));
            vendorA.MakeBag(typeof (MockNutA));
            Assert.AreEqual(1, proc.NotifiedBags.Count);
            var vendorB = new Vendor();
            vendorB.MakeBag(typeof (MockNutA));
            proc.ChangeVendor(vendorB);
            Assert.AreEqual(2, proc.NotifiedBags.Count);
            vendorA.MakeBag(typeof (MockNutA));
            Assert.AreEqual(2, proc.NotifiedBags.Count);
        }

        [Test()]
        public void OnChangeBagMixTest()
        {
            Peanuts.Initialize();
            var vendor = new Vendor();
            var proc = new MockProcess(vendor, typeof(MockNutA));
            var bag = vendor.MakeBag(typeof (MockNutA));
            Assert.IsTrue(proc.GetMatchingBags().Contains(bag));
            //Assert.IsTrue(proc.GetMatchingBagIds().Contains(bag.Id));
            var nut = bag.Get<MockNutA>();
            vendor.Remove(bag, nut);
            Assert.IsFalse(proc.GetMatchingBags().Contains(bag));
            //Assert.IsFalse(proc.GetMatchingBagIds().Contains(bag.Id));
        }

    }
}
