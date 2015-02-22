using NUnit.Framework;

namespace Peanuts.Tests
{
    [TestFixture()]
    public class ComponentTests
    {
        [Test()]
        public void CloneTest()
        {
            Peanuts.Initialize();
            var a = new MockEntityA() {SomeText = "This should work!"};
            var b = a.Clone() as MockEntityA;
            Assert.NotNull(b);
            Assert.AreEqual(a.SomeText, b.SomeText);
        }
    }
}
