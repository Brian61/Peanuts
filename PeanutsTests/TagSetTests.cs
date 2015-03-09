using NUnit.Framework;

namespace Peanuts.Tests
{
    [TestFixture()]
    public class TagSetTests
    {
        [Test()]
        public void TagSetTest()
        {
            Peanuts.Initialize();
            var a = new TagSet(typeof (MockComponentA));
            Assert.IsNotNull(a);
        }

        [Test()]
        public void KeyFitsLockTest()
        {
            Peanuts.Initialize();
            var a = new TagSet(typeof (MockComponentA));
            var b = new TagSet(typeof(MockComponentA), typeof(MockComponentB));
            Assert.IsTrue(a.KeyFitsLock(b));
            Assert.IsFalse(b.KeyFitsLock(a));
        }
    }
}
