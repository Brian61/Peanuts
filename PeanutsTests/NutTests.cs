using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peanuts;
using NUnit.Framework;
namespace Peanuts.Tests
{
    [TestFixture()]
    public class NutTests
    {
        [Test()]
        public void InitializeTest()
        {
            Assert.DoesNotThrow(() => Nut.Initialize());
            Assert.DoesNotThrow(() => Nut.Initialize(true));
        }

        [Test()]
        public void GetIdTest()
        {
            Nut.Initialize();
            Assert.AreEqual( Nut.GetId(typeof(MockNutA)), Nut.GetId("MockNutA"));
            Assert.AreNotEqual(Nut.GetId(typeof(MockNutA)), Nut.GetId("MockNutB"));
            Assert.AreEqual(Nut.GetId(typeof(MockNutB)), Nut.GetId("MockNutB"));
        }

        [Test()]
        public void CloneTest()
        {
            Nut.Initialize();
            var a = new MockNutA() {SomeText = "This should work!"};
            var b = a.Clone() as MockNutA;
            Assert.NotNull(b);
            Assert.AreEqual(a.SomeText, b.SomeText);
        }

        [Test]
        public void CloneTest1()
        {
            Nut.Initialize();
            var a = new MockNutB() { SomeFloat = 2.0f };
            var b = a.Clone() as MockNutB;
            Assert.NotNull(b);
            Assert.AreEqual(a.SomeFloat, b.SomeFloat);
        }
    }
}
