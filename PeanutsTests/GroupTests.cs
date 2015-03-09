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
    public class GroupTests
    {
        [Test()]
        public void GroupTest()
        {
            Group group =null;
            Assert.DoesNotThrow(() => group = new Group());
            Assert.IsNotNull(group);
        }

        [Test()]
        public void AddListenerTest()
        {
            Action<Entity, Type, bool> listener = (Entity entity, Type ctype, bool added) => { };
            var group = new Group();
            Assert.DoesNotThrow(() => group.AddListener(listener));
        }

        [Test()]
        public void RemoveListenerTest()
        {
            Action<Entity, Type, bool> listener = (Entity entity, Type ctype, bool added) => { };
            var group = new Group();
            group.AddListener(listener);
            Assert.DoesNotThrow(() => group.RemoveListener(listener));
        }

        [Test()]
        public void AddTest()
        {
            Peanuts.Initialize();
            var entity = new Entity();
            var group = new Group();
            Assert.DoesNotThrow(() => group.Add(entity));
        }

        [Test()]
        public void RemoveTest()
        {
            Peanuts.Initialize();
            var entity = new Entity();
            var group = new Group();
            group.Add(entity);
            Assert.DoesNotThrow(() => group.Remove(entity));
        }

        [Test()]
        public void GetTest()
        {
            Peanuts.Initialize();
            var entity = new Entity();
            var id = entity.Id;
            var group = new Group();
            group.Add(entity);
            Entity result = null;
            Assert.DoesNotThrow(() => result = group.Get(id));
            Assert.IsNotNull(result);
            Assert.AreSame(entity, result);
        }

        [Test()]
        public void TryGetTest()
        {
            Peanuts.Initialize();
            var entity = new Entity();
            var id = entity.Id;
            var group = new Group();
            group.Add(entity);
            Entity result = null;
            bool success = false;
            Assert.DoesNotThrow(() => success = group.TryGet(id, out result));
            Assert.IsTrue(success);
            Assert.IsNotNull(result);
            Assert.AreSame(entity, result);
            Assert.DoesNotThrow(() => success = group.TryGet(1000000, out result));
            Assert.IsFalse(success);
            Assert.IsNull(result);
        }

        [Test()]
        public void GetEnumeratorTest()
        {
            Peanuts.Initialize();
            var list = new List<Entity>();
            var group = new Group();
            for (var i = 0; i < 10; i++)
            {
                var e = new Entity();
                list.Add(e);
                group.Add(e);
            }
            var ids = list.Select(e => e.Id).ToList();
            Assert.AreEqual(10, group.Count());
            foreach (var ent in group)
            {
                Assert.IsTrue(ids.Contains(ent.Id));
                ids.Remove(ent.Id);
            }
        }
    }
}
