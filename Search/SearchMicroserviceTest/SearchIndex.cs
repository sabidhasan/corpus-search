using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Search.SearchCore;

namespace SearchMicroserviceTest
{
    [TestFixture]
    public class SearchIndexTests
    {
        private SearchIndex Subject;
        private const string MockVerseText = "this is a verse verse text";
        private const string MockVerse2Text = "some other text";

        [SetUp]
        public void Setup()
        {
            var verses = new List<Verse>()
            {
                new Verse(1, 1, MockVerseText),
                new Verse(1, 2, MockVerse2Text),
            };

            static string[] tokenGenerator(string t) => (t == "someothertext" ? MockVerse2Text : MockVerseText).Split(" ");
            Subject = new SearchIndex(verses, tokenGenerator);
        }

        [Test]
        public void SearchIndexWithOneMatch()
        {
            var verseMatch = Subject.GetCandidateVersesForToken("this").ToArray();

            Assert.AreEqual(verseMatch[0].MatchIndex, 0);
            Assert.AreEqual(verseMatch[0].Verse.Text, MockVerseText);
        }

        [Test]
        public void SearchIndexWithNoMatches()
        {
            var verseMatch = Subject.GetCandidateVersesForToken("nonexistent");

            Assert.AreEqual(verseMatch.Count(), 0);
        }

        [Test]
        public void SearchIndexWithTwoMatches()
        {
            var verseMatch = Subject.GetCandidateVersesForToken("verse").ToArray();

            Assert.AreEqual(verseMatch.Length, 1);
            Assert.AreEqual(verseMatch[0].MatchIndex, 3);
        }

        [Test]
        public void SearchIndexFromMultipleVerses()
        {
            var match = Subject.GetCandidateVersesForToken("text").ToArray();

            Assert.AreEqual(match.Length, 2);
            Assert.AreEqual(match[0].Verse.Reference.Verse, 1);
            Assert.AreEqual(match[1].Verse.Reference.Verse, 2);
        }
    }
}
