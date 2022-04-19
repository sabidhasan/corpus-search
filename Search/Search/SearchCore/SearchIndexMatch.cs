#nullable enable

using System;

namespace Search.SearchCore
{
    public class SearchIndexMatch
    {
        public int MatchIndex;
        public Verse Verse;

        public SearchIndexMatch(Verse verse, int matchIndex)
        {
            MatchIndex = matchIndex;
            Verse = verse;
        }

        public override int GetHashCode()
        {
            string str = $"{Verse.Reference.Chapter}-{Verse.Reference.Verse}";
            return string.GetHashCode(str);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !GetType().Equals(obj.GetType()))
            {
                return false;
            }

            SearchIndexMatch match = (SearchIndexMatch)obj;
            return match.Verse.Reference.Chapter == Verse.Reference.Chapter &&
                match.Verse.Reference.Verse == Verse.Reference.Verse;
        }

        public string GetNCharactersFromMatchPosition(int characterCount)
        {
            int verseLengthFromIndex = Verse.SpacelessNormalizedText.Length - MatchIndex;
            int maxPossibleCharacters = Math.Min(characterCount, verseLengthFromIndex);
            return Verse.SpacelessNormalizedText.Substring(MatchIndex, maxPossibleCharacters);
        }
    }
}
