using System.IO;
using System.Text;
using NUnit.Framework;
using System.Linq;

namespace Peanuts.Tests
{
    [TestFixture()]
    public class RecipeBookTests
    {
        public const string JsonSample = @"{
    'RecipeA':  {
            'MockComponentA': {
                'SomeText': 'Wee!!'
                        }
                },
    'RecipeB':  {
            'Prototype': 'RecipeA',
            'MockComponentB': {
                'SomeFloat': 4.0
                        }
                }
}";

        public static Stream GetSampleStream()
        {
            var js = JsonSample.Replace('\'', '"');
            return new MemoryStream(Encoding.UTF8.GetBytes(js));
        }

        [Test()]
        public void LoadTest()
        {
            RecipeBook book = new RecipeBook();
            using (var stream = RecipeBookTests.GetSampleStream())
            {
                Assert.DoesNotThrow(() => JsonRecipe.LoadCollection(book, stream));
            }
        }

        [Test()]
        public void ContainsTest()
        {
            RecipeBook book = new RecipeBook();
            using (var stream = GetSampleStream())
            {
                JsonRecipe.LoadCollection(book, stream);
            }
            Assert.IsTrue(book.Contains("RecipeA"));
            var tsa = book.GetRecipe("RecipeA").Components().Select(c => c.GetType());
            Assert.IsTrue(tsa.Contains(typeof(MockComponentA)));
            Assert.AreEqual(1, tsa.Count());
            Assert.IsTrue(book.Contains("RecipeB"));
            Assert.IsFalse(book.Contains("mananana"));
        }
    }
}
