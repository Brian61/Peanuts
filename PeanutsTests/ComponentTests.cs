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
            var a = new MockComponentA() {SomeText = "This should work!"};
            var b = a.Clone() as MockComponentA;
            Assert.NotNull(b);
            Assert.AreEqual(a.SomeText, b.SomeText);
        }
    }
}
