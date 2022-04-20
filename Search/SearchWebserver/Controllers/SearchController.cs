using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Search;

namespace SearchWebserver.Controllers
{
    public record SearchResult(double Score, int Chapter, int Verse, string Text);

    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<SearchController> _logger;
        private readonly Searcher.SearcherClient _grpcClient;


        public SearchController(ILogger<SearchController> logger, Searcher.SearcherClient grpcClient)
        {
            _logger = logger;
            _grpcClient = grpcClient;
        }


        [HttpGet]
        public async Task<IEnumerable<SearchResult>> Get(string term, int count = 10)
        {
            _logger.LogInformation($"Search for `{term}`");

            var request = new SearchRequest() { SearchTerm = term };
            var reply = await _grpcClient.DoSearchAsync(request);
            return reply
                .SearchResults
                .OrderByDescending(result => result.Score)
                .Take(count)
                .Select((result) => new SearchResult(result.Score, result.Chapter, result.Verse, result.Text))
                .ToArray();
        }
    }
}
