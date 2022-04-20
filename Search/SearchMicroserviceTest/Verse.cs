using NUnit.Framework;
using Search.SearchCore;

namespace SearchMicroserviceTest
{
    [TestFixture]
    public class VerseTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void StringifiesTheVerse()
        {
            var verse = new Verse(1, 2, "");
            Assert.AreEqual(verse.ToString(), "1:2");
        }

        [Test]
        public void RemovesDiacritics()
        {
            var verse = new Verse(1, 2, "some `test ža 'here al-test");
            Assert.AreEqual(verse.SpacelessNormalizedText, "sometestzaherealtest");
        }
    }
}
