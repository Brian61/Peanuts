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
            var a = new TagSet(typeof (MockEntityA));
            Assert.IsNotNull(a);
        }

        [Test()]
        public void KeyFitsLockTest()
        {
            Peanuts.Initialize();
            var a = new TagSet(typeof (MockEntityA));
            var b = new TagSet(typeof(MockEntityA), typeof(MockEntityB));
            Assert.IsTrue(a.KeyFitsLock(b));
            Assert.IsFalse(b.KeyFitsLock(a));
        }
    }
}
