#nullable enable

using System;
using System.Collections.Generic;

namespace Search.SearchCore
{
    public delegate double GetStringDistance(string stringA, string stringB);

    public class CorpusSearch
    {
        private readonly GetStringDistance StringDistanceComparator;
        private readonly int TokenizationGranularity;
        private readonly SearchIndex SearchIndex;

        public CorpusSearch(GetStringDistance comparator, int tokenizationGranularity, SearchIndex searchIndex)
        {
            StringDistanceComparator = comparator;
            TokenizationGranularity = tokenizationGranularity;
            SearchIndex = searchIndex;
        }

        public Dictionary<Verse, double> DoSearch(string searchTerm, double threshold=0)
        {
            Dictionary<Verse, double> searchResults = new();
            string searchWords = SearchHelpers.RemoveDiacriticsFromText(searchTerm.ToLower().Replace(" ", ""));

            IEnumerable<string> tokens = SearchHelpers.GenerateTokens(searchWords, TokenizationGranularity);

            foreach (string token in tokens)
            {
                // Look up in search index and score candidates
                IEnumerable<SearchIndexMatch> candidates = SearchIndex.GetCandidateVersesForToken(token);

                foreach (SearchIndexMatch candidate in candidates)
                {
                    // Compute lexical similarity
                    double currentCandidateScore = ComputeLexicalSimilarity(searchWords, candidate);
                    if (currentCandidateScore < threshold) continue;

                    searchResults.TryGetValue(candidate.Verse, out double previousCandidateScore);
                    searchResults[candidate.Verse] = Math.Max(previousCandidateScore, currentCandidateScore);
                }
            }

            return searchResults;
        }

        private double ComputeLexicalSimilarity(string searchTerm, SearchIndexMatch candidate)
        {
            double lexicalSimilarity = 0;

            for (int i = searchTerm.Length;  i < searchTerm.Length + 4; i++)
            {
                string candidateVerseText = candidate.GetNCharactersFromMatchPosition(i);
                double currentSimilarity = StringDistanceComparator(searchTerm, candidateVerseText);
                lexicalSimilarity = Math.Max(lexicalSimilarity, currentSimilarity);
            }

            return lexicalSimilarity;
        }
    }
}
