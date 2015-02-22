﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Peanuts.Tests
{
    [TestFixture()]
    public class GroupTests
    {
        [Test()]
        public void GroupTest()
        {
            Assert.DoesNotThrow(() => new Group());
            Assert.NotNull(new Group());
        }

        [Test()]
        public void MakeBagTest()
        {
            Peanuts.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var group = new Group();
            Entity entity = null;
            Assert.DoesNotThrow(() => entity = group.NewEntity(book.Get("RecipeA")));
            Assert.NotNull(entity);
        }

        [Test()]
        public void MakeBagTest1()
        {
            Peanuts.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var group = new Group();
            var entA = group.NewEntity(book.Get("RecipeA"));
            Entity entB = null;
            Assert.DoesNotThrow(() => entB = group.NewEntity(entA));
            Assert.NotNull(entB);
        }

        [Test()]
        public void MakeBagTest2()
        {
            Peanuts.Initialize();
            var group = new Group();
            Entity entity = null;
            Assert.DoesNotThrow(() => entity = group.NewEntity(typeof(MockEntityA), typeof(MockEntityB)));
            Assert.NotNull(entity);
            Assert.Catch(() => group.NewEntity(typeof (Entity)));
        }

        [Test()]
        public void TryGetTest()
        {
            Peanuts.Initialize();
            var group = new Group();
            var entity = group.NewEntity(typeof(MockEntityA));
            var id = entity.Id;
            Entity entB;
            Assert.IsTrue(group.TryGet(id, out entB));
            Assert.AreSame(entity, entB);
            Assert.IsFalse(group.TryGet(10024, out entB));
        }

        [Test()]
        public void GetTest()
        {
            Peanuts.Initialize();
            var group = new Group();
            var entA = group.NewEntity(typeof(MockEntityA));
            var entB = group.NewEntity(typeof(MockEntityA));
            Assert.AreNotSame(entA, entB);
            Assert.AreNotEqual(entA.Id, entB.Id);
            Assert.DoesNotThrow(() => group.Get(entA.Id));
            Assert.AreSame(entA, group.Get(entA.Id));
            Assert.AreSame(entB, group.Get(entB.Id));
            Assert.Catch(() => group.Get(33));
        }

        [Test()]
        public void AllBagsTest()
        {
            Peanuts.Initialize();
            var group = new Group();
            ISet<int> ids = new HashSet<int>();
            ISet<Entity> bags = new HashSet<Entity>();
            for (var i = 0; i < 10; i++)
            {
                var entity = group.NewEntity(typeof (MockEntityA));
                ids.Add(entity.Id);
                bags.Add(entity);
            }
            Assert.AreEqual(ids.Count, 10);
            Assert.AreEqual(bags.Count, 10);
            foreach (var entity in group)
            {
                ids.Remove(entity.Id);
                bags.Remove(entity);
            }
            Assert.AreEqual(ids.Count, 0);
            Assert.AreEqual(bags.Count, 0);
        }

        [Test()]
        public void AddTest()
        {
            Peanuts.Initialize();
            var group = new Group();
            var entity = group.NewEntity(typeof(MockEntityA));
            Component component = new MockEntityB { SomeFloat = 2.0f };
            Assert.DoesNotThrow(() => entity.Add(component));
            MockEntityB nut2;
            Assert.IsTrue(entity.TryGet(out nut2));
            Assert.AreSame(component, nut2);
        }

        [Test()]
        public void RemoveTest()
        {
            Peanuts.Initialize();
            var group = new Group();
            var entity = group.NewEntity(typeof(MockEntityA), typeof(MockEntityB));
            MockEntityB component = null;
            Assert.DoesNotThrow(() => component = entity.Get<MockEntityB>());
            Assert.DoesNotThrow(() => entity.Remove(component));
            Assert.IsFalse(entity.TryGet(out component));
        }

        [Test()]
        public void MorphTest()
        {
            Peanuts.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var group = new Group();
            var entA = group.NewEntity(book.Get("RecipeA"));
            MockEntityA mna = null;
            Assert.DoesNotThrow(() => mna = entA.Get<MockEntityA>());
            mna.SomeText = "Quagmire";
            var entB = group.NewEntity(book.Get("RecipeB"));
            Assert.DoesNotThrow(() => entA.Morph(entB));
            MockEntityB mnb;
            Assert.IsTrue(entA.TryGet(out mnb));
            Assert.NotNull(mnb);
            Assert.AreEqual(mnb.SomeFloat, 4.0f);
            Assert.DoesNotThrow(() => mna = entA.Get<MockEntityA>());
            Assert.NotNull(mna);
            Assert.AreEqual(mna.SomeText, "Quagmire");
            entB = group.NewEntity(typeof (MockEntityA));
            entA.Morph(entB);
            Assert.IsFalse(entA.TryGet(out mnb));
        }

        [Test()]
        public void DiscardTest()
        {
            Peanuts.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var group = new Group();
            var entA = group.NewEntity(book.Get("RecipeA"));
            var bid = entA.Id;
            var mix = new TagSet(typeof (MockEntityA));
            Assert.IsTrue(entA.Contains(mix));
            Assert.DoesNotThrow(() => group.Discard(entA));
            Assert.IsFalse(entA.Contains(mix));
            Assert.IsFalse(group.TryGet(bid, out entA));
        }

        [Test]
        public void SerializeTest()
        {
            Peanuts.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var group = new Group();
            group.NewEntity(book.Get("RecipeA"));
            group.NewEntity(book.Get("RecipeB"));
            var json = JsonConvert.SerializeObject(group, Formatting.Indented, Peanuts.OutputSettings);
            group = JsonConvert.DeserializeObject<Group>(json, Peanuts.InputSettings);
            Assert.AreEqual(2, group.Count());
            var mix = new TagSet(typeof (MockEntityB));
            var countOfB = 0;
            foreach (var entity in group)
            {
                MockEntityA nuta = null;
                Assert.DoesNotThrow(() => nuta = entity.Get<MockEntityA>());
                Assert.NotNull(nuta);
                Assert.AreEqual("Wee!!", nuta.SomeText);
                if (!entity.Contains(mix)) continue;
                MockEntityB nutb = null;
                Assert.DoesNotThrow(() => nutb = entity.Get<MockEntityB>());
                Assert.NotNull(nutb);
                Assert.AreEqual(4.0f, nutb.SomeFloat);
                countOfB++;
            }
            Assert.AreEqual(1, countOfB);
        }
    }
}
