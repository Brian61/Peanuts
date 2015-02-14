using NUnit.Framework;

namespace Peanuts.Tests
{
    [TestFixture()]
    public class MixTests
    {
        [Test()]
        public void MixTest()
        {
            Peanuts.Initialize();
            var a = new Mix(typeof (MockNutA));
            Assert.IsNotNull(a);
        }

        [Test()]
        public void KeyFitsLockTest()
        {
            Peanuts.Initialize();
            var a = new Mix(typeof (MockNutA));
            var b = new Mix(typeof(MockNutA), typeof(MockNutB));
            Assert.IsTrue(a.KeyFitsLock(b));
            Assert.IsFalse(b.KeyFitsLock(a));
        }
    }
}
