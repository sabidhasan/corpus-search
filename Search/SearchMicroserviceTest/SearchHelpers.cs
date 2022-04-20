using System.Collections.Generic;
using NUnit.Framework;
using Search.SearchCore;

namespace SearchMicroserviceTest
{
    [TestFixture]
    public class SearchHelpersTests
    {
        [Test]
        public void RemovesDiacritics()
        {
            var text = "žūţ alors! āáaĥ! đīş be-ĥárd /'`hah";
            var subject = SearchHelpers.RemoveDiacriticsFromText(text);

            Assert.AreEqual("zut alors! aaah! dis be hard hah", subject);
        }

        [Test]
        public void GenerateTokensWithHighIndex()
        {
            var subject = SearchHelpers.GenerateTokens("test", 100);

            CollectionAssert.AreEqual(new List<string>(), subject);
        }

        [Test]
        public void GenerateTokens()
        {
            var subject = SearchHelpers.GenerateTokens("test", 3);

            CollectionAssert.AreEqual(new List<string> { "tes", "est" }, subject);
        }

        [Test]
        public void GenerateTokensEmptyString()
        {
            var subject = SearchHelpers.GenerateTokens("", 3);

            CollectionAssert.AreEqual(new List<string>(), subject);
        }

        [Test]
        public void GenerateTokensZeroGranularity()
        {
            var subject = SearchHelpers.GenerateTokens("test", 0);

            CollectionAssert.AreEqual(new List<string>(), subject);
        }

        [Test]
        public void GenerateTokensZeroGranularityEmptyString()
        {
            var subject = SearchHelpers.GenerateTokens("", 0);

            CollectionAssert.AreEqual(new List<string>(), subject);
        }
    }
}
