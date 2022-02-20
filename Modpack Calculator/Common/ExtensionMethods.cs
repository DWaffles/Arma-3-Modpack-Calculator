using System.Text.RegularExpressions;

namespace ModpackCalculator
{
    internal static class ExtensionMethods
    {
        private static Regex IdRegex { get; set; } = new Regex("[0-9]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex SpecialCharactersRegex { get; set; } = new Regex(@"\t|\n|\r", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // STRINGS
        public static ulong? GetModId(this string text)
        {
            MatchCollection matches = IdRegex.Matches(text);
            if (matches.Count > 0)
            {
                return Convert.ToUInt64(matches[0].Value);
            }
            else
            {
                return null;
            }
        }
        public static string StripSpecialCharacters(this string text)
        {
            return SpecialCharactersRegex.Replace(text, "");
        }
        public static string EscapeCSVString(this string str)
        {
            var mustQuote = str.Any(x => x == ',' || x == '\"' || x == '\r' || x == '\n');

            if (!mustQuote)
                return str;

            str = str.Replace("\"", "\"\"");
            return string.Format("\"{0}\"", str);
        }

        // ENUMS
        public static bool HasFlags(this Enum testThing, params Enum[] flags)
        {
            foreach (var flag in flags)
                if (!testThing.HasFlag(flag))
                    return false;
            return true;
        }
    }
}
