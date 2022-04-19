#nullable enable

namespace Search.SearchCore
{
    public class Verse
    {
        public (int Chapter, int Verse) Reference;
        public string Text;
        public string SpacelessNormalizedText;

        public Verse(int chapter, int verseNumber, string text)
        {
            Reference = (chapter, verseNumber);
            Text = text;
            SpacelessNormalizedText = SearchHelpers
                .RemoveDiacriticsFromText(text.ToLower())
                .Replace(" ", "");
        }

        public override string ToString()
        {
            return $"{Reference.Chapter}:{Reference.Verse}";
        }
    }
}
