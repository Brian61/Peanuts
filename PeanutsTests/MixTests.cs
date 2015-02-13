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
    public class MixTests
    {
        [Test()]
        public void MixTest()
        {
            Nut.Initialize();
            var a = new Mix(typeof (MockNutA));
            Assert.IsNotNull(a);
        }

        [Test()]
        public void MixTest1()
        {
            Nut.Initialize();
            Assert.IsNotNull(new Mix("MockNutA"));
        }

        [Test()]
        public void IsSetTest()
        {
            Nut.Initialize();
            var id = Nut.GetId(typeof (MockNutA));
            var a = new Mix("MockNutA");
            Assert.IsTrue(a.IsSet(id));
            var b = new Mix(typeof (MockNutB));
            Assert.IsFalse(b.IsSet(id));
        }

        [Test()]
        public void IsSubsetOfTest()
        {
            Nut.Initialize();
            var a = new Mix(typeof (MockNutA));
            var b = new Mix("MockNutA", "MockNutB");
            Assert.IsTrue(a.IsSubsetOf(b));
            Assert.IsFalse(b.IsSubsetOf(a));
        }
    }
}
