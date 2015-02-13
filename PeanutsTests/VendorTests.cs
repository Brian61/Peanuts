using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Peanuts;
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
            Nut.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var vendor = new Vendor();
            Bag bag = null;
            Assert.DoesNotThrow(() => bag = vendor.MakeBag(book.Get("RecipeA")));
            Assert.NotNull(bag);
        }

        [Test()]
        public void MakeBagTest1()
        {
            Nut.Initialize();
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
            Nut.Initialize();
            var vendor = new Vendor();
            Bag bag = null;
            Assert.DoesNotThrow(() => bag = vendor.MakeBag(typeof(MockNutA), typeof(MockNutB)));
            Assert.NotNull(bag);
            Assert.Catch(() => vendor.MakeBag(typeof (Bag)));
        }

        [Test()]
        public void MakeBagTest3()
        {
            Nut.Initialize();
            var vendor = new Vendor();
            Bag bag = null;
            Assert.DoesNotThrow(() => bag = vendor.MakeBag("MockNutA", "MockNutB"));
            Assert.NotNull(bag);
            Assert.Catch(() => vendor.MakeBag("MockNutB", "NoneSuch"));
        }

        [Test()]
        public void TryGetTest()
        {
            Nut.Initialize();
            var vendor = new Vendor();
            var bag = vendor.MakeBag("MockNutA");
            var id = bag.Id;
            Bag bagB;
            Assert.IsTrue(vendor.TryGet(id, out bagB));
            Assert.AreSame(bag, bagB);
            Assert.IsFalse(vendor.TryGet(10024, out bagB));
        }

        [Test()]
        public void GetTest()
        {
            Nut.Initialize();
            var vendor = new Vendor();
            var bagA = vendor.MakeBag("MockNutA");
            var bagB = vendor.MakeBag("MockNutA");
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
            Nut.Initialize();
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
            foreach (var bag in vendor.AllBags())
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
            Nut.Initialize();
            var vendor = new Vendor();
            var bag = vendor.MakeBag("MockNutA");
            Nut nut = new MockNutB { SomeFloat = 2.0f };
            Assert.DoesNotThrow(() => vendor.Add(bag, nut));
            Nut nut2 = null;
            Assert.IsTrue(bag.TryGet(Nut.GetId(typeof(MockNutB)), out nut2));
            Assert.AreSame(nut, nut2);
        }

        [Test()]
        public void RemoveTest()
        {
            Nut.Initialize();
            var vendor = new Vendor();
            var bag = vendor.MakeBag("MockNutA", "MockNutB");
            var id = Nut.GetId("MockNutB");
            var nut = bag.Get(id);
            Assert.DoesNotThrow(() => vendor.Remove(bag, nut));
            Assert.IsFalse(bag.TryGet(id, out nut));
        }

        [Test()]
        public void MorphTest()
        {
            Nut.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var vendor = new Vendor();
            var bagA = vendor.MakeBag(book.Get("RecipeA"));
            var idA = Nut.GetId("MockNutA");
            MockNutA mna = null;
            Assert.DoesNotThrow(() => mna = bagA.Get(idA) as MockNutA);
            mna.SomeText = "Quagmire";
            var bagB = vendor.MakeBag(book.Get("RecipeB"));
            Assert.DoesNotThrow(() => vendor.Morph(bagA, bagB));
            var idB = Nut.GetId("MockNutB");
            Nut nut;
            Assert.IsTrue(bagA.TryGet(idB, out nut));
            var mnb = nut as MockNutB;
            Assert.NotNull(mnb);
            Assert.AreEqual(mnb.SomeFloat, 4.0f);
            Assert.DoesNotThrow(() => mna = bagA.Get(idA) as MockNutA);
            Assert.NotNull(mna);
            Assert.AreEqual(mna.SomeText, "Quagmire");
            bagB = vendor.MakeBag(typeof (MockNutA));
            vendor.Morph(bagA, bagB);
            Assert.IsFalse(bagA.TryGet(idB, out nut));
        }

        [Test()]
        public void DiscardTest()
        {
            Nut.Initialize();
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

        [Test()]
        public void RegisterTest()
        {
            Nut.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var vendor = new Vendor();
            var bagA = vendor.MakeBag(book.Get("RecipeA"));
            MockProcess proc = null;
            Assert.DoesNotThrow(() => proc = new MockProcess(vendor, typeof (MockNutA)));
            Assert.AreEqual(1, proc.NotifiedBags.Count);
            Bag bagB = null;
            Assert.DoesNotThrow(() => bagB = proc.NotifiedBags[0]);
            Assert.AreSame(bagA, bagB);
        }

        [Test()]
        public void UnregisterTest()
        {
            Nut.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var vendor = new Vendor();
            var proc = new MockProcess(vendor, typeof (MockNutA));
            vendor.MakeBag(book.Get("RecipeA"));
            Assert.AreEqual(1, proc.NotifiedBags.Count);
            Assert.DoesNotThrow(() => vendor.Unregister(proc));
            vendor.MakeBag(book.Get("RecipeA"));
            Assert.AreEqual(1, proc.NotifiedBags.Count);
        }

        [Test]
        public void SerializeTest()
        {
            Nut.Initialize();
            var book = RecipeBook.Load(new StringReader(RecipeBookTests.JsonSample));
            var vendor = new Vendor();
            vendor.MakeBag(book.Get("RecipeA"));
            vendor.MakeBag(book.Get("RecipeB"));
            var json = JsonConvert.SerializeObject(vendor, Formatting.Indented, JsonHelper.OutputSettings);
            //Assert.AreEqual(json, "{}");
            vendor = JsonConvert.DeserializeObject<Vendor>(json, JsonHelper.InputSettings);
            Assert.AreEqual(2, vendor.AllBags().Count());
            var mix = new Mix(typeof (MockNutB));
            var countOfB = 0;
            foreach (var bag in vendor.AllBags())
            {
                var nuta = bag.Get(Nut.GetId(typeof (MockNutA))) as MockNutA;
                Assert.NotNull(nuta);
                Assert.AreEqual("Wee!!", nuta.SomeText);
                if (!bag.Contains(mix)) continue;
                var nutb = bag.Get(Nut.GetId(typeof (MockNutB))) as MockNutB;
                Assert.NotNull(nutb);
                Assert.AreEqual(4.0f, nutb.SomeFloat);
                countOfB++;
            }
            Assert.AreEqual(1, countOfB);
        }
    }
}
