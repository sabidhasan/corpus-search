using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Search;

namespace SearchWebserver.Services
{
    public class SearchService
    {
        private readonly Searcher.SearcherClient _grpcClient;

        public record SearchResult(double Score, int Chapter, int Verse, string Text);

        public SearchService(Searcher.SearcherClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public async Task<IEnumerable<SearchResult>> DoSearch(string searchTerm, int count = 10)
        {
            var request = new SearchRequest() { SearchTerm = searchTerm };
            var reply = await _grpcClient.DoSearchAsync(request);

            return reply
                .SearchResults
                .OrderByDescending(result => result.Score)
                .Take(count)
                .Select(r => new SearchResult(r.Score, r.Chapter, r.Verse, r.Text))
                .ToArray();
        }
    }
}
