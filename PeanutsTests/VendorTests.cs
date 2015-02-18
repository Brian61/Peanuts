using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Peanuts.Tests
{
    [TestFixture()]
    public class VendorTests
    {
        [Test()]
        public void VendorTest()
        {
            Assert.DoesNotThrow(() => new Vendor());
            Assert.NotNull(new Vendor());
        }

        [Test()]
        public void MakeBagTest()
        {
            Peanuts.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var vendor = new Vendor();
            Bag bag = null;
            Assert.DoesNotThrow(() => bag = vendor.MakeBag(book.Get("RecipeA")));
            Assert.NotNull(bag);
        }

        [Test()]
        public void MakeBagTest1()
        {
            Peanuts.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var vendor = new Vendor();
            var bagA = vendor.MakeBag(book.Get("RecipeA"));
            Bag bagB = null;
            Assert.DoesNotThrow(() => bagB = vendor.MakeBag(bagA));
            Assert.NotNull(bagB);
        }

        [Test()]
        public void MakeBagTest2()
        {
            Peanuts.Initialize();
            var vendor = new Vendor();
            Bag bag = null;
            Assert.DoesNotThrow(() => bag = vendor.MakeBag(typeof(MockNutA), typeof(MockNutB)));
            Assert.NotNull(bag);
            Assert.Catch(() => vendor.MakeBag(typeof (Bag)));
        }

        [Test()]
        public void TryGetTest()
        {
            Peanuts.Initialize();
            var vendor = new Vendor();
            var bag = vendor.MakeBag(typeof(MockNutA));
            var id = bag.Id;
            Bag bagB;
            Assert.IsTrue(vendor.TryGet(id, out bagB));
            Assert.AreSame(bag, bagB);
            Assert.IsFalse(vendor.TryGet(10024, out bagB));
        }

        [Test()]
        public void GetTest()
        {
            Peanuts.Initialize();
            var vendor = new Vendor();
            var bagA = vendor.MakeBag(typeof(MockNutA));
            var bagB = vendor.MakeBag(typeof(MockNutA));
            Assert.AreNotSame(bagA, bagB);
            Assert.AreNotEqual(bagA.Id, bagB.Id);
            Assert.DoesNotThrow(() => vendor.Get(bagA.Id));
            Assert.AreSame(bagA, vendor.Get(bagA.Id));
            Assert.AreSame(bagB, vendor.Get(bagB.Id));
            Assert.Catch(() => vendor.Get(33));
        }

        [Test()]
        public void AllBagsTest()
        {
            Peanuts.Initialize();
            var vendor = new Vendor();
            ISet<int> ids = new HashSet<int>();
            ISet<Bag> bags = new HashSet<Bag>();
            for (var i = 0; i < 10; i++)
            {
                var bag = vendor.MakeBag(typeof (MockNutA));
                ids.Add(bag.Id);
                bags.Add(bag);
            }
            Assert.AreEqual(ids.Count, 10);
            Assert.AreEqual(bags.Count, 10);
            foreach (var bag in vendor)
            {
                ids.Remove(bag.Id);
                bags.Remove(bag);
            }
            Assert.AreEqual(ids.Count, 0);
            Assert.AreEqual(bags.Count, 0);
        }

        [Test()]
        public void AddTest()
        {
            Peanuts.Initialize();
            var vendor = new Vendor();
            var bag = vendor.MakeBag(typeof(MockNutA));
            Nut nut = new MockNutB { SomeFloat = 2.0f };
            Assert.DoesNotThrow(() => vendor.Add(bag, nut));
            MockNutB nut2;
            Assert.IsTrue(bag.TryGet(out nut2));
            Assert.AreSame(nut, nut2);
        }

        [Test()]
        public void RemoveTest()
        {
            Peanuts.Initialize();
            var vendor = new Vendor();
            var bag = vendor.MakeBag(typeof(MockNutA), typeof(MockNutB));
            MockNutB nut = null;
            Assert.DoesNotThrow(() => nut = bag.Get<MockNutB>());
            Assert.DoesNotThrow(() => vendor.Remove(bag, nut));
            Assert.IsFalse(bag.TryGet(out nut));
        }

        [Test()]
        public void MorphTest()
        {
            Peanuts.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var vendor = new Vendor();
            var bagA = vendor.MakeBag(book.Get("RecipeA"));
            MockNutA mna = null;
            Assert.DoesNotThrow(() => mna = bagA.Get<MockNutA>());
            mna.SomeText = "Quagmire";
            var bagB = vendor.MakeBag(book.Get("RecipeB"));
            Assert.DoesNotThrow(() => vendor.Morph(bagA, bagB));
            MockNutB mnb;
            Assert.IsTrue(bagA.TryGet(out mnb));
            Assert.NotNull(mnb);
            Assert.AreEqual(mnb.SomeFloat, 4.0f);
            Assert.DoesNotThrow(() => mna = bagA.Get<MockNutA>());
            Assert.NotNull(mna);
            Assert.AreEqual(mna.SomeText, "Quagmire");
            bagB = vendor.MakeBag(typeof (MockNutA));
            vendor.Morph(bagA, bagB);
            Assert.IsFalse(bagA.TryGet(out mnb));
        }

        [Test()]
        public void DiscardTest()
        {
            Peanuts.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var vendor = new Vendor();
            var bagA = vendor.MakeBag(book.Get("RecipeA"));
            var bid = bagA.Id;
            var mix = new Mix(typeof (MockNutA));
            Assert.IsTrue(bagA.Contains(mix));
            Assert.DoesNotThrow(() => vendor.Discard(bagA));
            Assert.IsFalse(bagA.Contains(mix));
            Assert.IsFalse(vendor.TryGet(bid, out bagA));
        }

        [Test]
        public void SerializeTest()
        {
            Peanuts.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var vendor = new Vendor();
            vendor.MakeBag(book.Get("RecipeA"));
            vendor.MakeBag(book.Get("RecipeB"));
            var json = JsonConvert.SerializeObject(vendor, Formatting.Indented, Peanuts.OutputSettings);
            vendor = JsonConvert.DeserializeObject<Vendor>(json, Peanuts.InputSettings);
            Assert.AreEqual(2, vendor.Count());
            var mix = new Mix(typeof (MockNutB));
            var countOfB = 0;
            foreach (var bag in vendor)
            {
                MockNutA nuta = null;
                Assert.DoesNotThrow(() => nuta = bag.Get<MockNutA>());
                Assert.NotNull(nuta);
                Assert.AreEqual("Wee!!", nuta.SomeText);
                if (!bag.Contains(mix)) continue;
                MockNutB nutb = null;
                Assert.DoesNotThrow(() => nutb = bag.Get<MockNutB>());
                Assert.NotNull(nutb);
                Assert.AreEqual(4.0f, nutb.SomeFloat);
                countOfB++;
            }
            Assert.AreEqual(1, countOfB);
        }
    }
}
