using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Search.SearchCore;

namespace Search
{
    public class SearchService : Searcher.SearcherBase
    {
        private readonly ILogger<SearchService> _logger;
        private readonly CorpusSearch searcher;

        public SearchService(ILogger<SearchService> logger)
        {
            _logger = logger;

            int tokenizationGranularity = 3;
            var verses = GetVerses(114);
            IEnumerable<string> tokenGenerator(string word) => SearchHelpers.GenerateTokens(word, tokenizationGranularity);
            var searchIndex = new SearchIndex(verses, tokenGenerator);
            searcher = new CorpusSearch(JaroWinklerDistance.Proximity, tokenizationGranularity, searchIndex);
        }

        public override Task<SearchReply> DoSearch(SearchRequest request, ServerCallContext context)
        {
            var response = new SearchReply();

            var results = searcher.DoSearch(request.SearchTerm, 0.85);
            foreach (var verseScorePair in results.OrderBy(r => r.Value * -1))
            {
                var result = new SearchResult()
                {
                    Chapter = verseScorePair.Key.Reference.Chapter,
                    Verse = verseScorePair.Key.Reference.Verse,
                    Text = verseScorePair.Key.Text,
                    Score = verseScorePair.Value,
                };
                response.SearchResults.Add(result);
            }

            return Task.FromResult(response);
        }

        private static List<Verse> GetVerses(int chapterCount)
        {
            var verses = new List<Verse>();

            for (int chapter = 1; chapter <= chapterCount; chapter++)
            {
                var chapterVerses = ReadChapterFromDisk(chapter);
                foreach (var verseObject in chapterVerses)
                {
                    string verseText = verseObject.phonetic;
                    var verseNumber = int.Parse(verseText.Split(" ")[0].Split(".")[1]);
                    var rawText = string.Join(" ", verseText.Split(" ").Skip(1));

                    verses.Add(new Verse(chapter, verseNumber, rawText));

                }
            }

            return verses;
        }

        private static List<dynamic> ReadChapterFromDisk(int chapter)
        {
            var jsonText = File.ReadAllText($"corpus/_{chapter}.json");
            var corpusText = JsonConvert.DeserializeObject<List<dynamic>>(jsonText);
            return corpusText ?? new List<dynamic>();
        }
    }
}
