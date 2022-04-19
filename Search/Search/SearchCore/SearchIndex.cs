#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Search.SearchCore
{
    public class SearchIndex
    {
        public readonly Dictionary<string, HashSet<SearchIndexMatch>> searchIndex = new();

        public SearchIndex(List<Verse> verses, Func<string, IEnumerable<string>> tokenGenerator)
        {
            foreach (Verse verse in verses)
            {
                string wordsInCurrentVerse = verse.SpacelessNormalizedText;
                var tokens = tokenGenerator(wordsInCurrentVerse).ToArray();

                for (int i = 0; i < tokens.Length; i++)
                {
                    searchIndex.TryAdd(tokens[i], new HashSet<SearchIndexMatch>());
                    searchIndex[tokens[i]].Add(new SearchIndexMatch(verse, i));
                }
            }
        }

        public IEnumerable<SearchIndexMatch> GetCandidateVersesForToken(string token)
        {
            if (searchIndex.ContainsKey(token))
            {
                return searchIndex[token].ToList();
            }

            return new List<SearchIndexMatch>();
        }
    }
}
