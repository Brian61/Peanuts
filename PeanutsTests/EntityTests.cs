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
    public class EntityTests
    {
        [Test()]
        public void EntityTest()
        {
            Peanuts.Initialize();
            Entity entity =null;
            Assert.DoesNotThrow(() => entity = new Entity());
            Assert.NotNull(entity);
            Assert.Greater(entity.Id, 0);
        }

        [Test()]
        public void EntityTest1()
        {
            Peanuts.Initialize();
            var comps = new List<Component>();
            comps.Add(new MockComponentA());
            Entity entity = null;
            Assert.DoesNotThrow(() => entity = new Entity(comps));
            Assert.NotNull(entity);
            Assert.Greater(entity.Id, 0);
            Assert.True(entity.Contains(typeof(MockComponentA)));
        }

        [Test()]
        public void EntityTest2()
        {
            Peanuts.Initialize();
            var comps = new List<Component>();
            comps.Add(new MockComponentA());
            Entity source = new Entity(comps);
            Entity entity = null;
            Assert.DoesNotThrow(() => entity = Entity.FromPrototype(source));
            Assert.NotNull(entity);
            Assert.Greater(entity.Id, 0);
            Assert.True(entity.Contains(typeof(MockComponentA)));
        }

        [Test()]
        public void EntityTest3()
        {
            Peanuts.Initialize();
            var comps = new List<Component>();
            comps.Add(new MockComponentA());
            Entity entity = null;
            Assert.DoesNotThrow(() => entity = new Entity(comps.ToArray()));
            Assert.NotNull(entity);
            Assert.Greater(entity.Id, 0);
            Assert.True(entity.Contains(typeof(MockComponentA)));
        }

        [Test()]
        public void GetTest()
        {
            Peanuts.Initialize();
            var comps = new List<Component>();
            comps.Add(new MockComponentA());
            Entity entity = new Entity(comps);
            Component gotten = null;
            Assert.DoesNotThrow(() => gotten = entity.Get<MockComponentA>());
            Assert.AreSame(comps[0], gotten);
        }

        [Test()]
        public void TryGetTest()
        {
            Peanuts.Initialize();
            var comps = new List<Component>();
            comps.Add(new MockComponentA());
            Entity entity = new Entity(comps);
            MockComponentA gotten = null;
            bool result = false;
            Assert.DoesNotThrow(() => result = entity.TryGet<MockComponentA>(out gotten));
            Assert.IsTrue(result);
            Assert.AreSame(comps[0], gotten);
            MockComponentB tried;
            result = entity.TryGet<MockComponentB>(out tried);
            Assert.False(result);
            Assert.IsNull(tried);
        }

        [Test()]
        public void ContainsTest1()
        {
            Peanuts.Initialize();
            var comps = new List<Component>();
            comps.Add(new MockComponentA());
            Entity entity = new Entity(comps);
            Assert.True(entity.Contains(typeof(MockComponentA)));
            Assert.False(entity.Contains(typeof(MockComponentB)));
        }

        [Test()]
        public void ContainsTest2()
        {
            Peanuts.Initialize();
            var comps = new List<Component>();
            comps.Add(new MockComponentA());
            comps.Add(new MockComponentB());
            Entity entity = new Entity(comps);
            Assert.True(entity.Contains(typeof(MockComponentA), typeof(MockComponentB)));
            Assert.False(entity.Contains(typeof(MockComponentB), typeof(MockComponentC)));
        }

        [Test()]
        public void AddTest()
        {
            Peanuts.Initialize();
            var comps = new List<Component>();
            comps.Add(new MockComponentA());
            Entity entity = new Entity(comps);
            var mkc = new MockComponentC();
            Assert.DoesNotThrow(() => entity.Add(mkc));
            Assert.True(entity.Contains(mkc.GetType()));
        }

        [Test()]
        public void RemoveTest()
        {
            Peanuts.Initialize();
            var comps = new List<Component>();
            comps.Add(new MockComponentA());
            comps.Add(new MockComponentB());
            Entity entity = new Entity(comps);
            var mka = comps[0];
            Assert.DoesNotThrow(() => entity.Remove(mka));
            Assert.False(entity.Contains(mka.GetType()));
        }

        [Test()]
        public void MorphTest()
        {
            Peanuts.Initialize();
            var comps = new List<Component>();
            comps.Add(new MockComponentA());
            comps.Add(new MockComponentB());
            Entity entity = new Entity(comps);
            var comps2 = new List<Component>();
            comps2.Add(new MockComponentB());
            comps2.Add(new MockComponentC());
            Entity entity2 = new Entity(comps2);
            Assert.DoesNotThrow(() => entity.Morph(entity2));
            Assert.True(entity.Contains(typeof(MockComponentC)));
            Assert.False(entity.Contains(typeof(MockComponentA)));
            Assert.AreSame(comps[1], entity.Get<MockComponentB>());
            Assert.AreNotSame(comps2[1], entity.Get<MockComponentC>());
        }
    }
}
