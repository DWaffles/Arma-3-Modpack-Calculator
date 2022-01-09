using System.Text.RegularExpressions;

namespace ModpackCalculator
{
    internal static class RegexHelper
    {
        private static Regex RegexId { get; set; } = new Regex("[0-9]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex RegexName { get; set; } = new Regex(@"\t|\n|\r", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static ulong? GetModId(string text)
        {
            MatchCollection matches = RegexId.Matches(text);
            if (matches.Count > 0)
            {
                return Convert.ToUInt64(matches[0].Value);
            }
            else
            {
                return null;
            }
        }
        public static string StripSpecialCharacters(string text)
        {
            return RegexName.Replace(text, "");
        }
    }
}
