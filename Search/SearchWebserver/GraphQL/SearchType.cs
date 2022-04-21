using Search;

namespace SearchWebserver.GraphQL
{
    public class SearchType
    {
        public int Chapter { get; set; }
        public int Verse { get; set; }
        public double Score { get; set; }
        public string Text { get; set; }
    }
}
