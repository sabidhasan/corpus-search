#nullable enable

using System.Collections.Generic;
using System.Text;

namespace Search.SearchCore
{
    public static class SearchHelpers
    {
        public static IEnumerable<string> GenerateTokens(string word, int granularity)
        {
            if (granularity == 0)
            {
                yield break;
            }

            int index = 0;
            while (index <= (word.Length - granularity))
            {
                yield return word.Substring(index, granularity);
                index++;
            }
        }

        public static string RemoveDiacriticsFromText(string text)
        {
            var replacements = new Dictionary<string, string>()
            {
                { "ī", "i" },
                { "/", "" },
                { "ş", "s" },
                { "ž", "z" },
                { "ĥ", "h" },
                { "ū", "u" },
                { "ţ", "t" },
                { "ā", "a" },
                { "á", "a" },
                { "'", "" },
                { "đ", "d" },
                { "`", "" },
                { "-", " " },
            };

            var ret = new StringBuilder();

            foreach (char letter in text)
            {
                string replacement = letter.ToString();
                if (replacements.ContainsKey(replacement))
                {
                    replacement = replacements[replacement];
                }

                ret.Append(replacement);
            }

            return ret.ToString();
        }
    }
}
