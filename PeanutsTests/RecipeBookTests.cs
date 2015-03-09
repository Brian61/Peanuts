using System.IO;
using System.Text;
using NUnit.Framework;

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
            var tsa = new TagSet(book.GetRecipe("RecipeA").Components());
            var tsb = new TagSet(typeof(MockComponentA));
            Assert.IsTrue(0 == tsa.CompareTo(tsb));
            Assert.IsTrue(book.Contains("RecipeB"));
            Assert.IsFalse(book.Contains("mananana"));
        }
    }
}
