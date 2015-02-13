using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peanuts;
using NUnit.Framework;
namespace Peanuts.Tests
{
    [TestFixture()]
    public class RecipeBookTests
    {
        public const string JsonSample = @"{
    'RecipeA':  {
            'MockNutA': {
                'SomeText': 'Wee!!'
                        }
                },
    'RecipeB':  {
            'Prototype': 'RecipeA',
            'MockNutB': {
                'SomeFloat': 4.0
                        }
                }
}";

        [Test()]
        public void LoadTest()
        {
            Assert.DoesNotThrow(() => RecipeBook.Load(new StringReader(JsonSample)));
        }

        [Test()]
        public void TryGetTest()
        {
            var book = RecipeBook.Load(new StringReader(JsonSample));
            Recipe recipe;
            Assert.IsTrue(book.TryGet("RecipeA", out recipe));
            Assert.NotNull(recipe);
            Assert.AreEqual(recipe.Name, "RecipeA");
            Assert.IsFalse(book.TryGet("NoneSuch", out recipe));
        }

        [Test()]
        public void GetTest()
        {
            var book = RecipeBook.Load(new StringReader(JsonSample));
            Assert.Catch(() => book.Get("NoneSuch"));
            Assert.DoesNotThrow(() => book.Get("RecipeB"));
            Assert.IsNotNull(book.Get("RecipeB"));
        }
    }
}
