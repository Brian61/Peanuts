using NUnit.Framework;

namespace Peanuts.Tests
{
    [TestFixture()]
    public class NutTests
    {
        [Test()]
        public void CloneTest()
        {
            Peanuts.Initialize();
            var a = new MockNutA() {SomeText = "This should work!"};
            var b = a.Clone() as MockNutA;
            Assert.NotNull(b);
            Assert.AreEqual(a.SomeText, b.SomeText);
        }
    }
}
