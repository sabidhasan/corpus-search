using NUnit.Framework;
using Search.SearchCore;

namespace SearchMicroserviceTest
{
    [TestFixture]
    public class SearchIndexMatchTests
    {
        Verse verse1;
        Verse verse2;

        [SetUp]
        public void Setup()
        {
            verse1 = new Verse(3, 5, "testing 123");
            verse2 = new Verse(3, 5, "testing 123");
        }

        [Test]
        public void GeneratesHashCodesForSameVerseReference()
        {
            var subject = new SearchIndexMatch(verse1, 0);
            var subject2 = new SearchIndexMatch(verse2, 0);
            Assert.AreEqual(subject.GetHashCode(), subject2.GetHashCode());
        }

        [Test]
        public void GeneratesHashCodesForVersesWithDifferentIndex()
        {
            var subject = new SearchIndexMatch(verse1, 0);
            var subject2 = new SearchIndexMatch(verse1, 5);
            Assert.AreEqual(subject.GetHashCode(), subject2.GetHashCode());
        }

        [Test]
        public void GetsCharactersIfRangeSmall()
        {
            var subject = new SearchIndexMatch(verse1, 0);
            Assert.AreEqual("testi", subject.GetNCharactersFromMatchPosition(5));
        }

        [Test]
        public void GetsCharactersIfRangeLarge()
        {
            var subject = new SearchIndexMatch(verse1, 7);
            Assert.AreEqual("123", subject.GetNCharactersFromMatchPosition(int.MaxValue));
        }

        [Test]
        public void GetsCharactersWithNormalizedText()
        {
            var subject = new SearchIndexMatch(verse1, 0);
            Assert.AreEqual("testing123", subject.GetNCharactersFromMatchPosition(int.MaxValue));
        }
    }
}
