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
            'MockEntityA': {
                'SomeText': 'Wee!!'
                        }
                },
    'RecipeB':  {
            'Prototype': 'RecipeA',
            'MockEntityB': {
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
            Assert.DoesNotThrow(() => RecipeBook.Load(GetSampleStream()));
        }

        [Test()]
        public void ContainsTest()
        {
            var book = RecipeBook.Load(GetSampleStream());
            Assert.IsTrue(book.Contains("RecipeA"));
            var tsa = book.GetTagSetFor("RecipeA");
            var tsb = new TagSet(typeof(MockEntityA));
            Assert.IsTrue(0 == tsa.CompareTo(tsb));
            Assert.IsTrue(book.Contains("RecipeB"));
            Assert.IsFalse(book.Contains("mananana"));
        }
    }
}
