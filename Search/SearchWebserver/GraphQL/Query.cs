using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using Search;
using SearchWebserver.Services;

namespace SearchWebserver.GraphQL
{
    public class Query
    {
        private readonly SearchService _searchService;

        public Query(Searcher.SearcherClient grpcClient)
        {
            _searchService = new SearchService(grpcClient);
        }

        public async Task<IEnumerable<SearchType>> GetSearch([GraphQLName("term")] string searchTerm)
        {
            var results = await _searchService.DoSearch(searchTerm);
            return results
                .Select(res => new SearchType() {
                    Chapter = res.Chapter, Verse = res.Verse, Score = res.Score, Text = res.Text
                });
        }
    }
}
